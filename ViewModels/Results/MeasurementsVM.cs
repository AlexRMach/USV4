using Common.WPF.ViewModels;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using ush4.Instruments;
using ush4.Models;

namespace ush4.ViewModels.Results
{
    public class MeasurementsVM : DeviceViewModel
    {
        private DataPoint[] rawData;
        public DataPoint[] RawData
        {
            get { return rawData; }
            private set
            {
                rawData = value;
                OnPropertyChanged("RawData");
            }
        }

        private DataPoint[] fft;
        public DataPoint[] FFT
        {
            get { return fft; }
            private set
            {
                fft = value;
                OnPropertyChanged("FFT");
            }
        }

        private DateTime start_time;
        public DateTime StartTime
        {
            get { return start_time; }
            set
            {
                start_time = value;
                OnPropertyChanged("StartTime");
            }
        }

        private ResultSummaryVM summaryVM = new ResultSummaryVM();
        public ResultSummaryVM Summary
        {
            get { return summaryVM; }
            set
            {
                summaryVM = value;
                OnPropertyChanged("Summary");
            }
        }

        public Double MaxSigma { get; set; }

        public String FilePath { get; set; }
        public String DeviceName { get; set; }

        public async void SetData(Double[] data_cnt, Double factor_cnt_to_m, double time_gap_s)
        {
            try
            {
                RawData = Instruments.DataConvertion.ConvertDoubleDataToOxyPoints
                    (data_cnt, x => x * time_gap_s, y => y * factor_cnt_to_m);

                Double[] fft_mag = await Task.Run((Func<Double[]>)CalcFFT_from_RawData);
                // Double[] fft_mag = CalcFFT_from_RawData();
                FFT = Instruments.DataConvertion.ConvertDoubleDataToOxyPoints
                    (fft_mag, x => (x / (time_gap_s * fft_mag.Length)), y => 2.0 * y / fft_mag.Length, fft_mag.Length / 2);
                FFT[0] = new DataPoint(FFT[0].X, FFT[0].Y / 2.0);

                int freq_index = FindIndexOfFrequency(FFT, Summary.TypeToResultDict[Borders.enSetPointType.Frequency].SetValue, time_gap_s, MaxSigma);

                Summary.SetMeasuredValue(FFT[freq_index].X, FFT[freq_index].Y);

                SaveAllData(FilePath, StartTime, RawData, FFT, Summary);
            }
            catch (Exception ex)
            {
                SetNewStatusDispatcher(DeviceStateViewModel.enDeviceStates.Error, Properties.Resources.Error_proc_data, ex);
            }
        }


        public async void SetData(DataPoint[] raw_data)
        {
            try
            {
                RawData = raw_data;
                Double time_gap_s = raw_data[1].X - raw_data[0].X;
                Double[] fft_mag = await Task.Run((Func<Double[]>)CalcFFT_from_RawData);
                //Double[] fft_mag = await new Task<double[]>(CalcFFT_from_RawData);
                //Double[] fft_mag = CalcFFT_from_RawData();
                FFT = Instruments.DataConvertion.ConvertDoubleDataToOxyPoints
                    (fft_mag, x => (x / (time_gap_s * fft_mag.Length)), y => 2.0 * y / fft_mag.Length, fft_mag.Length / 2);
                FFT[0] = new DataPoint(FFT[0].X, FFT[0].Y / 2.0);

                int freq_index = FindIndexOfFrequency(FFT, Summary.TypeToResultDict[Borders.enSetPointType.Frequency].SetValue, time_gap_s, MaxSigma);

                Summary.SetMeasuredValue(FFT[freq_index].X, FFT[freq_index].Y);

                //SaveAllData(FilePath, StartTime, RawData, FFT, Summary);                
            }
            catch (Exception ex)
            {
                SetNewStatusDispatcher(DeviceStateViewModel.enDeviceStates.Error, Properties.Resources.Error_proc_data, ex);
            }
        }



        private Double[] CalcFFT_from_RawData()
        {
            return Instruments.FFTCalculation.FFT_Magnitude<DataPoint>(RawData, x => x.Y);
        }


        private int FindIndexOfFrequency(DataPoint[] FFT, Double frequency, double time_gap_s, Double max_sigma)
        {
            int l = FFT.Count();
            Double frequency_resolution = 1 / (2.0 * l * time_gap_s);
            max_sigma = frequency * max_sigma;
            int f = (int)(max_sigma / frequency_resolution);
            if (f <= 0) { f = 1; }

            int to_ret = 0;

            int mean_index = (int)Math.Floor(frequency / frequency_resolution);

            if (((mean_index - f) <= 0) || ((mean_index + f) >= l))
                return mean_index;


            Double max_amp = 0;
            for (int j = mean_index - f; j <= mean_index + f; j++)
            {
                if (max_amp < FFT[j].Y)
                {
                    max_amp = FFT[j].Y;
                    to_ret = j;
                }

            }
            return to_ret;


        }



        private void SaveAllData(String file, DateTime start_time, DataPoint[] raw, DataPoint[] fft, ResultSummaryVM summaryVM)
        {
            try
            {
                HDF5_Structs.Point[] raw_points = DataConvertion.ConvertOxyPointToHDFPoint(raw);
                HDF5_Structs.Point[] fft_points = DataConvertion.ConvertOxyPointToHDFPoint(fft);
                HDF5_Structs.SummaryValue[] summaryValues = DataConvertion.SummaryVMToHDFSummary(summaryVM);

                String groupname = String.Format("{0}  Set Frequency: {1} Hz", DateTime.Now.ToLongTimeString(), Summary.TypeToResultDict[Borders.enSetPointType.Frequency].SetValue);

                HDF5.SaveResultToHDF5(file, groupname, summaryValues, raw_points, fft_points, DeviceName);
            }
            catch (Exception ex)
            {
                SetNewStatusDispatcher(DeviceStateViewModel.enDeviceStates.Error, Properties.Resources.SaveError, ex);
            }
        }
        /*
        public RelayCommand ShowChartCommand
        {
            get
            {
                return new RelayCommand(x => (RawData != null), ShowCharts);
            }
        }
        */

        public void ShowCharts()// Object param
        {
            Views.Windows.ChartsSummaryView chartsSummaryView = new Views.Windows.ChartsSummaryView();
            ((ChartsSummaryVM)chartsSummaryView.DataContext).Measurement = this;
            chartsSummaryView.Show();
        }
    }
}
