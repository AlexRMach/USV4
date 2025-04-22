using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using ush4.ViewModels.Base;
using ush4.Infrastructure.Commands;
using OxyPlot.Series;
using OxyPlot;
using ush4.Models;
using ush4.Models.SIOS;
using System.Threading;
using OxyPlot.Axes;
//using Common.WPF.ViewModels;
using ush4.ViewModels.Recorder;
//using Newtonsoft.Json;
using System.Windows.Controls;
using ush4.ViewModels.SetPoint;
using ush4.ViewModels.Results;
using System.IO;
using ush4.Instruments;
using System.Collections.ObjectModel;
using HDF5DotNet;
using System.Windows.Markup;
using MathNet.Numerics;
using ush4.Views.Windows;
using ElmoMotionControlComponents.Drive.EASComponents.Recording;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;
using OxyPlot.Annotations;
using System.Runtime.InteropServices.ComTypes;
using ush4.Infrastructure;
using ush4.ViewModels.Disp;
using System.Windows.Media.Media3D;

namespace ush4.ViewModels
{
    public class USH_VM : DeviceWithStatusesViewModel<Models.USH_model>
    {
        public enum enDevStates
        {
            InitState,
            ElmoConnState,
            SioConnState,
            PlatfInitState,
            EnterParsState,
            IdleState,
            PlayState,
            RegistrState
        }

        #region Команды

        #region CloseApplicationCommand

        public ICommand CloseApplicationCommand { get; }

        private bool CanCloseApplicationCommandExecute(object p) => true;

        private void OnCloseApplicationCommandExecuted(object p)
        {
            MotionController.KillUserProgram();
            Application.Current.Shutdown();
        }


        #endregion

        #endregion

        public enum enStates
        {
            DoNothing,
            Wait,
            Measure,
            Oscillate,
            Paused,
        }

        private enStates work_state = enStates.DoNothing;

        public enStates WorkState
        {
            get { return work_state; }
            set
            {
                work_state = value;
                OnPropertyChanged("WorkState");
            }
        }

        private ResultValues_VM result_values = new ResultValues_VM();

        public ResultValues_VM ResultValues
        {
            get { return result_values; }
            set
            {
                result_values = value;
                OnPropertyChanged("ResultValues");
            }
        }

        private MotionController_VM motionController = new MotionController_VM();
        public MotionController_VM MotionController
        {
            get { return motionController; }
            set
            {
                motionController = value;
                OnPropertyChanged("MotionController");
            }
        }

        private RemoteControlVM remoteControl = new RemoteControlVM();

        public RemoteControlVM RemoteControl
        {
            get { return remoteControl; }
            set
            {
                remoteControl = value;
                OnPropertyChanged("RemoteControl");
            }
        }

        private IRecorderVM recorderVM;

        public IRecorderVM RecorderController
        {
            get { return recorderVM; }
            set
            {
                recorderVM = value;
                OnPropertyChanged("RecorderController");
            }
        }                       

        private PlayingParam_VM _play_param;// = new PlayingParam_VM();

        public PlayingParam_VM PlayParam
        {
            get { return _play_param; }
            set
            {
                _play_param = value;
                OnPropertyChanged("PlayParam");
            }
        }

        private String main_win_name;

        public String MainWinName
        {
            get { return main_win_name; }
            set
            {
                main_win_name = value;
                OnPropertyChanged("MainWinName");
            }
        }

        private int _selected_page;
        public int SelectedPage
        {
            get { return _selected_page; }
            set
            {
                _selected_page = value;
                OnPropertyChanged("SelectedPage");
            }
        }

        private bool _IsPlayTabSelected = false;

        public bool IsPlayTabSelected
        {
            get { return _IsPlayTabSelected; }
            set
            {
                if (value != _IsPlayTabSelected)
                {
                    //SetPlayPlot();

                    _IsPlayTabSelected = value;
                    OnPropertyChanged("IsPlayTabSelected ");
                    if (_IsPlayTabSelected && !IsSiosProc)
                    {
                        SetRawPlotForPlayAxes();
                    }
                }
            }
        }

        private bool _IsRegTabSelected = false;

        public bool IsRegTabSelected
        {
            get { return _IsRegTabSelected; }
            set
            {
                if (value != _IsRegTabSelected)
                {
                    //SetPlayPlot();

                    _IsRegTabSelected = value;
                    OnPropertyChanged("IsRegTabSelected ");
                    if (_IsRegTabSelected && !isRegInabled)
                    {
                        SetRawPlotForRegAxes();
                    }
                }
            }
        }

        private double _load_weight;

        public double LoadWeight
        {
            get { return _load_weight; }
            set
            {
                _load_weight = value;
                OnPropertyChanged("LoadWeight");
            }
        }

        /*
        private double _res_press;
        public double ResPress
        {
            get { return _res_press; }
            set
            {
                _res_press = value;
                OnPropertyChanged("ResPress");
            }
        }
        */

        private bool _sios_used;
        public bool SiosUsed
        {
            get { return _sios_used; }
            set
            {
                _sios_used = value;
                OnPropertyChanged("SiosUsed");
            }
        }        

        // SIOS part
        public PlotModel SiosRawPlotModel { get; private set; }
        public PlotModel SiosFFTPlotModel { get; private set; }
        public PlotModel SiosRegRawPlotModel { get; private set; }

        SIOSManagerModel _siosManagerModel = new SIOSManagerModel();

        private bool IsSiosProc = false;

        public double FreqValue;

        public double DisplcValue;

        public double VelValue;

        public double AccelValue;

        static int periodSios;

        int numOfPeriodsForPlot = 4;

        int numOfPeriodsForFft = 10;

        public void Close()
        {
            MotionController.KillUserProgram();
        }

        #region StartCommand
        public ICommand StartCommand { get; }

        private bool CanStartCommandExecute(object p) => true;

