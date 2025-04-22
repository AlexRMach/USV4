using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ElmoMotionControlComponents.Drive.EASComponents.Recording;
using ElmoMotionControlComponents.Drive.EASComponents.Personality;
using ElmoMotionControlComponents.Drive.EASComponents;



namespace ush4.Models.ELMO
{
    public class ElmoHandler
    {
        IDriveCommunication driveCommunication;
        const Byte ATTEMPTS = 3;
        public const Double USEC_TO_SEC = 1e6;
        //public const int MAX_RECORD_LENGTH = 4096;

        Object _lock = new Object();
        private int timeout = 1000;

        public IDriveRecording Recording { get; private set; }

       

        public ElmoHandler(IDriveCommunication adapter)
        {
            driveCommunication = adapter;
            Recording = driveCommunication.GetRecordingObject();
        }

        


        //public event Action<object, EventArgs> CommunicationIsReady
        //{
        //    add { _adapter.PersonalityIsUpload += value; }
        //    remove { _adapter.PersonalityIsUpload -= value; }
        //}


        public void MotorOn()
        {
            String motor_off = ElmoCommandsEnum.ElmoCommands.SetDataRequest(ElmoCommandsEnum.ElmoCommands.MotorOn, (int)ElmoCommandsEnum.enOnOff.On);
            String rep;
            
            ResendInErrorCase(motor_off, out rep);
        }

        public void MotorOff()
        {
            String motor_on = ElmoCommandsEnum.ElmoCommands.SetDataRequest(ElmoCommandsEnum.ElmoCommands.MotorOn, (int)ElmoCommandsEnum.enOnOff.Off);
            String rep;
           
            ResendInErrorCase(motor_on, out rep);
        }


        public void ToRelativePosition(Int32 pos)
        {
            String pr = ElmoCommandsEnum.ElmoArrayCommands.SetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.PositionRelative, 1, pos);
            String rep;

            ResendInErrorCase(pr, out rep);
        }

        public Int32 FeedbackPosition(int socetNum)
        {
            String bm = ElmoCommandsEnum.ElmoArrayCommands.GetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.FeedbackPosition, socetNum);
            String rep;

