using ush4.ViewModels.Disp;
using CsvHelper;
using ush4.Models.ELMO;
using ush4.ViewModels.ELMO;
using ElmoMotionControlComponents.Drive.EASComponents.Recording;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using ush4.ViewModels.Recorder;
using ush4.Views.Windows;
using System.Windows.Shapes;
using CsvHelper.Configuration;

namespace ush4.ViewModels
{
    public class MotionController_VM : DriveViewModel<Models.MotionControllerModel>, IRecorderVM
    {
        const int RECORDWAIT_TIME_MS = 50;
        Double current_recorder_gap_s = 0;
        Double record_countdown_ms = 0;

        protected Models.ElmoCommandHandler ModifiedCommands { get { return (Models.ElmoCommandHandler)this.model.Commands; } }

        private int feedback_pos;
        public int FeedbackPosition
        {
            get { return feedback_pos; }
            set
            {
                feedback_pos = value;
                OnPropertyChanged("FeedbackPosition");
            }
        }

        private Boolean user_programm_error;
        public Boolean IsUserProgramInError
        {
            get { return user_programm_error; }
            set
            {
                user_programm_error = value;
                OnPropertyChanged("IsUserProgramInError");
            }
        }

        private Boolean is_play = false;

        public Boolean IsPlaying
        {
            get { return is_play; }
            private set
            {
                is_play = value;
                OnPropertyChanged("IsPlaying");
            }
        }

        private Boolean is_can_measure = false;
        public Boolean IsCanMeasure
        {
            get { return is_can_measure; }
            private set
            {
                is_can_measure = value;
                OnPropertyChanged("IsCanMeasure");
            }
        }

        private Boolean is_homing = false;
        public Boolean IsHome
        {
            get { return is_homing; }
            private set
            {
                is_homing = value;
                OnPropertyChanged("IsHome");
            }
        }

        private Boolean is_center = false;
        public Boolean IsCenter
        {
            get { return is_center; }
            set
            {
                if (is_center != value)
                {
                    is_center = value;
                    OnPropertyChanged("IsCenter");

                    if (is_center)
                    {
                        SetNewStatusDispatcher(DeviceStateViewModel.enDeviceStates.Ok,
                                                Properties.ResourcesE.DeviceIsReady);
                    }
                }
            }
        }

        private Boolean is_steady = false;
        public Boolean IsSteady
        {
            get { return is_steady; }
            set
            {
                is_steady = value;
                OnPropertyChanged("IsSteady");
            }
        }


        private Double set_frequency_Hz;
        public Double SetFrequencyHz
        {
            get { return set_frequency_Hz; }
            private set
            {
                set_frequency_Hz = value;
                OnPropertyChanged("SetFrequencyHz");
            }
        }

        private Double set_displacement_m;

        public Double SetDisplacement_m
        {
            get { return set_displacement_m; }
            private set
            {
                set_displacement_m = value;
                OnPropertyChanged("SetDisplacement_m");
            }
        }


        private double currentFrequencyHz;
        public double CurrentFrequency_Hz
        {
            get { return currentFrequencyHz; }
            set
            {
                currentFrequencyHz = value;
                OnPropertyChanged("CurrentFrequency_Hz");
            }
        }


        private Double currentAmplitude_m;
        public Double CurrentAmplitude_m
        {
            get { return currentAmplitude_m; }
            set
            {
                currentAmplitude_m = value;
                OnPropertyChanged("CurrentAmplitude_m");
            }
        }

        private Double resPress;
        public Double ResPress
        {
            get { return resPress; }
            set
            {
                resPress = value;
                OnPropertyChanged("ResPress");
            }
        }

        private int answer;
        public int Answer
        {
            get { return answer; }
            set
            {
                answer = value;
                OnPropertyChanged("Answer");
            }        
        }

        private int status;
        public int Status
        {
            get { return status; }
            set
            {
                status = value;
                OnPropertyChanged("Status");
            }
        }

        private int _sm_state;
        public int SmState
        {
            get { return _sm_state; }
            set
            {
                _sm_state = value;
                OnPropertyChanged("SmState");
            }
        }

        private TimeSpan recordCountdown;
        public TimeSpan RecordCountdown
        {
            get { return recordCountdown; }
            private set
            {
                recordCountdown = value;
                OnPropertyChanged("RecordCountdown");
            }
        }


        private Boolean isRecording = false;

        public Boolean IsRecording
        {
            get { return isRecording; }
            set
            {
                isRecording = value;
                OnPropertyChanged("IsRecording");
            }
        }