        private void OnStartCommandExecuted(object p)
        {
            //SiosWaitAndStartRecord();
            //
            //double weight;

            FreqValue = SelectedSetPoint.Frequency.Value;

            DisplcValue = SelectedSetPoint.Displacement.Value;

            VelValue = SelectedSetPoint.Velocity.Value;

            AccelValue = SelectedSetPoint.Acceleration.Value;

            SetNewOscillationParamAndUpdate(SelectedSetPoint.Frequency.Value, SelectedSetPoint.Displacement.Value);

            //
            fftWrIdx = 0;

            x_reg_end = 0;

            rawDataForRegPlotIdx = 0;

            SiosDoubleAccRawDataSize = (int)(((double)_siosManagerModel.Rate * (double)SetRegParams.NumOfRegPeriods) / (double)FreqValue);

            for (int i = 0; i < SiosDoubleRawDataForRegPlot.Length; i++)
            {
                SiosDoubleRawDataForRegPlot[i] = 0;
            }

            var s = (LineSeries)SiosRegRawPlotModel.Series[0];
            s.Points.Clear();

            s = (LineSeries)SiosRegRawPlotModel.Series[1];
            s.Points.Clear();

            SiosRegRawPlotModel.InvalidatePlot(true);

            SessionStartTime = DateTime.Now;

            SetNewStatusDispatcher(DeviceStateViewModel.enDeviceStates.Ok,
                                                Properties.Resources.StartRegistr);

            isRegInabled = false;// true; // AlexM 090325
            //

            if (SiosUsed)
            {
                if (motionController.IsCenter)
                {
                    if (siosIsReset == false)
                    {
                        //siosIsReset = true;

                        _siosManagerModel.Reset(); // AlexM 280324
                    }
                }

                if (IsSiosProc == false)
                {
                    IsSiosProc = true;

                    for (int i = 0; i < SiosDoubleAccRawData.Length; i++)
                    {
                        SiosDoubleAccRawData[i] = 0;
                    }

                    SetRawPlotForPlayAxes();

                    SetPlayParam();

                    if (motionController.IsCenter)
                    {
                        if (siosIsReset == false)
                        {
                            //siosIsReset = true;

                            //_siosManagerModel.Reset();
                        }
                    }

                    SiosWaitAndStartRecord(); // AlexM 110324
                    
                    fftWrIdx = 0;

                    x_play_end = 0;

                    SaveRawDataEn = true;

                    SaveDataToPdf = true;
                }
            }

            Thread.Sleep(100);

            //PlayOrPause();
            //MotionController.StartOscillation();
            //MotionController.UpdateOscillation();
            MotionController.IsMoving = true;

            //weight = 0;
            // Можно передать массив
            Start(LoadWeight); // AlexM 110324
        }
        #endregion

        #region StopCommand
        public ICommand StopCommand { get; }

        private bool CanStopCommandExecute(object p) => true;

        private void OnStopCommandExecuted(object p)
        {
            
            _siosManagerModel.Stop();

            IsSiosProc = false;

            MotionController.IsMoving = false;

            Stop();
        }
        #endregion

        bool isRegInabled = false;  

        #region StartRegCommand
        public ICommand StartRegCommand { get; }

        private bool CanStartRegCommandExecute(object p) => true;

        private void OnStartRegCommandExecuted(object p)
        {            
            fftWrIdx = 0;

            x_reg_end = 0;

            rawDataForRegPlotIdx = 0;

            SiosDoubleAccRawDataSize = (int)(((double)_siosManagerModel.Rate * (double)SetRegParams.NumOfRegPeriods) / (double)FreqValue);

            for (int i = 0; i < SiosDoubleRawDataForRegPlot.Length; i++)
            {
                SiosDoubleRawDataForRegPlot[i] = 0;
            }

            var s = (LineSeries)SiosRegRawPlotModel.Series[0];
            s.Points.Clear();

            s = (LineSeries)SiosRegRawPlotModel.Series[1];
            s.Points.Clear();

            SiosRegRawPlotModel.InvalidatePlot(true);

            SessionStartTime = DateTime.Now;

            SetNewStatusDispatcher(DeviceStateViewModel.enDeviceStates.Ok,
                                                Properties.Resources.StartRegistr);

            isRegInabled = true;
        }
        #endregion

        #region StopRegCommand
        public ICommand StopRegCommand { get; }

        private bool CanStopRegCommandExecute(object p) => true;

        private void OnStopRegCommandExecuted(object p)
        {
            isRegInabled = false;
        }
        #endregion
       

        Views.Windows.ChartsSummaryView elmoChartsSummaryView = new Views.Windows.ChartsSummaryView();

        public bool IsAtCenter()
        {
            return ((MotionController.GetStatus() & (0x01 << 18)) == (0x01 << 18));
        }

        bool siosIsReset = false;

        private async void SiosWaitAndStartRecord()
        {
            //while (_siosManagerModel.IsReadyForStart())

            int cnt = 0;

            //var ready = await IsReadyForMeasure();

            //while(IsSiosProc && (cnt < 10)) // !motionController.IsCenter && 
            while (IsSiosProc && !motionController.IsCenter) // 150224
            //while(!MotionController.IsReadyForMeasurment())
            //while (!IsAtCenter())
            {
                //Thread.Sleep(100);
                await Task.Run(() => Thread.Sleep(100));

                cnt++;
            }

            init_cnt = 0;
            /*
            if (siosIsReset == false)
            {
                //siosIsReset = true;

                _siosManagerModel.Reset();
            }
            */
            if(SiosUsed)
            {
                if (_siosManagerModel.Start())
                {

                }
            }
            

            //motionController.IsCenter = true;

            while (IsSiosProc && SiosWaitRecorderedDataIsReady())
            //while(true)
            {
                await SiosConfigureRecorderAndStartMeasurements();
            }
        }

        // SIOSManagerViewModel _siosManagerViewModel = new SIOSManagerViewModel();