            ResendInErrorCase(bm, out rep);
            return Convert.ToInt32(rep);
        }


        public Int32 GetOutputPorts()
        {
            String output_port = ElmoCommandsEnum.ElmoCommands.GetDataRequest(ElmoCommandsEnum.ElmoCommands.OutputPort);
            String rep;

            ResendInErrorCase(output_port, out rep);
            return Convert.ToInt32(rep);
        }

        public void KillUserProgram()
        {
            String kl = ElmoCommandsEnum.ElmoCommands.GetDataRequest(ElmoCommandsEnum.ElmoCommands.KillUserProgram);
            String rep;

            ResendInErrorCase(kl, out rep);
        }


        public void BeginMotion()
        {
            String bm = ElmoCommandsEnum.ElmoArrayCommands.GetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.BeginMotion, 
                (int)ElmoCommandsEnum.enOnOff.On);
            String rep;

            ResendInErrorCase(bm, out rep);
        }

        #region SCF
        //public void Reset_ToSocet4()
        //{
        //    String sts4 = ElmoCommandsEnum.ElmoArrayCommands.SetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.CommutationArray, (int)ElmoCommandsEnum.enOnOff.Socet4_in_CA, (int)ElmoCommandsEnum.enOnOff.Off);
        //    String rep;

        //    ResendInErrorCase(sts4, out rep);
        //}

        //public void SinToSocet4()
        //{
        //    String sts4 = ElmoCommandsEnum.ElmoArrayCommands.SetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.CommutationArray, (int)ElmoCommandsEnum.enOnOff.Socet4_in_CA, (int)ElmoCommandsEnum.enOnOff.VirtualSin);
        //    String rep;

        //    ResendInErrorCase(sts4, out rep);
        //}

        //public void BiSSToSocet4()
        //{
        //    String bts4 = ElmoCommandsEnum.ElmoArrayCommands.SetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.CommutationArray, (int)ElmoCommandsEnum.enOnOff.Socet4_in_CA, (int)ElmoCommandsEnum.enOnOff.BiSS);
        //    String rep;

        //    ResendInErrorCase(bts4, out rep);
        //}

        //public void Socet4toPosition()
        //{
        //    String s4tp = ElmoCommandsEnum.ElmoArrayCommands.SetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.CommutationArray, (int)ElmoCommandsEnum.enOnOff.SocetPosition_in_CA, (int)ElmoCommandsEnum.enOnOff.Socet4);
        //    String rep;

        //    ResendInErrorCase(s4tp, out rep);
        //}

        //public void Socket4toVelocity()
        //{
        //    String s4tp = ElmoCommandsEnum.ElmoArrayCommands.SetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.CommutationArray, (int)ElmoCommandsEnum.enOnOff.SocetVelocity_in_CA, (int)ElmoCommandsEnum.enOnOff.Socet4);
        //    String rep;

        //    ResendInErrorCase(s4tp, out rep);
        //}


        //public void Socket4toCurrent()
        //{
        //    String s4tp = ElmoCommandsEnum.ElmoArrayCommands.SetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.CommutationArray, (int)ElmoCommandsEnum.enOnOff.SocetCurrent_in_CA, (int)ElmoCommandsEnum.enOnOff.Socet4);
        //    String rep;

        //    ResendInErrorCase(s4tp, out rep);
        //} 
        #endregion

        public Int32 StatusRegister()
        {
            String status_register = ElmoCommandsEnum.ElmoCommands.GetDataRequest(ElmoCommandsEnum.ElmoCommands.StatusRegister);
            String rep;
            
            ResendInErrorCase(status_register, out rep);
            return Convert.ToInt32(rep);
        }

        public Int32 MotorFault()
        {
            String motor_fault = ElmoCommandsEnum.ElmoCommands.GetDataRequest(ElmoCommandsEnum.ElmoCommands.MotorFault);
            String rep;
           
            ResendInErrorCase(motor_fault, out rep);
            return Convert.ToInt32(rep);
        }


        public Int32 EncoderFaultExtended()
        {
            String encoder_state = ElmoCommandsEnum.ElmoArrayCommands.GetDataRequest(
                ElmoCommandsEnum.ElmoArrayCommands.ExtendedError, (Int32)ElmoCommandsEnum.ElmoArrayCommands.enExtendetErrorIndexes.FeedbackError);
            String rep;
            
            ResendInErrorCase(encoder_state, out rep);
            return Convert.ToInt32(rep);
        }


        public Int32 MotorFaultExtended()
        {
            String encoder_state = ElmoCommandsEnum.ElmoArrayCommands.GetDataRequest(
                ElmoCommandsEnum.ElmoArrayCommands.ExtendedError, 
                (Int32)ElmoCommandsEnum.ElmoArrayCommands.enExtendetErrorIndexes.MotionOnError);
            String rep;
            
            ResendInErrorCase(encoder_state, out rep);

            return Convert.ToInt32(rep);
        }


        public void ArmHoming()
        {
            String activateHoming = ElmoCommandsEnum.ElmoArrayCommands.SetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.Homming,
                (int)ElmoCommandsEnum.ElmoArrayCommands.enHomingIndexes.ActivationMode, (int)ElmoCommandsEnum.enOnOff.On);

            String rep;

            ResendInErrorCase(activateHoming, out rep);
        }

        public void DisarmHoming()
        {
            String diactivateHoming = ElmoCommandsEnum.ElmoArrayCommands.SetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.Homming,
                (int)ElmoCommandsEnum.ElmoArrayCommands.enHomingIndexes.ActivationMode, (int)ElmoCommandsEnum.enOnOff.Off);

            String rep;
            ResendInErrorCase(diactivateHoming, out rep);
        }

        public int HomingArmState()
        {
            String homingState = ElmoCommandsEnum.ElmoArrayCommands.GetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.Homming,
                (int)ElmoCommandsEnum.ElmoArrayCommands.enHomingIndexes.ActivationMode);

            String rep;

            ResendInErrorCase(homingState, out rep);

            return Convert.ToInt32(rep);
        }

        #region SCF
        //public void SineGeneratorOn()
        //{
        //    String _sineGeneratorOn = ElmoCommandsEnum.ElmoArrayCommands.SetDataRequest(
        //        ElmoCommandsEnum.ElmoArrayCommands.TW, (int)ElmoCommandsEnum.enOnOff.SinGenerator_in_TW, (int)ElmoCommandsEnum.enOnOff.On);
        //    String rep;

        //    ResendInErrorCase(_sineGeneratorOn, out rep);
        //}

        //public void SineGeneratorOff()
        //{
        //    String _sineGeneratorOff = ElmoCommandsEnum.ElmoArrayCommands.SetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.TW, (int)ElmoCommandsEnum.enOnOff.SinGenerator_in_TW, (int)ElmoCommandsEnum.enOnOff.Off);
        //    String rep;

        //    ResendInErrorCase(_sineGeneratorOff, out rep);

        //}

        //public void SineMotionUnits(ElmoCommandsEnum.ElmoArrayCommands.enSineExcitationUnits units)
        //{
        //    String _sineUnits = ElmoCommandsEnum.ElmoArrayCommands.SetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.SineExcitation, (Int32)ElmoCommandsEnum.ElmoArrayCommands.enSineExcitationIndexes.Units, (Int32)units);
        //    String rep;

        //    ResendInErrorCase(_sineUnits, out rep);
        //}

        //public void SineMotionAmplitude(Int32 value)
        //{
        //    String _sineAmplitude = ElmoCommandsEnum.ElmoArrayCommands.SetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.SineExcitation, (Int32)ElmoCommandsEnum.ElmoArrayCommands.enSineExcitationIndexes.Amplitude, value);
        //    String rep;

        //    ResendInErrorCase(_sineAmplitude, out rep);
        //}

        //public void SineMotionFrequency(Double freq)
        //{
        //    String _sineFrequency = ElmoCommandsEnum.ElmoArrayCommands.SetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.SineExcitation, (Int32)ElmoCommandsEnum.ElmoArrayCommands.enSineExcitationIndexes.Frequency, freq);
        //    String rep;

        //    ResendInErrorCase(_sineFrequency, out rep);
        //}


        //public Double GetSineMotionAmplitude()
        //{
        //    String _sineAmplitude = ElmoCommandsEnum.ElmoArrayCommands.GetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.SineExcitation, (Int32)ElmoCommandsEnum.ElmoArrayCommands.enSineExcitationIndexes.Amplitude);
        //    String rep;

        //    ResendInErrorCase(_sineAmplitude, out rep);

        //    return Convert.ToDouble(rep);
        //} 
        #endregion


        public Int32 RecorderTimeQuanta()
        {
            String _recorder_quanta = ElmoCommandsEnum.ElmoArrayCommands.GetDataRequest(
                ElmoCommandsEnum.ElmoArrayCommands.MiscellaneousReports, (int)ElmoCommandsEnum.enService.TimeQuanta);//Commands.ElmoCommands.GetDataRequest(Commands.ElmoCommands.RecorderTimeQuanta);
            String rep;

            ResendInErrorCase(_recorder_quanta, out rep);

            return Convert.ToInt32(rep);
        }

        public Double AnalogInput()
        {
            String bm = ElmoCommandsEnum.ElmoArrayCommands.GetDataRequest(ElmoCommandsEnum.ElmoCommands.AnalogInput, 0);
            String rep;

            ResendInErrorCase(bm, out rep);
            return Convert.ToDouble(rep);
        }


        public void ExecuteUserProgram(String programName, string[] args = null)
        { 
            String data_args = "";
            if ((args != null) && (args.Length != 0))
            { 
                data_args = args[0];
                for (int i = 1; i < args.Length; i++)
                {
                    data_args += ", ";
                    data_args += args[i];
                }
            }

            String _xq_command = ElmoCommandsEnum.ElmoCommands.UserProgramExecuteRequest(ElmoCommandsEnum.ElmoCommands.ExecuteProgram, 
                programName, data_args);
            String rep;

            ResendInErrorCase(_xq_command, out rep);
        }   

        public void SetUnitMode(ElmoCommandsEnum.ElmoCommands.enUnitModes enUM)
        {
            String _unit_mode = ElmoCommandsEnum.ElmoCommands.SetDataRequest(ElmoCommandsEnum.ElmoCommands.UnitMode, (Int32)enUM);
            String rep;

            ResendInErrorCase(_unit_mode, out rep);
        }


       

        public RecordingSetup SetupRecording(Int32 samplingTime, Int32 gap, Int32 recordLength, Int32[] signalIndexs)
        {
            RecordingSetup setup = new RecordingSetup();

            setup.SamplingTime = samplingTime;
            setup.TimeResolution = gap;
            setup.RecordingLength = recordLength;

            TriggerSetup triggerSetup = new TriggerSetup();
            triggerSetup.TriggerMode = TriggerMode.Manual;
            triggerSetup.SetupType = TriggerSetupType.Immediate;

            setup.TriggerSetup = triggerSetup;

            RecordingSignalSetup signalSetup;
            for (int i = 0; i < signalIndexs.Length; i++)
            { 
                signalSetup = driveCommunication.PersonalityModel.SignalsMetaData[signalIndexs[i]];
                setup.SignalData.Add(signalSetup);
            }

            
            return setup;
        }

        //public RecordingSetup SetupRecordingWithTrigger(Int32 samplingTime, Int32 gap, Int32 recordLength, Int32[] signalIndexs)
        //{
        //    RecordingSetup setup = new RecordingSetup();

        //    setup.SamplingTime = samplingTime;
        //    setup.TimeResolution = gap;
        //    setup.RecordingLength = recordLength;

        //    TriggerSetup triggerSetup = new TriggerSetup();
        //    triggerSetup.TriggerMode = TriggerMode.Normal;
        //    triggerSetup.SetupType = TriggerSetupType.Digital;
        //    triggerSetup.SlopeType = TriggerSlope.Positive;
        //    triggerSetup.High = 1;

        //    setup.TriggerSetup = triggerSetup;

        //    RecordingSignalSetup signalSetup;
        //    for (int i = 0; i < signalIndexs.Length; i++)
        //    {
        //        signalSetup = driveCommunication.PersonalityModel.SignalsMetaData[signalIndexs[i]];
        //        setup.SignalData.Add(signalSetup);
        //    }

        //    Recording = driveCommunication.GetRecordingObject();
        //    return setup;
        //}


        public RecordingSetup SetupRecordingWithTrigger(TriggerSetup triggerSetup, Int32 samplingTime, Int32 gap, Int32 recordLength, Int32[] signalIndexs)
        {
            RecordingSetup setup = new RecordingSetup();

            setup.SamplingTime = samplingTime;
            setup.TimeResolution = gap;
            setup.RecordingLength = recordLength;

          

            setup.TriggerSetup = triggerSetup;

            RecordingSignalSetup signalSetup;
            for (int i = 0; i < signalIndexs.Length; i++)
            {
                signalSetup = driveCommunication.PersonalityModel.SignalsMetaData[signalIndexs[i]];
                setup.SignalData.Add(signalSetup);
            }

            //Recording = driveCommunication.GetRecordingObject();
            return setup;
        }


        public int GetMaxRecordLength()
        {
            String max_rec_l = ElmoCommandsEnum.ElmoArrayCommands.GetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.WizardInt, 
                (int)ElmoCommandsEnum.ElmoArrayCommands.enWizardIntIndexes.MaxRecordLength);

            String rep;
            ResendInErrorCase(max_rec_l, out rep);
            return Convert.ToInt32(rep);
        }

        public int GetActualRecordLength()
        {
            String act_rec_l = ElmoCommandsEnum.ElmoArrayCommands.GetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.WizardInt,
                (int)ElmoCommandsEnum.ElmoArrayCommands.enWizardIntIndexes.ActualRecordength);

            String rep;
            ResendInErrorCase(act_rec_l, out rep);
            return Convert.ToInt32(rep);
        }


        //public int GetRecordLength()
        //{
        //    String act_rec_l = ElmoCommandsEnum.ElmoCommands.GetDataRequest(ElmoCommandsEnum.ElmoCommands.RecordLength);

        //    String rep;
        //    ResendInErrorCase(act_rec_l, out rep);
        //    return Convert.ToInt32(rep);
        //}

        public void ConfigureRecorder(RecordingSetup recorderSetup)
        {
            ResendInErrorCase(() => Recording.ConfigureRecording(recorderSetup));           
        }


        public void StartRecorder()
        {
            ResendInErrorCase(() => Recording.StartRecording());   
        }

        public ElmoCommandsEnum.ElmoCommands.enRecordStatus RecordStatus()
        {
            String _sampleTime = ElmoCommandsEnum.ElmoCommands.GetDataRequest(ElmoCommandsEnum.ElmoCommands.ActivateRecorder_Status);
            String rep;

            ResendInErrorCase(_sampleTime, out rep);

            return (ElmoCommandsEnum.ElmoCommands.enRecordStatus)Enum.Parse(typeof(ElmoCommandsEnum.ElmoCommands.enRecordStatus),rep);
        }

        public void StopRecorder()
        {
            ResendInErrorCase(() => Recording.StopRecorder());
            Recording = driveCommunication.GetRecordingObject();
        }

        public RecordingData GetRecorderedData()
        {
            lock (_lock)
            {
                return Recording.UploadRecordingData();
            }
        }


        protected virtual void ResendInErrorCase(String cmd, out String rep)
        {
            int i = ATTEMPTS;
            Exception ex_l = new Exception();
            while (i > 0)
            {
                try
                {
                    lock (_lock)
                    {
                        SendCommandToDrive(cmd, out rep);
                    }
                    return;
                }
                catch (Exception ex)
                {

                    ex_l = ex;
                    i--;
                }
            }
            throw ex_l;
        }

        protected virtual void ResendInErrorCase(Action action)
        {
            int i = ATTEMPTS;
            Exception ex_l = new Exception();
            while (i > 0)
            {
                try
                {
                    lock (_lock)
                    {
                        action();
                    }
                    return;
                }
                catch (Exception ex)
                {

                    ex_l = ex;
                    //LoggerMessenger.Error(ex_message);
                    i--;
                }
            }
            throw ex_l;
        }

        protected void SendCommandToDrive(String command, out string reply)
        {
            IDriveErrorObject err;

            if (!driveCommunication.SendCommandAnalyzeError(command, out reply, out err, timeout))
            {
                if (err != null)
                    throw new Exception(Properties.ResourcesE.ErrorWhileWrite + ElmoCommandsEnum.DriveErrorObjectToString(err));
                else
                    throw new Exception(Properties.ResourcesE.ErrorWhileWrite);
            }

        }

    }
}
