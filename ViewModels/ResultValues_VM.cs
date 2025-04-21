using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.WPF.ViewModels;
using ush4.Models;
using ush4.ViewModels.Results;

namespace ush4.ViewModels
{
    public class ResultValues_VM : ViewModel
    {
        private double measuredFrequency;
        public double MeasuredFrequency
        {
            get { return measuredFrequency; }
            set
            {
                measuredFrequency = value;
                OnPropertyChanged("MeasuredFrequency");
            }
        }


        private Double measuredDispl;
        public Double MeasuredDispl
        {
            get { return measuredDispl; }
            set
            {
                measuredDispl = value;
                OnPropertyChanged("MeasuredDispl");
            }
        }

        private Double measuredVel;
        public Double MeasuredVel
        {
            get { return measuredVel; }
            set
            {
                measuredVel = value;
                OnPropertyChanged("MeasuredVel");
            }
        }

        private Double measuredAcc;
        public Double MeasuredAcc
        {
            get { return measuredAcc; }
            set
            {
                measuredAcc = value;
                OnPropertyChanged("MeasuredAcc");
            }
        }

        private string _type_ft;
        public string TypeFt
        {
            get { return _type_ft; }
            set
            {
                _type_ft = value;
                OnPropertyChanged("TypeFt");
            }
        }

        public void SetMeasuredValue(Double frequency_Hz, Double displacement_m)
        {
            MeasuredFrequency = frequency_Hz;

            MeasuredDispl = displacement_m * 0.001 *
                Instruments.SimpleCalculations.DerivationCoeffOfSinusoidal(frequency_Hz, Borders.enSetPointType.Displacement - Borders.enSetPointType.Displacement);

            MeasuredVel = displacement_m * 0.001 *
                Instruments.SimpleCalculations.DerivationCoeffOfSinusoidal(frequency_Hz, Borders.enSetPointType.Velocity - Borders.enSetPointType.Displacement);

            MeasuredAcc = displacement_m * 0.001 *
                Instruments.SimpleCalculations.DerivationCoeffOfSinusoidal(frequency_Hz, Borders.enSetPointType.Acceleration - Borders.enSetPointType.Displacement);
        }

        public ResultValues_VM()
        {
            //MeasuredFrequency = 0.1234567890123;
        }
    }
}