        double x_init = 0;

        double x_play_end;
        double x_play_size;

        int plotPlaySize;

        double x_reg_end;
        double x_reg_size;

        int plotRegSize;

        int init_cnt = 0;

        const int arr_size = 15000000; //1048576 * 10;//131072 * 4; // 16

        double[] SiosDoubleAccRawData = new double[arr_size]; // 2

        int SiosDoubleAccRawDataSize = 0;

        DataPoint[] SiosDoubleAccData = new DataPoint[16384];
        DataPoint[] SiosAccRawData = new DataPoint[16384];

        double[] SiosDoubleRawDataForPlayPlot = new double[arr_size]; // 65536 // 2
        double[] SiosDoubleRawDataForRegPlot = new double[arr_size];

        //double[] SiosDoubleRawDataScaled = new double[16385];

        int rawDataForPlayPlotIdx = 0;
        int rawDataForRegPlotIdx = 0;

        int fftWrIdx = 0;

        bool SaveRawDataEn = false;

        bool SaveDataToPdf = true;

        Double time_gap_s = 1 / 1000;

        async Task SiosConfigureRecorderAndStartMeasurements()
        {
            //DataPoint[] data = await _siosManagerViewModel.DownloadRecorderedData();
            //DataPoint[] data = await SiosDownloadRecorderedData();
            SiosDoubleRawData = await SiosDownloadRecorderedData(); // Raw double recordered data

            //if (motionController.IsCenter)
            {
                init_cnt++;

                if (IsSiosProc && motionController.IsSteady) // if ((init_cnt > 5) && IsSiosProc)
                {
                    //Double time_gap_s = 1.0 / (double)_siosManagerModel.Rate;
                    time_gap_s = 1.0 / (double)_siosManagerModel.Rate;

                    var orderedRawData = SiosDoubleRawData.OrderBy(x => x).ToList();

                    double rawhighestPoint = orderedRawData.Last();
                    double rawlowestPoint = orderedRawData.First();

                    //double minPoint;
                    //double maxPoint;

                    //double delta;

                    double rawdelta = rawhighestPoint - rawlowestPoint;//(Math.Abs(rawhighestPoint) + Math.Abs(rawlowestPoint)); // Math.Abs

                    double rawOffset = rawhighestPoint - rawdelta / 2;

                    //minPoint = -rawdelta / 2;
                    //maxPoint = rawdelta / 2;

                    double[] SiosDoubleRawDataScaled = new double[SiosDoubleRawData.Length];

                    // Нормализуем данные
                    for (int i = 0; i < SiosDoubleRawData.Length; i++)
                    {
                        SiosDoubleRawDataScaled[i] = SiosDoubleRawData[i];// - rawOffset;// + rawdelta / 2;

                        SiosDoubleRawDataForPlayPlot[rawDataForPlayPlotIdx++] = SiosDoubleRawDataScaled[i];

                        if(rawDataForPlayPlotIdx >= plotPlaySize)
                        {
                            rawDataForPlayPlotIdx = 0;
                        }

                        if (isRegInabled)
                        {
                            SiosDoubleRawDataForRegPlot[rawDataForRegPlotIdx++] = SiosDoubleRawDataScaled[i];

                            if (rawDataForRegPlotIdx >= plotRegSize)
                            {
                                rawDataForRegPlotIdx = 0;
                            }
                        }
                    }

                    SetPlayValue(SiosDoubleRawDataScaled[0]/ 2000000000);                   

                    //if (SelectedPage == 1)
                    {
                        x_play_end = x_play_end + SiosDoubleRawData.Length * time_gap_s;

                        if (SelectedPage == 1)
                        {
                            var s = (LineSeries)SiosRawPlotModel.Series[0];

                            SiosRawData = Instruments.DataConvertion.ConvertDoubleDataToOxyPoints(SiosDoubleRawDataForPlayPlot, x => (x * time_gap_s), y => y / 2000000, plotPlaySize);
                            s.Points.Clear();
                            s.Points.AddRange(SiosRawData);

                            /**/
                            var orderedSeries = s.Points.OrderBy(o => o.Y).ToList();

                            double highestPoint = orderedSeries.Last().Y;
                            double lowestPoint = orderedSeries.First().Y;

                            SiosRawPlotModel.Axes[0].AbsoluteMaximum = highestPoint;
                            SiosRawPlotModel.Axes[0].AbsoluteMinimum = lowestPoint;

                            SiosRawPlotModel.Axes[0].Maximum = highestPoint;
                            SiosRawPlotModel.Axes[0].Minimum = lowestPoint;

                            SiosRawPlotModel.ResetAllAxes();
                            /**/

                            var l = (LineAnnotation)SiosRawPlotModel.Annotations[0];
                            /**/
                            l.MaximumY = highestPoint;
                            l.MinimumY = lowestPoint;
                            /**/
                            l.X = x_play_end;

                            this.SiosRawPlotModel.InvalidatePlot(true);
                        }

                        if (x_play_end >= x_play_size)
                        {
                            x_play_end = x_play_end - x_play_size;
                        }
                    }

                    //if (SelectedPage == 2)
                    {
                        if (isRegInabled)
                        {
                            x_reg_end = x_reg_end + SiosDoubleRawData.Length * time_gap_s;                       

                            if (SelectedPage == 2)
                            {
                                var s = (LineSeries)SiosRegRawPlotModel.Series[0];

                                SiosRawData = Instruments.DataConvertion.ConvertDoubleDataToOxyPoints(SiosDoubleRawDataForRegPlot, x => (x * time_gap_s), y => y / 2000000, plotRegSize);
                                s.Points.Clear();
                                s.Points.AddRange(SiosRawData);

                                var l = (LineAnnotation)SiosRegRawPlotModel.Annotations[0];
                                l.X = x_reg_end;

                                this.SiosRegRawPlotModel.InvalidatePlot(true);
                            }

                            if (x_reg_end >= x_reg_size)
                            {
                                x_reg_end = x_reg_end - x_reg_size;
                            }
                        }
                    }                    

                    if (motionController.IsSteady && isRegInabled)
                    {// Перемещение достигло заданной амплитуды
                        // Накапливаем нормированные данные  
                        for (int i = 0; i < SiosDoubleRawData.Length; i++)
                        {
                            SiosDoubleAccRawData[fftWrIdx++] = SiosDoubleRawDataScaled[i];

                            if (fftWrIdx >= SiosDoubleAccRawDataSize) // SiosDoubleAccRawData.Length
                            //if (fftWrIdx >= SiosDoubleAccRawData.Length)
                            {  
                                ExecuteShowGraph(null);

                                isRegInabled = false;

                                break;
                            }
                        }
                    }                                                            
                }
            }
        }

