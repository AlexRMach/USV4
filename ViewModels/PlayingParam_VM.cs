using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using Common.WPF.ViewModels;

namespace ush4.ViewModels
{
    public class PlayingParam_VM : ViewModel
    {
        private int _param;
        public int PlayParam
        {
            get { return _param; }
            set
            {
                _param = value;
                OnPropertyChanged("PlayParam");
            }
        }

        private double _param_val;
        public double PlayParamValue
        {
            get { return _param_val; }
            set
            {
                _param_val = value;
                OnPropertyChanged("PlayParamValue");
            }
        }

        private string _param_name;
        public string ParamName
        {
            get { return _param_name; }
            set
            {
                _param_name = value;
                OnPropertyChanged("ParamName");
            }
        }

        public PlayingParam_VM() 
        {
            ParamName = string.Concat("A, m/s", '\u00B2');

            PlayParam = 0;

            PlayParamValue = 0;
        }
    }
}
