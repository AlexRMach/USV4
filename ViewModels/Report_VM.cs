using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ush4.ViewModels.Disp;
using ush4.Infrastructure;
using System.Runtime.CompilerServices;
using CsvHelper;
using System.Globalization;
using System.IO;
using CsvHelper.Configuration;
using ush4.Models.SIOS;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Xps.Packaging;
using ush4.Views.Windows;
using System.Runtime.InteropServices;
using System.Windows.Interop;

using DSPLib;
using System.Numerics;
using MathNet.Numerics;

namespace ush4.ViewModels
{
    public class Report_VM : ViewModel
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetActiveWindow();

        public PlotModel RepRawPlotModel { get; private set; }
        public PlotModel RepFFTPlotModel { get; private set; }

        private double targetFrequency;
        public double TargetFrequency
        {
            get { return targetFrequency; }
            set
            {
                targetFrequency = value;
                OnPropertyChanged("TargetFrequency");
            }
        }


        private Double targetDispl;
        public Double TargetDispl
        {
            get { return targetDispl; }
            set
            {
                targetDispl = value;
                OnPropertyChanged("TargetDispl");
            }
        }

        private Double targetVel;
        public Double TargetVel
        {
            get { return targetVel; }
            set
            {
                targetVel = value;
                OnPropertyChanged("TargetVel");
            }
        }