        private SetRegParams_VM _set_reg_params;// = new SetRegParams_VM();
        public SetRegParams_VM SetRegParams
        {
            get { return _set_reg_params; }
            set
            {
                if (value == null)
                    return;

                _set_reg_params = value;
                OnPropertyChanged("SetRegParams");                
            }
        }

        private SetPidParams_VM _set_pid_params;// = new SetRegParams_VM();
        public SetPidParams_VM SetPidParams
        {
            get { return _set_pid_params; }
            set
            {
                if (value == null)
                    return;

                _set_pid_params = value;
                OnPropertyChanged("SetPidParams");
            }
        }

        public Action<Double, Double> SetNewOscillationParamAndUpdateCallback;

        private SetPoint_VM selectedSetPoint;
        public SetPoint_VM SelectedSetPoint
        {
            get { return selectedSetPoint; }
            set
            {
                if (value == null)
                    return;
                selectedSetPoint = value;
                OnPropertyChanged("SelectedSetPoint");

                if (value.IsAllValuesIsOk() && (SetNewOscillationParamAndUpdateCallback != null))
                    SetNewOscillationParamAndUpdateCallback(value.Frequency.Value, value.Displacement.Value);
            }
        }

        public Models.Borders Borders { get; set; }

        private String units = "m";
        public String DisplacementUnits
        {
            get { return units; }
            set
            {
                units = value;
                OnPropertyChanged("DisplacementUnits");
            }
        }

        public USH_VM()
        {
            #region Команды
            CloseApplicationCommand = new LambdaCommand(OnCloseApplicationCommandExecuted, CanCloseApplicationCommandExecute);

            StartCommand = new LambdaCommand(OnStartCommandExecuted, CanStartCommandExecute);
            StopCommand = new LambdaCommand(OnStopCommandExecuted, CanStopCommandExecute);            

            StartRegCommand = new LambdaCommand(OnStartRegCommandExecuted, CanStartRegCommandExecute);
            StopRegCommand = new LambdaCommand(OnStopRegCommandExecuted, CanStopRegCommandExecute);
            #endregion

            SiosRawPlotModel = new PlotModel();            
            
            SiosRawPlotModel.Title = "Осциллограф";

            SiosRawPlotModel.Axes.Add(new LinearAxis()
            {
                Position = AxisPosition.Left,
                Title = "Перемещение, мм",
                MajorGridlineStyle = LineStyle.Dot,
                AbsoluteMinimum = 0,
                AbsoluteMaximum = 20,
                StringFormat = "0.000"
            });

            SiosRawPlotModel.Axes.Add(new LinearAxis() 
            { 
                Position = AxisPosition.Bottom, 
                Title = "Время, с", 
                MajorGridlineStyle = LineStyle.Dot,
                AbsoluteMinimum = 0,
                AbsoluteMaximum = 20,
                StringFormat = "0.000"
            });
            
            var SiosRawSeries = new LineSeries { Title = "Sios Data", MarkerType = MarkerType.None };            

            //PlotModel.Series.Add(new LineSeries { LineStyle = LineStyle.Solid });
            SiosRawPlotModel.Series.Add(SiosRawSeries);

            var SiosMarkSeries = new LineSeries { MarkerType = MarkerType.None };

            SiosRawPlotModel.Series.Add(SiosMarkSeries);

            SiosRawPlotModel.Annotations.Add(new LineAnnotation { Type = LineAnnotationType.Vertical, X = 0, MaximumY = 20, Color = OxyColors.Green });

            SiosFFTPlotModel = new PlotModel();

            SiosFFTPlotModel.Title = "FFT";

            SiosFFTPlotModel.Axes.Add(new LinearAxis()
            {
                Position = AxisPosition.Left,
                Title = "Перемещение, мм",
                MajorGridlineStyle = LineStyle.Dot,
                AbsoluteMinimum = 0,
                AbsoluteMaximum = 20,
                StringFormat = "0.000"
            });

            SiosFFTPlotModel.Axes.Add(new LogarithmicAxis() 
            { 
                Position = AxisPosition.Bottom, 
                Title = "Частота, Гц", 
                MajorGridlineStyle = LineStyle.Dot,
                AbsoluteMinimum = 0,
                AbsoluteMaximum = 20,
                StringFormat = "0.00"
            });
            
            var SiosFFTSeries = new LineSeries { Title = "Sios Data", MarkerType = MarkerType.None };

            //PlotModel.Series.Add(new LineSeries { LineStyle = LineStyle.Solid });
            SiosFFTPlotModel.Series.Add(SiosFFTSeries);

            SiosRegRawPlotModel = new PlotModel();

            SiosRegRawPlotModel.Title = "Осциллограф";

            SiosRegRawPlotModel.Axes.Add(new LinearAxis()
            {
                Position = AxisPosition.Left,
                Title = "Перемещение, мм",
                MajorGridlineStyle = LineStyle.Dot,
                AbsoluteMinimum = 0,
                AbsoluteMaximum = 20,
                StringFormat = "0.000"
            });

            SiosRegRawPlotModel.Axes.Add(new LinearAxis()
            {
                Position = AxisPosition.Bottom,
                Title = "Время, с",
                MajorGridlineStyle = LineStyle.Dot,
                AbsoluteMinimum = 0,
                AbsoluteMaximum = 20,
                StringFormat = "0.000"
            });

            var SiosRegRawSeries = new LineSeries { Title = "Sios Data", MarkerType = MarkerType.None };

            //PlotModel.Series.Add(new LineSeries { LineStyle = LineStyle.Solid });
            SiosRegRawPlotModel.Series.Add(SiosRegRawSeries);

            var SiosRegMarkSeries = new LineSeries { MarkerType = MarkerType.None };

            SiosRegRawPlotModel.Series.Add(SiosRegMarkSeries);

            SiosRegRawPlotModel.Annotations.Add(new LineAnnotation { Type = LineAnnotationType.Vertical, X = 0, MaximumY = 20, Color = OxyColors.Green });

            recorderVM = motionController;
            DeviceName = "USH";

            DeviceSettingsInitialization();

            Borders = model.Borders;

            SelectedSetPoint = new SetPoint_VM() { Borders = this.Borders, Units = this.DisplacementUnits };

            SelectedSetPoint.SetPointChanged += SelectedPointChanged;

            SetNewOscillationParamAndUpdateCallback = SetNewOscillationParamAndUpdate;                        

            SavePath = model.PathToDataFolder;

            _siosManagerModel.Rate = 512;// 1000;// 512;

            periodSios = 100;

            SetRegParams = new SetRegParams_VM();            

            PlayParam = new PlayingParam_VM();

            SetPidParams = new SetPidParams_VM();

            SelectedPage = 0;

            LoadWeight = 0;

            SiosUsed = true;

            //ResPress = 0;
        }

       

