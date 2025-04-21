using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using ush4.Models;
using ush4.ViewModels;
using ush4.ViewModels.Results;

namespace ush4.Instruments
{
    public static class DataConvertion
    {
        public static DataPoint[] ConvertTDataToOxyPoints<T>(T[] data, Func<T, Double> XFunc, Func<T, Double> YFunc, int lenght = 0, int offset = 0)
        {
            int l = lenght;
            if (lenght == 0)
                l = data.Count();

            DataPoint[] to_ret = new DataPoint[l];
            for (int i = 0; i < l; i++)
            {
                to_ret[i] = new DataPoint(XFunc(data[i + offset]), YFunc(data[i + offset]));
            }
            return to_ret;
        }

        public static DataPoint[] ConvertDoubleDataToOxyPoints(Double[] data, Func<int, Double> XFunc, Func<Double, Double> YFunc, int lenght = 0, int offset = 0)
        {
            int l = lenght;
            if (lenght == 0)
                l = data.Count();

            if (l != 0)
            {
                DataPoint[] to_ret = new DataPoint[l];
                for (int i = 0; i < l; i++)
                {
                    to_ret[i] = new DataPoint(XFunc(i), YFunc(data[i + offset]));
                }

                return to_ret;
            }
            return null;
        }


        public static HDF5_Structs.Point[] ConvertOxyPointToHDFPoint(DataPoint[] oxypoints)
        {
            int l = oxypoints.Count();
            HDF5_Structs.Point[] points = new HDF5_Structs.Point[l];

            for (int i = 0; i < l; i++)
            {
                points[i] = new HDF5_Structs.Point() { X = oxypoints[i].X, Y = oxypoints[i].Y };
            }
            return points;
        }
        
        public static unsafe HDF5_Structs.SummaryValue[] SummaryVMToHDFSummary(ResultSummaryVM resultSummaryVM)
        {
            List<HDF5_Structs.SummaryValue> summaryValues = new List<HDF5_Structs.SummaryValue>();
            foreach (var item in resultSummaryVM.Summary)
            {
                summaryValues.Add(new HDF5_Structs.SummaryValue()
                {
                    EnSetPointType = item.ValueType,
                    SetValue = item.SetValue,
                    MeasuredValue = item.MeasuredValue,
                    Units = (char*)Marshal.StringToHGlobalAnsi(item.Units)
                });
            }
            return summaryValues.ToArray();
        }        

        public static String TypeToUnitConvertion(Borders.enSetPointType enSetPointType, String unit)
        {
            switch (enSetPointType)
            {
                case Borders.enSetPointType.Frequency:
                    return "Hz";
                // break;
                case Borders.enSetPointType.Displacement:
                    return unit;
                case Borders.enSetPointType.Velocity:
                    return String.Format(ush4.Properties.Resources.Velocity_format, unit);
                case Borders.enSetPointType.Acceleration:
                    return String.Format(ush4.Properties.Resources.accleleration_format, unit); ;
                default:
                    return "";
            }
        }
    }
}