        private String units = "";

        public String Units
        {
            get { return units; }
            set
            {
                units = value;
                OnPropertyChanged("Units");
            }
        }


        public MotionController_VM()
        {
            DeviceName = "Motion controller";
            /*
            SiosDataView siosDataView = new SiosDataView();
            siosDataView.Show();

            siosDataView.DataContext = siosDataVM;

            siosDataVM.SiosData1.Add(0.6);
            siosDataVM.SiosData1.Add(0.8);            
            */

            //SiosData = new ObservableCollection<double>();

            //ModifiedCommands.ResetIsCenter();

            ResPress = 0;
        }

        public override async Task<bool> ConnectAsync()
        {
            if (IsConnected)
                return true;

            var to_ret = await base.ConnectAsync();
            if (to_ret)
            {
                try
                {
                    new Thread(FeedbackAndOutput) { IsBackground = true }.Start();
                }
                catch (Exception ex)
                {
                    SetNewStatusDispatcher(DeviceStateViewModel.enDeviceStates.Error,
                                            Properties.Resources.Error_during_get_FP, ex);
                    Dispose();
                }
            }
            return to_ret;
        }

        public Boolean IsInMotion()
        {
            Boolean up_on_pause = ModifiedCommands.GetIsUserProgramPaused();
            Boolean up_work = IsUserProgramWork();
            return (up_work && (!up_on_pause));
        }