        public double[] GetRecordingData()
        {
            //double[] sios_data = new double[50];

            return _siosManagerModel.GetLenghtValues();
            /*
            for (var x = 0d; x <= 49; x += 0.1)
            {
                sios_data[(int)x] = (double)x * 2;
            }

            return sios_data;
            */
        }

        public async Task<double[]> SiosDownloadRecorderedData()
        {
            var data_ready = await Task.Run<Boolean>((Func<Boolean>)SiosWaitRecorderedDataIsReady);
            if (data_ready)
            {
                double[] recordingData = await Task.Run<double[]>((Func<double[]>)GetRecordingData);
                //DataPoint[] dataPoints = <OxyPlot.DataPoint>((int)(360 / 0.1));
                if (recordingData != null)
                {
                    //DataPoint[] dataPoints = Instruments.DataConvertion.ConvertDoubleDataToOxyPoints(recordingData, x => x * 1, y => y / 1);

                    //return dataPoints;
                    return recordingData;
                }
                return null;

            }
            throw new Exception("Failed to download data from recorder.");
        }

        protected static Boolean SiosWaitRecorderedDataIsReady()
        {
            Thread.Sleep(periodSios);
            //Task.Run(() => Thread.Sleep(500));
            return true;
        }

        public async void ConnectDevices()
        {
            bool sios_connected;

            DeviceSettingsInitialization();            
            
            SetNewStatusDispatcher(DeviceStateViewModel.enDeviceStates.Work,
                                           Properties.ResourcesE.DeviceTryToConnect);
            

            var dev_connected = await MotionController.ConnectAsync();
            //await RecorderController.ConnectAsync();
            /*
            if(dev_connected)
            {
                MotionController.IsCenter = false;

                MotionController.ResetIsCenter();

                //MotionController.StartOscaillationProgram();

                MotionController.KillUserProgram();

                MotionController.StartUserProgram();
            }
            */            
            
            SetNewStatusDispatcher(DeviceStateViewModel.enDeviceStates.Work,
                                           Properties.ResourcesS.DeviceTryToConnect);

            if(SiosUsed)
            {
                // Sios
                sios_connected = _siosManagerModel.Connect();

                if (!sios_connected)
                {
                    SetNewStatusDispatcher(DeviceStateViewModel.enDeviceStates.Error,
                                                Properties.ResourcesS.UnableToConnect);
                }
                else
                {
                    SetNewStatusDispatcher(DeviceStateViewModel.enDeviceStates.Ok,
                                                Properties.ResourcesS.Connected);
                }
            }
            else
            {
                sios_connected = true;

                SetNewStatusDispatcher(DeviceStateViewModel.enDeviceStates.Warning,
                                                Properties.ResourcesS.IsNotUsed);
            }

            /*
            SetNewStatusDispatcher(DeviceStateViewModel.enDeviceStates.Ok,
                                                Properties.Resources.progIsRemoteMaster);
            */

            
            if (dev_connected && sios_connected)
            {
                MotionController.IsCenter = false;

                MotionController.ResetIsCenter();

                //MotionController.StartOscaillationProgram();

                MotionController.KillUserProgram();

                MotionController.StartUserProgram();

                SetNewStatusDispatcher(DeviceStateViewModel.enDeviceStates.Work,
                                           Properties.ResourcesE.PlatformInitializing);
            }
            
        }

        public event Action<Object, Double> ValueChanged;

        private void SetNewOscillationParamAndUpdate(Double frequency, Double displacement)
        {
            try
            {
                if (!MotionController.IsConnected)
                    return;

                if (RecorderController.IsRecordingNow())
                    RecorderController.StopRecorder();

                MotionController.SetOscillationParameters(frequency, displacement);
                /* // ALexM 090124
                if (IsAuto && MotionController.IsOscillationPaused())
                {
                    MotionController.ResumeOscillation();
                    SetWorkStateByDispach(enStates.Oscillate);
                    WaitAndStartRecord();
                    // new Thread(WaitAndStartRecord) { IsBackground = true }.Start();
                }
                else if (MotionController.IsPlaying)
                    WaitAndStartRecord();
                //new Thread(WaitAndStartRecord) { IsBackground = true }.Start();
                */
                SetNewStatusDispatcher(DeviceStateViewModel.enDeviceStates.Ok,
                    String.Format(Properties.Resources.New_oscillation_value_set, frequency.ToString(), displacement.ToString()));
            }
            catch (Exception ex)
            {
                SetNewStatusDispatcher(DeviceStateViewModel.enDeviceStates.Error, Properties.Resources.Failed_to_set_new_oscillation_parameters, ex);
            }
        }

