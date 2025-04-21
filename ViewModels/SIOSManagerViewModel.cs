using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Markup;
using ush4.Models.SIOS;
using ush4.ViewModels.Base;

namespace ush4.ViewModels
{
    public class SIOSManagerViewModel
    {
        SIOSManagerModel _siosManagerModel = new Models.SIOS.SIOSManagerModel();

        public double[] GetRecordingData()
        {
            double[] sios_data = new double[50];

            return _siosManagerModel.GetLenghtValues();
            /*
            for (var x = 0d; x <= 49; x += 0.1)
            {
                sios_data[(int)x] = (double)x * 2;
            }

            return sios_data;
            */
        }

        public async Task<DataPoint[]> DownloadRecorderedData()
        {
            var data_ready = await Task.Run<Boolean>((Func<Boolean>)WaitRecorderedDataIsReady);
            if (data_ready)
            {                
                double[] recordingData = await Task.Run<double[]>((Func<double[]>)GetRecordingData);
                //DataPoint[] dataPoints = <OxyPlot.DataPoint>((int)(360 / 0.1));
                if (recordingData != null)
                {
                    DataPoint[] dataPoints = Instruments.DataConvertion.ConvertDoubleDataToOxyPoints(recordingData, x => x * 1, y => y / 1);

                    return dataPoints;
                }
                return null;
                
            }
            throw new Exception("Failed to download data from recorder.");
        }

        protected static Boolean WaitRecorderedDataIsReady()
        {
            Thread.Sleep(1000);
            return true;
        }        

       
    }
}
