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

        private string measuredDispl_str;
        public string MeasuredDisplStr
        {
            get { return measuredDispl_str; }
            set
            {
                measuredDispl_str = value;
                OnPropertyChanged("MeasuredDisplStr");
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

        private string measuredVel_str;
        public string MeasuredVelStr
        {
            get { return measuredVel_str; }
            set
            {
                measuredVel_str = value;
                OnPropertyChanged("MeasuredVelStr");
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

        private string measuredAcc_str;
        public string MeasuredAccStr
        {
            get { return measuredAcc_str; }
            set
            {
                measuredAcc_str = value;
                OnPropertyChanged("MeasuredAccStr");
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
        

        private Double measuredErr;
        public Double MeasuredErr
        {
            get { return measuredErr; }
            set
            {
                measuredErr = value;
                OnPropertyChanged("MeasuredErr");
            }
        }

        public void SetMeasuredValue(Double frequency_Hz, Double displacement_m, Double target_displ)
        {
            MeasuredFrequency = frequency_Hz;

            MeasuredDispl = displacement_m * 0.001 *
                Instruments.SimpleCalculations.DerivationCoeffOfSinusoidal(frequency_Hz, Borders.enSetPointType.Displacement - Borders.enSetPointType.Displacement);

            MeasuredVel = displacement_m * 0.001 *
                Instruments.SimpleCalculations.DerivationCoeffOfSinusoidal(frequency_Hz, Borders.enSetPointType.Velocity - Borders.enSetPointType.Displacement);

            MeasuredAcc = displacement_m * 0.001 *
                Instruments.SimpleCalculations.DerivationCoeffOfSinusoidal(frequency_Hz, Borders.enSetPointType.Acceleration - Borders.enSetPointType.Displacement);

            MeasuredErr = 100 - (target_displ / MeasuredDispl) * 100;

            string str_fmt = "0.######";

            if (target_displ >= 0.0001)
            {
                str_fmt = "0.#######";
            }
            else
            {
                if (target_displ >= 0.00001)
                {
                    str_fmt = "0.########";
                }
                else
                {
                    if (target_displ >= 0.000001)
                    {
                        str_fmt = "0.#########";
                    }                    
                }
            }
            
            MeasuredDisplStr = MeasuredDispl.ToString(str_fmt);
            MeasuredVelStr = MeasuredVel.ToString(str_fmt + "#");
            MeasuredAccStr = MeasuredAcc.ToString(str_fmt + "#");
        }

        public ResultValues_VM()
        {
            //MeasuredFrequency = 0.1234567890123;
        }
    }
}