        private void SetWorkStateByDispach(enStates state)
        {
            CDispatcher.BeginInvoke(
                (Action)(() =>
                {
                    WorkState = state;

                }));
        }

        private async void WaitAndStartRecord()
        {
            try
            {
                if (!model.AutoMeasurements)
                {
                    //InitiateNextPointIfAuto();
                    return;
                }

                /*
                var ready = await IsReadyForMeasure();
                if (ready)
                {
                    await ConfigureRecorderAndStartMeasurements();

                    //InitiateNextPointIfAuto();
                    return;
                }*/
                // For work AlexM
                while (MotionController.IsInMotion())
                {
                    if (MotionController.IsReadyForMeasurment())
                    {
                        //await ConfigureRecorderAndStartMeasurements();

                //        InitiateNextPointIfAuto();
                        return;
                    }
                       System.Threading.Thread.Sleep(50);
                }
            }
            catch (Exception ex)
            {
                SetNewStatusDispatcher(DeviceStateViewModel.enDeviceStates.Error, Properties.Resources.Failed_to_start_recording, ex);
            }
        }

        private async Task<Boolean> IsReadyForMeasure()
        {
            while (MotionController.IsInMotion())
            {
                if (MotionController.IsReadyForMeasurment())
                    return true;

                await Task.Run(() => Thread.Sleep(100));
            }
            return false;
        }

        private async Task ConfigureRecorderAndStartMeasurements()
        {
            Double measure_time = model.NumberOfPeriods / MotionController.SetFrequencyHz;
            if (measure_time > model.MaxMeasureTime_minutes * 60.0)
            {
                measure_time = (Math.Floor((model.MaxMeasureTime_minutes * 60.0) * MotionController.SetFrequencyHz)) / MotionController.SetFrequencyHz;
            }
            /*
            RecorderController.ConfigureRecorder(measure_time);
            RecorderController.StartRecorder();
            */

            SetWorkStateByDispach(enStates.Measure);
            /*
            DataPoint[] data = await RecorderController.DownloadRecorderedData();
            SetWorkStateByDispach(enStates.Oscillate);
            
            CreateAndSetAsCurrent(MotionController.SetFrequencyHz, MotionController.SetDisplacement_m, model.MaxFrequencyRelativeSigma);

            SetDataToCurrentAndAdd(data);
            */
        }
        

        private double[] _SiosDoubleRawData;

        public double[] SiosDoubleRawData
        {
            get { return _SiosDoubleRawData; }
            private set
            {
                _SiosDoubleRawData = value;
                OnPropertyChanged("SiosDoubleRawData");
            }
        }

        private DataPoint[] _SiosRawData;

        public DataPoint[] SiosRawData
        {
            get { return _SiosRawData; }
            private set
            {
                _SiosRawData = value;
                OnPropertyChanged("SiosRawData");
            }
        }                
                
        public DateTime SessionStartTime { get; private set; }
        public String SavePath { get; set; }                            
        
        public String DeviceName { get; set; }                                           

        private void PlayOrPause() //Object param
        {
            try
            {
                if (MotionController.IsUserProgramWork())
                {
                    /*
                    Boolean paused = MotionController.IsOscillationPaused();
                    if (paused)
                    {
                        MotionController.ResumeOscillation();
                        SetWorkStateByDispach(enStates.Oscillate);
                        //WaitAndStartRecord();
                        //new Thread(WaitAndStartRecord) { IsBackground = true }.Start();
                    }
                    else
                    {
                        MotionController.PauseOscillation();
                        SetWorkStateByDispach(enStates.Paused);
                        //RecorderController.StopRecorder();

                    }
                    */

                    MotionController.ResumeOscillation();
                    SetWorkStateByDispach(enStates.Oscillate);
                }
                else
                {
                    MotionController.StartOscaillationProgram();
                    SetWorkStateByDispach(enStates.Oscillate);
                    //WaitAndStartRecord();
                    //new Thread(WaitAndStartRecord) { IsBackground = true }.Start();
                }

            }
            catch (Exception ex)
            {
                SetNewStatusDispatcher(DeviceStateViewModel.enDeviceStates.Error, "Cant play or pause", ex);
            }
        }

        private void Start(double weight) //Object param
        {
            try
            {
                //IsAuto = false;
                //MotionController.StopOscillationProgram();
                //MotionController.KillUserProgram(); 
                MotionController.StartOscillation(weight, SetPidParams.Kp1_start, SetPidParams.Ki1_start, SetPidParams.Kp2_start, SetPidParams.Ki2_start,
                    SetPidParams.Kp1_steady, SetPidParams.Ki1_steady, SetPidParams.Kp2_steady, SetPidParams.Ki2_steady, SetPidParams.Kp3_start, SetPidParams.Kp3_steady);

                SetNewStatusDispatcher(DeviceStateViewModel.enDeviceStates.Ok,
                                                Properties.Resources.StartPlay);

                //RecorderController.StopRecorder();
                SetWorkStateByDispach(enStates.DoNothing);
            }
            catch (Exception ex)
            {
                SetNewStatusDispatcher(DeviceStateViewModel.enDeviceStates.Error, "Cant stop", ex);
            }
        }

