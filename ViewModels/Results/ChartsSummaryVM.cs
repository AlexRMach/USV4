using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ush4.ViewModels.Base;
using Common.WPF.ViewModels;
using ViewModel = Common.WPF.ViewModels.ViewModel;

namespace ush4.ViewModels.Results
{
    public class ChartsSummaryVM : ViewModel
    {
        private MeasurementsVM measurementsVM = new MeasurementsVM(); // AlexM 051223

        public MeasurementsVM Measurement
        {
            get { return measurementsVM; }
            set
            {
                measurementsVM = value;
                SetDataToPlots(value);
                OnPropertyChanged("Measurement");
            }
        }

        private OxyPlot.PlotModel fft_oxymodel = new PlotModel() { Title = "FFT" };
        public OxyPlot.PlotModel FFT_Plotmodel
        {
            get { return fft_oxymodel; }
            set
            {
                fft_oxymodel = value;
                OnPropertyChanged("FFT_Plotmodel");
            }
        }


        private PlotModel raw_plotmodel = new PlotModel() { Title = "Raw" };

        public PlotModel Raw_PlotModel

        {
            get { return raw_plotmodel; }
            set
            {
                raw_plotmodel = value;
                OnPropertyChanged("Raw_PlotModel");
            }
        }

        public ChartsSummaryVM()
        {
            FFT_Plotmodel.Axes.Add(new LogarithmicAxis() { Position = AxisPosition.Bottom, Title = "Frequency, Hz", MajorGridlineStyle = LineStyle.Dot });
            FFT_Plotmodel.Axes.Add(new LinearAxis() { Position = AxisPosition.Left, Title = "Displacement, m", MajorGridlineStyle = LineStyle.Dot });

            Raw_PlotModel.Axes.Add(new LinearAxis() { Position = AxisPosition.Bottom, Title = "Time, s", MajorGridlineStyle = LineStyle.Dot });
            Raw_PlotModel.Axes.Add(new LinearAxis() { Position = AxisPosition.Left, Title = "Displacement, m", MajorGridlineStyle = LineStyle.Dot });
        }

        private void SetDataToPlots(MeasurementsVM measurements)
        {
            LineSeries fft_series = new LineSeries();
            fft_series.Points.AddRange(measurements.FFT);
            FFT_Plotmodel.Series.Add(fft_series);


            LineSeries raw_series = new LineSeries();
            raw_series.Points.AddRange(measurements.RawData);
            Raw_PlotModel.Series.Add(raw_series);
        }

        private void ResetAxes(Object param)
        {
            try
            {
                PlotModel pm = (PlotModel)param;
                pm.ResetAllAxes();
                pm.InvalidatePlot(true);
            }
            catch (Exception)
            {


            }
        }

        public RelayCommand ResetAxexCommand { get { return new RelayCommand(x => true, ResetAxes); } }
    }
}
