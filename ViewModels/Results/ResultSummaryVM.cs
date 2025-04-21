using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ush4.Models;
using Common.WPF.ViewModels;

namespace ush4.ViewModels.Results
{
    public class ResultSummaryVM : ViewModel
    {
        private ObservableCollection<SetAndMeasureedValueVM> result_collection = new ObservableCollection<SetAndMeasureedValueVM>();

        public ObservableCollection<SetAndMeasureedValueVM> Summary
        {
            get { return result_collection; }
            private set
            {
                result_collection = value;
                OnPropertyChanged("Summary");
            }
        }

        public ResultSummaryVM()
        {
            TypeToResultDict = new Dictionary<Borders.enSetPointType, SetAndMeasureedValueVM>();
            foreach (var item in Enum.GetValues(typeof(Borders.enSetPointType)))
            {
                TypeToResultDict.Add((Borders.enSetPointType)item, new SetAndMeasureedValueVM());
            }
        }

        public Dictionary<Borders.enSetPointType, SetAndMeasureedValueVM> TypeToResultDict { get; private set; }


        public void SetSetValue(Double frequency_Hz, Double displacement_m, String displacement_units)
        {
            if (Summary.Count > 0)
                throw new Exception(Properties.Resources.SummarySetValueEx);

            foreach (var item in Enum.GetValues(typeof(Borders.enSetPointType)))
            {
                SetAndMeasureedValueVM to_add = new SetAndMeasureedValueVM()
                {
                    ValueType = (Borders.enSetPointType)item,
                    Units = Instruments.DataConvertion.TypeToUnitConvertion((Borders.enSetPointType)item, displacement_units)
                };

                //TypeToResultDict.Add(to_add.ValueType, to_add);

                TypeToResultDict[to_add.ValueType] = to_add;

                if (to_add.ValueType == Borders.enSetPointType.Frequency)
                    to_add.SetValue = frequency_Hz;
                else
                {
                    to_add.SetValue = displacement_m *
                        Instruments.SimpleCalculations.DerivationCoeffOfSinusoidal(frequency_Hz, to_add.ValueType - Borders.enSetPointType.Displacement);
                }

                Summary.Add(to_add);
            }
        }


        public void SetMeasuredValue(Double frequency_Hz, Double displacement_m)
        {


            foreach (var item in Summary)
            {
                if (item.MeasuredValue != 0)
                    throw new Exception(Properties.Resources.SummaryMeasuredValueEx);

                if (item.ValueType == Borders.enSetPointType.Frequency)
                    item.MeasuredValue = frequency_Hz;
                else
                {
                    item.MeasuredValue = displacement_m *
                        Instruments.SimpleCalculations.DerivationCoeffOfSinusoidal(frequency_Hz, item.ValueType - Borders.enSetPointType.Displacement);
                }


            }
        }



    }
}