        private void Stop() //Object param
        {
            try
            {
                //IsAuto = false;
                //MotionController.StopOscillationProgram();
                MotionController.StopOscillation();
                //RecorderController.StopRecorder();
                SetWorkStateByDispach(enStates.DoNothing);
            }
            catch (Exception ex)
            {
                SetNewStatusDispatcher(DeviceStateViewModel.enDeviceStates.Error, "Cant stop", ex);
            }
        }

        public override void DeviceSettingsInitialization()
        {
            base.DeviceSettingsInitialization();
            CheckPaths();

            UpdateAfterSaveSettings();

        }

        private void CheckPaths()
        {
            model.PathToListFolder = Instruments.FilesAndFolders.CheckPathToFolderAndReturnSuitable(model.PathToListFolder,
                model.MainWindowName, "Lists");
            model.PathToDataFolder = Instruments.FilesAndFolders.CheckPathToFolderAndReturnSuitable(model.PathToDataFolder,
                model.MainWindowName, "Data");
        }

        private void UpdateAfterSaveSettings()
        {
            MainWinName = model.MainWindowName;
            Borders = model.Borders;
            //Table_VM.ListsFolder = model.PathToListFolder;
            //Table_VM.DisplacementUnits = model.DisplacementUnits;
            SavePath = model.PathToDataFolder;
            DeviceName = model.MainWindowName;
            DisplacementUnits = model.DisplacementUnits;
            MotionController.Units = model.DisplacementUnits;

        }        

        private void SetRawPlotForPlayAxes()
        {           
            double maxPointX;

            double minPointY;
            double maxPointY;

            double delta;

            double displ;
            double freq;

            if (SelectedSetPoint.Frequency.Value > 0)
            {
                //displ = (DisplcValue * 1000);
                displ = SelectedSetPoint.Displacement.Value * 1000;
                freq = SelectedSetPoint.Frequency.Value;

                delta = 2 * displ;

                displ = displ + delta / 20;

                minPointY = -displ;// - delta / 2;
                maxPointY = displ;// delta / 2;

                SiosRawPlotModel.Axes[0].AbsoluteMaximum = maxPointY;
                SiosRawPlotModel.Axes[0].AbsoluteMinimum = minPointY;

                SiosRawPlotModel.Axes[0].Maximum = maxPointY;
                SiosRawPlotModel.Axes[0].Minimum = minPointY;

                SiosRawPlotModel.Axes[0].MajorStep = delta / 10;

                if (freq > 0.1)
                {
                    numOfPeriodsForPlot = 4;

                    //numOfPeriodsForFft = 10;
                }
                else
                {
                    numOfPeriodsForPlot = 2;

                    //numOfPeriodsForFft = 2;
                }

                maxPointX = (double)numOfPeriodsForPlot / freq;

                SiosRawPlotModel.Axes[1].AbsoluteMaximum = maxPointX;
                SiosRawPlotModel.Axes[1].Maximum = maxPointX;

                SiosRawPlotModel.Axes[1].MajorStep = (maxPointX) / 10;

                SiosRawPlotModel.ResetAllAxes();

                for (int i = 0; i < SiosDoubleRawDataForPlayPlot.Length; i++)
                {
                    SiosDoubleRawDataForPlayPlot[i] = 0;
                }

                var s = (LineSeries)SiosRawPlotModel.Series[0];
                s.Points.Clear();

                s = (LineSeries)SiosRawPlotModel.Series[1];
                s.Points.Clear();

                SiosRawPlotModel.InvalidatePlot(true);

                x_play_end = 0;
                x_play_size = maxPointX;

                rawDataForPlayPlotIdx = 0;

                _siosManagerModel.Rate = SetRegParams.SiosRate;

                plotPlaySize = (int)(x_play_size * _siosManagerModel.Rate);

                var l = (LineAnnotation)SiosRawPlotModel.Annotations[0];
                l.MinimumY = minPointY;
                l.MaximumY = maxPointY;

                ResultValues.MeasuredFrequency = 0;
                ResultValues.MeasuredVel = 0;
                ResultValues.MeasuredAcc = 0;
                ResultValues.MeasuredDispl = 0;
            }
        }

        private void SetRawPlotForRegAxes()
        {
            double maxPointX;

            double minPointY;
            double maxPointY;

            double delta;

            double displ;
            double freq;

            if (SelectedSetPoint.Frequency.Value > 0)
            {
                displ = SelectedSetPoint.Displacement.Value * 1000;
                freq = SelectedSetPoint.Frequency.Value;

                delta = 2 * displ;

                displ = displ + delta / 20;

                minPointY = -displ;// - delta / 2;
                maxPointY = displ;// delta / 2;

                maxPointX = (double)SetRegParams.NumOfRegPeriods / freq;

                SiosRegRawPlotModel.Axes[0].AbsoluteMaximum = maxPointY;
                SiosRegRawPlotModel.Axes[0].AbsoluteMinimum = minPointY;

                SiosRegRawPlotModel.Axes[0].Maximum = maxPointY;
                SiosRegRawPlotModel.Axes[0].Minimum = minPointY;

                SiosRegRawPlotModel.Axes[0].MajorStep = delta / 10;

                SiosRegRawPlotModel.Axes[1].AbsoluteMaximum = maxPointX;
                SiosRegRawPlotModel.Axes[1].Maximum = maxPointX;

                SiosRegRawPlotModel.Axes[1].MajorStep = (maxPointX) / 10;

                SiosRegRawPlotModel.ResetAllAxes();

                for (int i = 0; i < SiosDoubleRawDataForRegPlot.Length; i++)
                {
                    SiosDoubleRawDataForRegPlot[i] = 0;
                }

                var s = (LineSeries)SiosRegRawPlotModel.Series[0];
                s.Points.Clear();

                s = (LineSeries)SiosRegRawPlotModel.Series[1];
                s.Points.Clear();

                var l = (LineAnnotation)SiosRegRawPlotModel.Annotations[0];
                l.MinimumY = minPointY;
                l.MaximumY = maxPointY;

                SiosRegRawPlotModel.InvalidatePlot(true);

                x_reg_end = 0;
                x_reg_size = maxPointX;

                rawDataForRegPlotIdx = 0;

                plotRegSize = (int)(x_reg_size * _siosManagerModel.Rate);
            }
        }
                