        public Boolean IsRecordingNow()
        {
            ElmoCommandsEnum.ElmoCommands.enRecordStatus recordStatus = RecordStatus();
            return (recordStatus == ElmoCommandsEnum.ElmoCommands.enRecordStatus.StartedImmidiately);
        }

        
        public Boolean IsHoming()
        {
            try
            {
                Int32 status = GetStatus();
                Boolean up_work = DriveStatusParser.IsBitsIsSet(status, (int)DriveStatusParser.enstatusRegisters.HomingEvent);
                return up_work;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private double test_ampl = 0;

        private bool IsCheckStatusEnabled = true;

        int sin_idx = 1;

        int[] sinJpValArr = new int[1000];
        double[] sinTphValArr = new double[1000];

        double res_press_cnt = 0;
        double res_press_sum = 0;

        private void FeedbackAndOutput()
        {
            while (IsConnected)
            {
                if (IsCheckStatusEnabled)
                {
                    try
                    {
                        Int32 feedback_position = model.Commands.FeedbackPosition(1);
                        Int32 is_up_in_error = ModifiedCommands.GetUserProgramError();
                        Boolean up_on_pause = ModifiedCommands.GetIsUserProgramPaused();
                        Boolean can_measure = IsReadyForMeasurment();
                        Boolean up_work = IsUserProgramWork();
                        //Boolean is_rec = IsRecordingNow();
                        Double freq = ModifiedCommands.GetCurrentFrequency();
                        int amp = ModifiedCommands.GetCurrentAmplitude();
                        Boolean is_center = ModifiedCommands.GetIsCenter();
                        Boolean is_steady = ModifiedCommands.GetIsSteady();
                        int answ = ModifiedCommands.GetAnswer();
                        Double res_press = model.Commands.AnalogInput();

                        res_press_sum += res_press;
                        res_press_cnt++;

                        Boolean is_sin_val_ready = ModifiedCommands.GetIsSinValReady();

                        int smState = ModifiedCommands.GetSmState();

                        //Int32 output_ports = driveModel.Commands.GetOutputPorts();
                        CDispatcher.BeginInvoke(
                            (Action)(() =>
                            {
                                FeedbackPosition = feedback_position;
                                IsUserProgramInError = (is_up_in_error != 0);
                                IsPlaying = (up_work && (!up_on_pause));
                                IsCanMeasure = can_measure;
                                //IsRecording = is_rec;
                                CurrentFrequency_Hz = freq;
                                CurrentAmplitude_m = amp / model.CountsPerUnit;

                                if (res_press_cnt >= 100)
                                {
                                    res_press_cnt = 0;

                                    ResPress = res_press_sum / 100;

                                    res_press_sum = 0;
                                }

                                IsCenter = is_center;// is_center;
                                                     // 
                                IsSteady = is_steady;// true;// is_steady;

                                if(IsSteady && IsMoving)
                                {
                                    IsCheckStatusEnabled = false;
                                }

                                Answer = answ;

                                Status = GetStatus();

                                SmState = smState;
                                /*
                                if(IsCenter)
                                {
                                    CurrentFrequency_Hz = 1000;
                                }
                                */

                                /*
                                test_ampl += 0.01;

                                if(test_ampl > 10)
                                {
                                    test_ampl = 0;
                                }

                                CurrentAmplitude_m = test_ampl;*/
                                // OutputPorts = output_ports;

                                //siosDataVM.SiosData1.Add(test_ampl);
                                //SiosData.Add(test_ampl);
                                if(is_sin_val_ready)
                                {
                                    if (sin_idx < 900)
                                    {
                                        ModifiedCommands.SetSinValIndex(sin_idx);

                                        Thread.Sleep(50);

                                        //if (ModifiedCommands.GetSinValIndex() == sin_idx)
                                        {
                                            sinJpValArr[sin_idx] = ModifiedCommands.GetSinJpVal();
                                            sinTphValArr[sin_idx] = ModifiedCommands.GetSinTphVal();

                                            sin_idx++;

                                            if (sin_idx >= 900)
                                            {
                                                String SiosRawDataFilePath;

                                                string RepCaption;

                                                RepCaption = String.Format("sin_sw_{0}_F{1}_X{2}.csv", DateTime.Now.ToString("ddMMyyyy_HHmmss"), CurrentFrequency_Hz, CurrentAmplitude_m);

                                                //SiosRawDataFilePath = "./sin_sw.csv";

                                                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                                                {
                                                    Delimiter = ";",
                                                };

                                                using (var stream = File.Open(RepCaption, FileMode.Append))
                                                using (var writer = new StreamWriter(stream))
                                                using (var csv = new CsvWriter(writer, config))
                                                {
                                                    /*
                                                    csv.WriteRecords(arr);
                                                    */
                                                    for (int i = 0; i < 900; i++)
                                                    {
                                                        csv.WriteField(sinJpValArr[i]);
                                                        csv.WriteField(sinTphValArr[i]);
                                                        csv.NextRecord();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                            }));
                        
                        Thread.Sleep(model.UpdateTime_ms);
                    }
                    catch (Exception ex)
                    {
                        SetNewStatusDispatcher(DeviceStateViewModel.enDeviceStates.Error,
                            Properties.Resources.Error_during_get_FP, ex);
                        Dispose();
                    }

                }
                else
                {
                    if(!IsMoving)
                    {
                        IsCheckStatusEnabled = true;

                        sin_idx = 1;

                        model.UpdateTime_ms = 100;
                    }
                }// if (IsCheckStatusEnabled)
            }
        }


        #region OscillationPart
        public void SetOscillationParameters(double frequency, double amplitude_m)
        {
            try
            {
                ModifiedCommands.SetCenterofOscillations(model.CenterOfOscillations);

                ModifiedCommands.SetFrequency(frequency);
                SetFrequencyHz = frequency;

                int amplitude_cnt = (int)(model.CountsPerUnit * amplitude_m);
                ModifiedCommands.SetOscillationAmplitude(amplitude_cnt);
                SetDisplacement_m = amplitude_m;
                ModifiedCommands.SetUpdateTrigger();

            }
            catch (Exception ex)
            {
                SetNewStatus(DeviceStateViewModel.enDeviceStates.Error, Properties.Resources.Unable_to_set_oscill_params, ex);
                throw new Exception(Properties.Resources.Unable_to_set_oscill_params, ex);
            }
        }

        public void UpdateOscillation()
        {
            try
            {
                ModifiedCommands.SetUpdateTrigger();
            }
            catch (Exception ex)
            {
                SetNewStatus(DeviceStateViewModel.enDeviceStates.Error, Properties.Resources.Unable_to_set_updateTrigger, ex);
                throw new Exception(Properties.Resources.Unable_to_set_updateTrigger, ex);
            }
        }

        public void SetCenterOfOscillation(int center_cnt)
        {
            try
            {
                ModifiedCommands.SetCenterofOscillations(center_cnt);
            }
            catch (Exception ex)
            {
                SetNewStatus(DeviceStateViewModel.enDeviceStates.Error, Properties.Resources.Unable_to_set_center, ex);
                throw new Exception(Properties.Resources.Unable_to_set_center, ex);
            }
        }

        public void StopOscillation()
        {
            try
            {
                ModifiedCommands.StopOscillations();
                //ModifiedCommands.SetUpdateTrigger();
            }
            catch (Exception ex)
            {
                SetNewStatus(DeviceStateViewModel.enDeviceStates.Error, Properties.Resources.Unable_to_pause_oscilation, ex);
                throw new Exception(Properties.Resources.Unable_to_pause_oscilation, ex);
            }
        }

        public void StartOscillation(double weight, double kp1_start, double ki1_start, double kp2_start, double ki2_start, 
                double kp1_steady, double ki1_steady, double kp2_steady, double ki2_steady, double kp3_start, double kp3_steady)
        {
            double uf6;
            double uf7;
            double uf8;
            double uf9;
            double uf10;
            double uf11;
            double uf12;
            double uf13;
            double uf14;
            double uf15;
            double uf16;
            double uf17;
            double uf18;

            try
            {
                /*
                if(weight == 0)
                {
                    uf6 = 40;           // KP[1]
                    uf7 = 130;          // KI[1]
                    uf8 = 0.0000003;    // KP[2]
                    uf9 = 1;            // KI[2]

                    // Steady
                    uf10 = 40;          // KP[1]
                    uf11 = 120;         // KI[1]
                    uf12 = 0.0000020;   // KP[2]    // 0.0000003 // 0.0000026        
                    uf13 = 4;           // KI[2]    // 1    // 90

                    // Start
                    uf14 = 40;          // KP[1]
                    uf15 = 120;         // KI[1]
                    uf16 = 0.0000003;   // KP[2]    // 0.0000003 // 0.0000026       
                    uf17 = 1;           // KI[2]    // 10            

                    uf18 = 10;          // KP[3]    
                }
                else
                {
                    uf6 = 40;           // KP[1]
                    uf7 = 120;          // KI[1]
                    uf8 = 0.0000010;    // KP[2]
                    uf9 = 1;            // KI[2]

                    // Steady
                    uf10 = 40;          // KP[1]
                    uf11 = 120;         // KI[1]
                    uf12 = 0.0000010;   // KP[2]
                    uf13 = 4;  // 10    // KI[2]

                    // Start
                    uf14 = 40;          // KP[1]
                    uf15 = 120;         // KI[1]
                    uf16 = 0.0000010;   // KP[2]
                    uf17 = 2;           // KI[2]

                    uf18 = 10;          // KP[3]    
                }                
                
                ModifiedCommands.SetUF6(uf6);
                ModifiedCommands.SetUF7(uf7);
                ModifiedCommands.SetUF8(uf8);
                ModifiedCommands.SetUF9(uf9);
                ModifiedCommands.SetUF10(uf10);
                ModifiedCommands.SetUF11(uf11);
                ModifiedCommands.SetUF12(uf12);
                ModifiedCommands.SetUF13(uf13);
                ModifiedCommands.SetUF14(uf14);
                ModifiedCommands.SetUF15(uf15);
                ModifiedCommands.SetUF16(uf16);
                ModifiedCommands.SetUF17(uf17);
                ModifiedCommands.SetUF18(uf18);
                */

                uf6 = 40;           // KP[1]
                uf7 = 130;          // KI[1]
                uf8 = 0.0000003;    // KP[2]
                uf9 = 1;            // KI[2]

                //uf18 = 180;          // KP[3]    

                ModifiedCommands.SetUF6(uf6);
                ModifiedCommands.SetUF7(uf7);
                ModifiedCommands.SetUF8(uf8);
                ModifiedCommands.SetUF9(uf9);
                ModifiedCommands.SetUF10(kp1_steady);
                ModifiedCommands.SetUF11(ki1_steady);
                ModifiedCommands.SetUF12(kp2_steady);
                ModifiedCommands.SetUF13(ki2_steady);
                ModifiedCommands.SetUF14(kp1_start);
                ModifiedCommands.SetUF15(ki1_start);
                ModifiedCommands.SetUF16(kp2_start);
                ModifiedCommands.SetUF17(ki2_start);
                ModifiedCommands.SetUF18(kp3_steady);

                ModifiedCommands.StartOscillations();
                //ModifiedCommands.SetUpdateTrigger();
            }
            catch (Exception ex)
            {
                SetNewStatus(DeviceStateViewModel.enDeviceStates.Error, Properties.Resources.Unable_to_resume_oscilation, ex);
                throw new Exception(Properties.Resources.Unable_to_resume_oscilation, ex);
            }
        }

        public void PauseOscillation()
        {
            try
            {
                ModifiedCommands.PauseOscillations();
                ModifiedCommands.SetUpdateTrigger();
            }
            catch (Exception ex)
            {
                SetNewStatus(DeviceStateViewModel.enDeviceStates.Error, Properties.Resources.Unable_to_pause_oscilation, ex);
                throw new Exception(Properties.Resources.Unable_to_pause_oscilation, ex);
            }
        }

        public void ResumeOscillation()
        {
            try
            {
                ModifiedCommands.ResumeOscillations();
                ModifiedCommands.SetUpdateTrigger();
            }
            catch (Exception ex)
            {
                SetNewStatus(DeviceStateViewModel.enDeviceStates.Error, Properties.Resources.Unable_to_resume_oscilation, ex);
                throw new Exception(Properties.Resources.Unable_to_resume_oscilation, ex);
            }
        }

        public Boolean IsReadyForMeasurment()
        {
            try
            {
                return (ModifiedCommands.GetIsReadyForMeasurements() != 0);
            }
            catch (Exception ex)
            {
                SetNewStatus(DeviceStateViewModel.enDeviceStates.Error, Properties.Resources.Unable_to_get_is_ready_for_measure, ex);
                throw new Exception(Properties.Resources.Unable_to_get_is_ready_for_measure, ex);
            }
        }

        public void ResetIsCenter()
        {
            ModifiedCommands.ResetIsCenter();
        }

        public void StartOscaillationProgram()
        {
            try
            {         
                //ModifiedCommands.KillUserProgram();
                ModifiedCommands.ResumeOscillations();
                ModifiedCommands.SetUpdateTrigger();
                ModifiedCommands.ExecuteUserProgram(model.UserProgramName, null);
            }
            catch (Exception ex)
            {
                SetNewStatus(DeviceStateViewModel.enDeviceStates.Error, Properties.Resources.Unable_to_start_up, ex);
                throw new Exception(Properties.Resources.Unable_to_start_up, ex);
            }
        }

        public void StartUserProgram()
        {
            try
            {
                ModifiedCommands.KillUserProgram();
                ModifiedCommands.ResumeOscillations();
                //ModifiedCommands.SetUpdateTrigger();
                ModifiedCommands.ExecuteUserProgram(model.UserProgramName, null);
            }
            catch (Exception ex)
            {
                SetNewStatus(DeviceStateViewModel.enDeviceStates.Error, Properties.Resources.Unable_to_start_up, ex);
                throw new Exception(Properties.Resources.Unable_to_start_up, ex);
            }
        }

        public void StopOscillationProgram()
        {
            try
            {
                //ModifiedCommands.ResetIsReadyForMeasurements();
                //ModifiedCommands.ResumeOscillations();
                ModifiedCommands.PauseOscillations();
                ModifiedCommands.SetUpdateTrigger();
                //ModifiedCommands.KillUserProgram();
            }
            catch (Exception ex)
            {
                SetNewStatus(DeviceStateViewModel.enDeviceStates.Error, Properties.Resources.Unable_to_stop_up, ex);
                throw new Exception(Properties.Resources.Unable_to_stop_up, ex);
            }
        }

        public void KillUserProgram()
        {
            if (ModifiedCommands != null)
            {
                ModifiedCommands.PauseOscillations();
                ModifiedCommands.SetUpdateTrigger();
                ModifiedCommands.KillUserProgram();
            }
        }


        public Int32 GetStatus()
        {
            try
            {
                return ModifiedCommands.StatusRegister();
            }
            catch (Exception ex)
            {
                SetNewStatus(DeviceStateViewModel.enDeviceStates.Error, Properties.Resources.Unable_to_get_status, ex);
                throw new Exception(Properties.Resources.Unable_to_get_status, ex);
            }
        }       

        public Boolean IsOscillationPaused()
        {
            try
            {
                return ModifiedCommands.GetIsUserProgramPaused();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Boolean IsUserProgramWork()
        {
            try
            {
                Int32 status = GetStatus();
                Boolean up_work = DriveStatusParser.IsBitsIsSet(status, (int)DriveStatusParser.enstatusRegisters.UserProgram);
                return up_work;
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        public RelayCommand ConnectCommand { get { return new RelayCommand(x => !IsConnected, x => ConnectAsync()); } }

        public RelayCommand DisconnectCommand { get { return new RelayCommand(x => IsConnected, x => Dispose()); } }

        #region RecorderPart
        protected void SetupRecording(double time, out int time_gap_us)
        {
            // double time = length_count / speed_count;

            Int32 samplingTime_usec = model.Commands.RecorderTimeQuanta();

            Int32 max_rec_l = model.Commands.GetMaxRecordLength();

            Double d_gap = (time * ElmoHandler.USEC_TO_SEC) / (max_rec_l * samplingTime_usec);

            Int32 int_gap = (int)Math.Ceiling(d_gap);
            Int32 record_length = (int)Math.Floor((time * ElmoHandler.USEC_TO_SEC) / (samplingTime_usec * int_gap));
            time_gap_us = int_gap * samplingTime_usec;

            RecordingSetup recordingSetup = model.Commands.SetupRecording(samplingTime_usec, int_gap, record_length,
                new int[] { (int)ElmoCommandsEnum.enSignalsIndex.Position });

            model.Commands.ConfigureRecorder(recordingSetup);
        }



        public ElmoCommandsEnum.ElmoCommands.enRecordStatus RecordStatus()
        {
            return model.Commands.RecordStatus();
        }


        public RecordingData GetRecordingData()
        {
            return model.Commands.GetRecorderedData();
        }

        public Boolean ConfigureRecorder(Double duration_s)
        {
            try
            {
                int time_gap_us;
                SetupRecording(duration_s, out time_gap_us);
                record_countdown_ms = duration_s * 1000;
                current_recorder_gap_s = time_gap_us / 1e6;
                SetNewStatus(DeviceStateViewModel.enDeviceStates.Ok, "Recorder succesfully configured!");
                return true;
            }
            catch (Exception ex)
            {
                SetNewStatus(DeviceStateViewModel.enDeviceStates.Error, "Can't configure recorder!", ex);
                current_recorder_gap_s = 0;
                return false;
            }
        }

        protected Boolean WaitRecorderedDataIsReady()
        {
            //try
            //{
            ElmoCommandsEnum.ElmoCommands.enRecordStatus rec_state = ElmoCommandsEnum.ElmoCommands.enRecordStatus.InvalidData;
            while (rec_state != ElmoCommandsEnum.ElmoCommands.enRecordStatus.Complete)
            {
                rec_state = RecordStatus();
                if (rec_state == ElmoCommandsEnum.ElmoCommands.enRecordStatus.InvalidData)
                {
                    //RecordCountdown = TimeSpan.FromMilliseconds(0);
                    return false;
                }

                record_countdown_ms -= RECORDWAIT_TIME_MS;
                if (record_countdown_ms < 0)
                    record_countdown_ms = 0;

                RecordCountdown = TimeSpan.FromMilliseconds(record_countdown_ms);

                Thread.Sleep(RECORDWAIT_TIME_MS);
            }
            RecordCountdown = TimeSpan.FromMilliseconds(0);
            return true;
            //}
            //catch (Exception)
            //{
            //    return false;
            //}
        }


        public async Task<DataPoint[]> DownloadRecorderedData()
        {
            var data_ready = await Task.Run<Boolean>((Func<Boolean>)WaitRecorderedDataIsReady);
            if (data_ready)
            {                
                RecordingData recordingData = await Task.Run<RecordingData>((Func<RecordingData>)GetRecordingData);
                Double[] data = recordingData.Data[0];// (int)ElmoCommandsEnum.enSignalsIndex.Position];
                DataPoint[] dataPoints = Instruments.DataConvertion.ConvertDoubleDataToOxyPoints(data, x => x * current_recorder_gap_s, y => y / model.CountsPerUnit);
                SetNewStatusDispatcher(DeviceStateViewModel.enDeviceStates.Ok, "Recordered data downloaded!");
                return dataPoints;
            }
            throw new Exception("Failed to download data from recorder.");
        }

        public Boolean StartRecorder()
        {
            try
            {
                ModifiedCommands.StartRecorder();
                return true;
            }
            catch (Exception ex)
            {
                SetNewStatusDispatcher(DeviceStateViewModel.enDeviceStates.Error, "Unable to start recorder!", ex);
                return false;
            }
        }

        public Boolean StopRecorder()
        {
            try
            {
                ModifiedCommands.StopRecorder();
                return true;
            }
            catch (Exception ex)
            {
                SetNewStatusDispatcher(DeviceStateViewModel.enDeviceStates.Error, "Unable to stop recorder!", ex);
                return false;
            }
        }
        #endregion        
        /*
        private ObservableCollection<double> _sios_data;
        public ObservableCollection<double> SiosData
        {
            get => _sios_data;
            set
            {
                _sios_data = value;
            }
        }   */      

        //SiosDataVM siosDataVM = new SiosDataVM();
    }
}
