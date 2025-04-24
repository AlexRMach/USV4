using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ush4.Infrastructure;
using Common.WPF.ViewModels;

namespace ush4.ViewModels
{
    public class SetPidParams_VM : ViewModel
    {
        private double _Kp1_start;
        public double Kp1_start
        {
            get { return _Kp1_start; }
            set
            {
                _Kp1_start = value;
                OnPropertyChanged("Kp1_start");                
            }
        }

        private double _Ki1_start;
        public double Ki1_start
        {
            get { return _Ki1_start; }
            set
            {
                _Ki1_start = value;
                OnPropertyChanged("Ki1_start");                
            }
        }

        private double _Kp2_start;
        public double Kp2_start
        {
            get { return _Kp2_start; }
            set
            {
                _Kp2_start = value;
                OnPropertyChanged("Kp2_start");                
            }
        }

        private double _Ki2_start;
        public double Ki2_start
        {
            get { return _Ki2_start; }
            set
            {
                _Ki2_start = value;
                OnPropertyChanged("Ki2_start");                
            }
        }

        private double _kp1_steady;
        public double Kp1_steady
        {
            get { return _kp1_steady; }
            set
            {
                _kp1_steady = value;
                OnPropertyChanged("Kp1_steady");                
            }
        }

        private double _ki1_steady;
        public double Ki1_steady
        {
            get { return _ki1_steady; }
            set
            {
                _ki1_steady = value;
                OnPropertyChanged("Ki1_steady");                
            }
        }

        private double _kp2_steady;
        public double Kp2_steady
        {
            get { return _kp2_steady; }
            set
            {
                _kp2_steady = value;
                OnPropertyChanged("Kp2_steady");                
            }
        }

        private double _ki2_steady;
        public double Ki2_steady
        {
            get { return _ki2_steady; }
            set
            {
                _ki2_steady = value;
                OnPropertyChanged("Ki2_steady");                
            }
        }

        private double _kp3_start;
        public double Kp3_start
        {
            get { return _kp3_start; }
            set
            {
                _kp3_start = value;
                OnPropertyChanged("Kp3_start");
            }
        }

        private double _kp3_steady;
        public double Kp3_steady
        {
            get { return _kp3_steady; }
            set
            {
                _kp3_steady = value;
                OnPropertyChanged("Kp3_steady");
            }
        }

        public SetPidParams_VM()
        {
            Kp1_start = 40;
            Ki1_start = 1e-6;

            Kp2_start = 3e-7;
            Ki2_start = 1e-6;            

            Kp1_steady = 40;
            Ki1_steady = 1e-6;

            Kp2_steady = 3e-7;
            Ki2_steady = 1e-6;

            Kp3_start = 250;
            Kp3_steady = 250;            
        }
    }
}