        private void SetPlayParam()
        {
            switch(SelectedSetPoint.ExecutedParam)
            {
                case 3: // Velocity
                    PlayParam.ParamName = "V, m/s";
                    break;

                case 4: // Acceleration
                    PlayParam.ParamName = string.Concat("A, m/s", '\u00B2');
                    break;

                case 2: // Displacement
                    PlayParam.ParamName = "X, m";
                    break;
            }
            
        }

        private void SetPlayValue(double value)
        {
            switch (SelectedSetPoint.ExecutedParam)
            {
                case 3: // Velocity
                    PlayParam.PlayParamValue = value * 2 * Math.PI * FreqValue; 
                    break;

                case 4: // Acceleration
                    PlayParam.PlayParamValue = value * Math.Pow(2 * Math.PI * FreqValue, 2);
                    break;

                case 2: // Displacement
                    PlayParam.PlayParamValue = value;
                    break;
            }
        }

        WindowService ws = new WindowService();

        public void ExecuteShowGraph(object parameter)
        {
            time_gap_s = 1.0 / (double)_siosManagerModel.Rate;

            ws.ShowWindow<ReportView>(new Report_VM(time_gap_s, FreqValue, DisplcValue, VelValue, AccelValue, LoadWeight, SiosDoubleAccRawData, SiosDoubleAccRawDataSize, SessionStartTime, SavePath));
        }

        //public SiosDataVM siosDataVM = new SiosDataVM();

        void SelectedPointChanged(Object sender)
        {
            // Критерии поиска
            double freq_trg = ((SetPointValue_VM)((SetPoint_VM)sender).Frequency).Value;
            double dipl_trg = ((SetPointValue_VM)((SetPoint_VM)sender).Displacement).Value;

            double uf14 = 10;
            double uf15 = 120;
            double uf16 = 6E-7;
            double uf17 = 4;

            double uf10 = 10;
            double uf11 = 120;
            double uf12 = 6E-7;
            double uf13 = 4;

            double uf18 = 180; ;

            double weigth;

            double freq_prev = 0;
            double freq;
            double dipl_prev = 0;
            double dipl;

            double dipl_delta;

            bool freq_found = false;
            bool found = false;

            // LoadWeight
            // ПОиск ближайших коэффициентов в файле
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                HasHeaderRecord = true,
            };

            using (var stream = File.Open("./Settings/reg_params.csv", FileMode.Open))
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvReader(reader, config))
            {
                csv.Read();
                csv.ReadHeader();
                while (csv.Read() && !found)
                {
                    /*
                    var records = csv.GetRecords<PidParams>();                    
                    
                    weigth = ((PidParams)records).W;
                    
                    SetPidParams.Kp1_start = ((PidParams)records).uf14;
                    */

                    weigth = csv.GetField<double>(0);

                    if(weigth == LoadWeight)
                    {
                        freq = csv.GetField<double>(1);

                        dipl = csv.GetField<double>(2);

                        if(freq_prev != freq)
                        {
                            freq_found = false;
                        }

                        if ((freq_trg >= freq_prev) && (freq >= freq_trg))
                        {
                            freq_found = true;
                        }
                        else
                        {
                            //freq_found = false;
                        }

                        if (freq_found)
                        {                            
                            if (dipl == dipl_trg)
                            {
                                uf14 = csv.GetField<double>(3);
                                uf15 = csv.GetField<double>(4);
                                uf16 = csv.GetField<double>(5);
                                uf17 = csv.GetField<double>(6);

                                uf10 = csv.GetField<double>(7);
                                uf11 = csv.GetField<double>(8);
                                uf12 = csv.GetField<double>(9);
                                uf13 = csv.GetField<double>(10);

                                uf18 = csv.GetField<double>(11);

                                found = true;
                            }
                            else
                            {
                                dipl_delta = (dipl - dipl_prev)/2;

                                if ((dipl_trg >= dipl_prev) && (dipl > dipl_trg))
                                {
                                    found = true;

                                    if(dipl_trg > (dipl_prev + dipl_delta))
                                    {
                                        uf14 = csv.GetField<double>(3);
                                        uf15 = csv.GetField<double>(4);
                                        uf16 = csv.GetField<double>(5);
                                        uf17 = csv.GetField<double>(6);

                                        uf10 = csv.GetField<double>(7);
                                        uf11 = csv.GetField<double>(8);
                                        uf12 = csv.GetField<double>(9);
                                        uf13 = csv.GetField<double>(10);

                                        uf18 = csv.GetField<double>(11);
                                    }                                    
                                }
                            }

                            if(!found)
                            {
                                uf14 = csv.GetField<double>(3);
                                uf15 = csv.GetField<double>(4);
                                uf16 = csv.GetField<double>(5);
                                uf17 = csv.GetField<double>(6);

                                uf10 = csv.GetField<double>(7);
                                uf11 = csv.GetField<double>(8);
                                uf12 = csv.GetField<double>(9);
                                uf13 = csv.GetField<double>(10);

                                uf18 = csv.GetField<double>(11);
                            }

                            dipl_prev = dipl;
                        }

                        freq_prev = freq;                                                
                    }                                        
                }

                SetPidParams.Kp1_start = uf14;
                SetPidParams.Ki1_start = uf15;

                SetPidParams.Kp2_start = uf16;
                SetPidParams.Ki2_start = uf17;

                SetPidParams.Kp1_steady = uf10;
                SetPidParams.Ki1_steady = uf11;

                SetPidParams.Kp2_steady = uf12;
                SetPidParams.Ki2_steady = uf13;

                SetPidParams.Kp3_start = uf18;
            }
        }
    }
}

