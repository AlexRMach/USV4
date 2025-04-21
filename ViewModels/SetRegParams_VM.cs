using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.WPF.ViewModels;
using ush4.Infrastructure;
using ush4.ViewModels.SetPoint;

namespace ush4.ViewModels
{
    public class SetRegParams_VM : ViewModel
    {        
        private int _rate;
        public int SiosRate
        {
            get { return _rate; }
            set
            {
                _rate = value;
                OnPropertyChanged("SiosRate");

                RegVars.Rate = _rate;
            }        
        }
        
        private int _num_of_reg_periods;
        public int NumOfRegPeriods
        {
            get { return _num_of_reg_periods; }
            set
            {
                _num_of_reg_periods = value;
                OnPropertyChanged("NumOfRegPeriods");

                if (RegVars.Freq != 0)
                {
                    TimeOfReg = _num_of_reg_periods / RegVars.Freq;
                }
                else
                {
                    TimeOfReg = 0;
                }

                RegVars.NumOfPeriods = _num_of_reg_periods;
            }
        }

        private double _time_of_reg;
        public double TimeOfReg
        {
            get { return _time_of_reg; }
            set
            {
                _time_of_reg = value;
                OnPropertyChanged("TimeOfReg");
            }
        }

        private double _freq;
        public double Freq
        {
            get { return _freq; }
            set
            {
                _freq = value;
                OnPropertyChanged("Freq");
            }
        }

        /*
        private SetPointValue_VM _rate = new SetPointValue_VM();
        public SetPointValue_VM SiosRate
        {
            get { return _rate; }
            set
            {
                _rate = value;
                OnPropertyChanged("SiosRate");
            }
        }
        */


        public SetRegParams_VM()
        {
            //SiosRate.Value = 1000;

            NumOfRegPeriods = 20;

            TimeOfReg = 5;

            SiosRate = 1000;            
        }
    }
}
