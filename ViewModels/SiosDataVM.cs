using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ush4.ViewModels
{
    public class  SiosDataVM
    {
        ObservableCollection<double[]> _sios_raw_data_arr;
        public ObservableCollection<double[]> SiosDataRawArr
        {
            get => _sios_raw_data_arr;
            set
            {
                _sios_raw_data_arr = value;
            }
        }

        ObservableCollection<double> _sios_data_raw;
        public ObservableCollection<double> SiosDataRaw
        {
            get => _sios_data_raw;
            set
            {
                _sios_data_raw = value;
            }
        }
        public SiosDataVM()
        {
            SiosDataRaw = new ObservableCollection<double>();
            SiosDataRawArr = new ObservableCollection<double[]>();
        }
    }
}