        private Double targetAcc;
        public Double TargetAcc
        {
            get { return targetAcc; }
            set
            {
                targetAcc = value;
                OnPropertyChanged("TargetAcc");
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

        private DataPoint[] _repFftData;
        public DataPoint[] RepFftData
        {
            get { return _repFftData; }
            private set
            {
                _repFftData = value;
                OnPropertyChanged("RepFftData");
            }
        }

        private DataPoint[] _repDftData;
        public DataPoint[] RepDftData
        {
            get { return _repDftData; }
            private set
            {
                _repDftData = value;
                OnPropertyChanged("RepDftData");
            }
        }

        private string _rep_caption;
        public string RepCaption
        {
            get { return _rep_caption; }
            set
            {
                _rep_caption = value;
                OnPropertyChanged("RepCaption");
            }
        }

        private string _rep_path_pdf;
        public string RepPathPdf
        {
            get { return _rep_path_pdf; }
            set
            {
                _rep_path_pdf = value;
                OnPropertyChanged("RepPathPdf");
            }
        }

        public Double MaxSigma { get; set; }
        /*
        private Double[] CalcFFT_from_SiosRawData()
        {
            return Instruments.FFTCalculation.FFT_Magnitude<DataPoint>(SiosAccRawData, x => x.Y);
        }
        */

        Double time_gap_s = 1 / 1000;
        /*
        DataPoint[] SiosAccRawData = new DataPoint[131072 * 4];

        DataPoint[] RepRawDataArr = new DataPoint[131072 * 4];
        DataPoint[] RepFftDataArr = new DataPoint[131072 * 4];
        DataPoint[] RepDftDataArr = new DataPoint[131072 * 4];
        */


        /*
        функция разделяет массив на элементы с четным/нечетным номером
        на месте. Четные номера собираются впереди исходного массива,
        нечетные - сзади
        mode=0 - разделяет четные/нечетные, все остальное собирает
        */
        /*
        int double_splitevenodd(double[] array, int length, int mode)
        {
            double swapplace;
            int num;

            if((length % 2) > 0) 
            { 
                return 1; 
            }

            if (mode == 0)
            {
                //	case 0:
                for (int i = 0; i < length / 2; i++)
                {
                    num = 2 * i;
                    swapplace = array[num];
                    for (int j = 0; j < i; j++)
                    {
                        array[num] = array[num - 1];
                        num--;
                    };
                    array[num] = swapplace;
                }
                //	break;
            }
            else
            //   default:
            {
                for (int i = length / 2 - 1; i > 0; i--)
                {
                    num = i;
                    swapplace = array[num];
                    for (int j = i; j > 0; j--)
                    {
                        array[num] = array[num + 1];
                        num++;
                    }
                    array[num] = swapplace;
                }

                // break;
            };
            return 0;
        }

        int double_spectrum(int N, double[] in_arr, double[] ampl, double phase)
        {
            double[] inRe, inIm;

            Complex im1;

            int i;
            Complex Fk, Hk, Fn, Hn,
                            Zk, Zn,
                            Gk, Gn,
                            W, Wk, Wn;

            if((N % 2) > 0)
            {
                return 3;
            }

            switch (chkfactorize(N))
            {
                case 1:// too many factors
                    return 1;

                case 2:// too large prime factor
                    return 2;

                default:
                    break;
            }

            double_splitevenodd(in_arr, N, 0); // SPLIT 0
            inRe = in_arr;
            inIm = in_arr + N / 2;

            fft(N / 2, inRe, inIm, ampl, phase);

            double_splitevenodd(in_arr, N, MIX);
            
            W = Complex(cos(2 * M_PI / N), -sin(2 * M_PI / N));

            Wk = W;

            Wn = complex(cos(2 * M_PI * (N / 2 - 1) / N), -sin(2 * M_PI * (N / 2 - 1) / N));

            im1 = complex(0, 1);

            for (i = 1; i < N / 4 + 1; i++)
            {
                Zk = complex(ampl[i], phase[i]);
                Zn = complex(ampl[N / 2 - i], phase[N / 2 - i]);
                Fk = (Zk + conj(Zn)) / 2;
                Hk = (Zk - conj(Zn)) * (-im1) / 2;
                Fn = (Zn + conj(Zk)) / 2;
                Hn = (Zn - conj(Zk)) * (-im1) / 2;
                Gk = Fk + Wk * Hk;
                Gn = Fn + Wn * Hn;
                ampl[i] = abs(Gk) * 2 / N;
                phase[i] = arg(Gk);
                ampl[N / 2 - i] = abs(Gn) * 2 / N;
                phase[N / 2 - i] = arg(Gn);
                Wk = Wk * W;
                Wn = Wn / W;
            }

            // восстановим 0-й и N/2-й элементы комплексного выходного массива
            ampl[N / 2] = ampl[0] - phase[0];
            phase[N / 2] = 0;
            if (ampl[N / 2] < 0)
            {
                ampl[N / 2] = -ampl[N / 2];
                phase[N / 2] = M_PI;
            }
            ampl[N / 2] = ampl[0] + phase[0];
            phase[0] = 0;
            if (ampl[0] < 0)
            {
                ampl[0] = -ampl[0];
                phase[0] = M_PI;
            }
            // В результате имеем односторонний спектр
            return 0;
        }
        */

        const bool useFft = true;

        public Report_VM(double time_gap_s_, double target_freq, double target_displ, double target_vel, double target_acc, double load_weight, double[] arr, int size, DateTime start_time, string path) 
        {
            int plot_points;

            double plot_time;

            int ft_to_save_cnt = 0;

            String SiosRawDataFilePath;
            String SiosFftDataFilePath;

            DataPoint[] SiosAccRawData = new DataPoint[size];

            DataPoint[] RepRawDataArr = new DataPoint[size];
            DataPoint[] RepFftDataArr = new DataPoint[size];
            //DataPoint[] RepDftDataArr = new DataPoint[size];

            //RepCaption = String.Format("{0}_F{1}_X{2}", DateTime.Now.ToString("ddMMyyyy_HHmmss"), target_freq, target_displ);
            if (load_weight > 0)
            {
                RepCaption = String.Format("{0}_F{1}_X{2}_W{3}", start_time.ToString("ddMMyyyy_HHmmss"), target_freq, target_displ, load_weight);
            }        
            else
            {
                RepCaption = String.Format("{0}_F{1}_X{2}", start_time.ToString("ddMMyyyy_HHmmss"), target_freq, target_displ);
            }

            RepRawPlotModel = new PlotModel();

            //SiosRawPlotModel.Title = "Sios Raw";

            RepRawPlotModel.Axes.Add(new LinearAxis()
            {
                Position = AxisPosition.Left,
                Title = "Amplitude, mm",
                MajorGridlineStyle = LineStyle.Dot,
                AbsoluteMinimum = 0,
                AbsoluteMaximum = 20,
                StringFormat = "0.000"
            });

            RepRawPlotModel.Axes.Add(new LinearAxis()
            {
                Position = AxisPosition.Bottom,
                Title = "Time, s",
                MajorGridlineStyle = LineStyle.Dot,
                AbsoluteMinimum = 0,
                AbsoluteMaximum = 20,
                StringFormat = "0.000"
            });

            var SiosRawSeries = new LineSeries { Title = "Sios Data", MarkerType = MarkerType.None };

            //PlotModel.Series.Add(new LineSeries { LineStyle = LineStyle.Solid });
            RepRawPlotModel.Series.Add(SiosRawSeries);

            var SiosMarkSeries = new LineSeries { MarkerType = MarkerType.None };

            RepRawPlotModel.Series.Add(SiosMarkSeries);


            RepFFTPlotModel = new PlotModel();

            //SiosFFTPlotModel.Title = "Sios FFT";

            RepFFTPlotModel.Axes.Add(new LinearAxis()
            {
                Position = AxisPosition.Left,
                Title = "Amplitude, mm",
                MajorGridlineStyle = LineStyle.Dot,
                AbsoluteMinimum = 0,
                AbsoluteMaximum = 20,
                StringFormat = "0.000"
            });

            RepFFTPlotModel.Axes.Add(new LogarithmicAxis()
            {
                Position = AxisPosition.Bottom,
                Title = "Frequency, Hz",
                MajorGridlineStyle = LineStyle.Dot,
                AbsoluteMinimum = 0,
                AbsoluteMaximum = 20,
                StringFormat = "0.0000"
            });

            var SiosFFTSeries = new LineSeries { Title = "FFT", MarkerType = MarkerType.None };

            //PlotModel.Series.Add(new LineSeries { LineStyle = LineStyle.Solid });
            RepFFTPlotModel.Series.Add(SiosFFTSeries);

            var SiosDFTSeries = new LineSeries { Title = "DFT", MarkerType = MarkerType.None };

            RepFFTPlotModel.Series.Add(SiosDFTSeries);

            TargetFrequency = target_freq;
            TargetDispl = target_displ;
            TargetVel = target_vel;
            TargetAcc = target_acc;

            time_gap_s = time_gap_s_;

            plot_time = 2 / (TargetFrequency);

            plot_points = (int)(2 / (TargetFrequency * time_gap_s));

            if (plot_points > SiosAccRawData.Length)
            {
                plot_points = SiosAccRawData.Length;
            }

            SiosAccRawData = Instruments.DataConvertion.ConvertDoubleDataToOxyPoints(arr, x => x * time_gap_s, y => y / 2000000, size);

            //Calculate();

            int typeOfft = 0;            

            switch (typeOfft)
            {
                // FFT 
                case 0:
                    ResultValues.TypeFt = "FFT";

                    Double[] fft_mag = Instruments.FFTCalculation.FFT_Magnitude<DataPoint>(SiosAccRawData, x => x.Y, size);

                    RepFftData = Instruments.DataConvertion.ConvertDoubleDataToOxyPoints
                            (fft_mag, x => (x / (time_gap_s * fft_mag.Length)), y => 2.0 * y / fft_mag.Length, fft_mag.Length / 2);

                    RepFftData[0] = new DataPoint(RepFftData[0].X, RepFftData[0].Y / 2.0);
                    break;

                // DFT direct
                case 1:
                    ResultValues.TypeFt = "DFT dir";

                    Double[] dft_mag = Instruments.FFTCalculation.DFT_Magnitude<DataPoint>(SiosAccRawData, x => x.Y, size);

                    RepFftData = Instruments.DataConvertion.ConvertDoubleDataToOxyPoints
                            (dft_mag, x => (x / (time_gap_s * dft_mag.Length)), y => 2.0 * y / dft_mag.Length, dft_mag.Length / 2);

                    RepFftData[0] = new DataPoint(RepFftData[0].X, RepFftData[0].Y / 2.0);
                    break;

                // DFT Lib
                case 2:
                    ResultValues.TypeFt = "DFT Lib";

                    DFT dft = new DFT();

                    dft.Initialize((uint)size);

                    double[] tempSpectr = new double[size];

                    for (int i = 0; i < size; i++)
                    {
                        tempSpectr[i] = arr[i] / 2000000;
                    }

                    Complex[] cSpectrum = dft.Execute(tempSpectr);

                    double[] lmSpectrum = DSP.ConvertComplex.ToMagnitude(cSpectrum);

                    RepFftData = Instruments.DataConvertion.ConvertDoubleDataToOxyPoints(lmSpectrum, x => (x / (time_gap_s * lmSpectrum.Length * 2)), y => Math.Sqrt(2) * y, lmSpectrum.Length);

                    RepFftData[0] = new DataPoint(RepFftData[0].X, RepFftData[0].Y / 2.0);
                    break;

                // MFT
                case 3:
                    ResultValues.TypeFt = "MFT";

                    Double[] mft_mag = Instruments.FFTCalculation.MFT_Magnitude<DataPoint>(SiosAccRawData, x => x.Y, size);

                    RepFftData = Instruments.DataConvertion.ConvertDoubleDataToOxyPoints
                                (mft_mag, x => (x / (time_gap_s * mft_mag.Length)), y => y, mft_mag.Length / 2);

                    RepFftData[0] = new DataPoint(RepFftData[0].X, RepFftData[0].Y / mft_mag.Length);
                    break;
            }            

            MaxSigma = 0.1;

            int freq_index = FindIndexOfFrequency(RepFftData, TargetFrequency, time_gap_s, MaxSigma);

            ResultValues.SetMeasuredValue(RepFftData[freq_index].X, RepFftData[freq_index].Y);

            for (int i = 0; i < RepFftData.Length; i++)
            {
                RepFftDataArr[i] = RepFftData[i];

                if (RepFftData[i].X > 100)
                {
                    ft_to_save_cnt = i;

                    break;
                }
            }

            DataPoint[] RepFtToSaveArr = new DataPoint[ft_to_save_cnt];

            for (int i = 0; i < ft_to_save_cnt; i++)
            {
                RepFtToSaveArr[i] = RepFftDataArr[i];
            }

            RepRawDataArr = Instruments.DataConvertion.ConvertDoubleDataToOxyPoints(arr, x => x * time_gap_s, y => y / 2000000, plot_points);

            var s = (LineSeries)RepRawPlotModel.Series[0];

            s.Points.Clear();
            s.Points.AddRange(RepRawDataArr);

            var orderedSeries = s.Points.OrderBy(o => o.Y).ToList();

            double highestPoint = orderedSeries.Last().Y;
            double lowestPoint = orderedSeries.First().Y;

            RepRawPlotModel.Axes[0].AbsoluteMaximum = highestPoint;
            RepRawPlotModel.Axes[0].AbsoluteMinimum = lowestPoint;

            RepRawPlotModel.Axes[0].Maximum = highestPoint;
            RepRawPlotModel.Axes[0].Minimum = lowestPoint;

            orderedSeries = s.Points.OrderBy(o => o.X).ToList();

            highestPoint = orderedSeries.Last().X;

            RepRawPlotModel.Axes[1].AbsoluteMaximum = plot_time;
            RepRawPlotModel.Axes[1].Maximum = plot_time;

            RepRawPlotModel.Axes[1].AbsoluteMinimum = 0;
            RepRawPlotModel.Axes[1].Minimum = 0;

            RepRawPlotModel.ResetAllAxes();

            RepRawPlotModel.InvalidatePlot(true);
            
            var s1 = (LineSeries)RepFFTPlotModel.Series[0];

            s1.Points.Clear();
            s1.Points.AddRange(RepFftDataArr);

            orderedSeries = s1.Points.OrderBy(o => o.Y).ToList();

            highestPoint = orderedSeries.Last().Y;
            lowestPoint = orderedSeries.First().Y;

            RepFFTPlotModel.Axes[0].AbsoluteMaximum = highestPoint;
            RepFFTPlotModel.Axes[0].AbsoluteMinimum = lowestPoint;

            RepFFTPlotModel.Axes[0].Maximum = highestPoint;
            RepFFTPlotModel.Axes[0].Minimum = lowestPoint;

            orderedSeries = s1.Points.OrderBy(o => o.X).ToList();

            highestPoint = orderedSeries.Last().X;

            RepFFTPlotModel.Axes[1].AbsoluteMaximum = highestPoint;
            RepFFTPlotModel.Axes[1].Maximum = highestPoint; 
                        
            RepFFTPlotModel.ResetAllAxes();

            RepFFTPlotModel.InvalidatePlot(true);            

            //SiosRawDataFilePath = Path.Combine(path, String.Format("{0}_F{1}_X{2}.csv", start_time.ToString("ddMMyyyy_HHmmss"), TargetFrequency, TargetDispl));
            SiosRawDataFilePath = Path.Combine(path, String.Format("{0}.csv", RepCaption));

            //SiosFftDataFilePath = Path.Combine(path, String.Format("{0}_F{1}_X{2}.csv", "fft_" + start_time.ToString("ddMMyyyy_HHmmss"), TargetFrequency, TargetDispl));
            SiosFftDataFilePath = Path.Combine(path, String.Format("fft_{0}.csv", RepCaption));

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
            };

            // Сохраняем в файл сырые данные
            using (var stream = File.Open(SiosRawDataFilePath, FileMode.OpenOrCreate)) // FileMode.CreateNew |
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteField("");
                csv.WriteField("F, Hz");
                csv.WriteField(TargetFrequency);
                csv.NextRecord();

                csv.WriteField("");
                csv.WriteField("X, m");
                csv.WriteField(TargetDispl);
                csv.NextRecord();

                csv.WriteField("");
                csv.WriteField("V, m/s");
                csv.WriteField(TargetVel);
                csv.NextRecord();

                csv.WriteField("");
                csv.WriteField("A, m/s2");
                csv.WriteField(TargetAcc);
                csv.NextRecord();

                csv.WriteField("");
                csv.WriteField("dT, s");
                csv.WriteField(time_gap_s);
                csv.NextRecord();

                writer.Flush();
            }

            using (var stream = File.Open(SiosRawDataFilePath, FileMode.Append))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                /*
                csv.WriteRecords(arr);
                */
                for (int i = 0; i < size; i++)
                {
                    csv.WriteField(arr[i]);
                    csv.NextRecord();
                }
            }

            using (var stream = File.Open(SiosFftDataFilePath, FileMode.OpenOrCreate)) // FileMode.CreateNew |
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteField("");
                csv.WriteField("F, Hz");
                csv.WriteField(TargetFrequency);
                csv.WriteField(ResultValues.MeasuredFrequency);               
                csv.WriteField(ResultValues.TypeFt);                
                csv.NextRecord();

                csv.WriteField("");
                csv.WriteField("X, m");
                csv.WriteField(TargetDispl);
                csv.WriteField(ResultValues.MeasuredDispl);
                csv.NextRecord();

                csv.WriteField("");
                csv.WriteField("V, m/s");
                csv.WriteField(TargetVel);
                csv.WriteField(ResultValues.MeasuredVel);
                csv.NextRecord();

                csv.WriteField("");
                csv.WriteField("A, m/s2");
                csv.WriteField(TargetAcc);
                csv.WriteField(ResultValues.MeasuredAcc);
                csv.NextRecord();

                csv.WriteField("");
                csv.WriteField("dT, s");
                csv.WriteField(time_gap_s);
                csv.NextRecord();

                writer.Flush();

                if (useFft)
                {
                    csv.WriteRecords(RepFtToSaveArr);
                }
                else
                {
                    csv.WriteRecords(RepFtToSaveArr);
                }
            }

            RepPathPdf = Path.Combine(path, String.Format("rep_{0}.pdf", RepCaption));
            /*
           SiosPdfDataFilePath = Path.Combine(path, String.Format("rep_{0}_F{1}_X{2}.pdf", start_time.ToString("ddMMyyyy_HHmmss"), TargetFrequency, TargetDispl));

           FixedDocument fixedDoc = new FixedDocument();

           PageContent pageContent = new PageContent();
           FixedPage fixedPage = new FixedPage();

           IntPtr active = GetActiveWindow();

           ReportView ActiveWindow = App.Current.Windows.OfType<ReportView>()
               .SingleOrDefault(window => new WindowInteropHelper(window).Handle == active);


           HwndSource hwndSource = HwndSource.FromHwnd(active);
           var ActiveWindow = hwndSource?.RootVisual as ReportView;

           Grid canvas = new Grid();
           //canvas = App.Current.Windows[0].FindName("report") as Grid;
           //canvas = App.Current.Windows.OfType<ReportView>().SingleOrDefault(x => x.IsActive).FindName("report") as Grid;
           canvas = ActiveWindow.FindName("report") as Grid;

           string tempFilename = "temp.xps";
           File.Delete(tempFilename);
           XpsDocument xpsd = new XpsDocument(tempFilename, FileAccess.ReadWrite);
           System.Windows.Xps.XpsDocumentWriter xw = XpsDocument.CreateXpsDocumentWriter(xpsd);
           xw.Write(canvas);
           xpsd.Close();
           PdfSharp.Xps.XpsConverter.Convert(tempFilename, SiosPdfDataFilePath, 1);
           */
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
    }
}
