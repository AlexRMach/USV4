using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ush4.Models;
using Common.WPF.ViewModels;

namespace ush4.ViewModels.Results
{
    public class SetAndMeasureedValueVM : ViewModel
    {
        private Borders.enSetPointType value_type;

        public Borders.enSetPointType ValueType
        {
            get { return value_type; }
            set
            {
                value_type = value;
                OnPropertyChanged("ValueType");
            }
        }


        private Double set_value;

        public Double SetValue
        {
            get { return set_value; }
            set
            {
                set_value = value;
                OnPropertyChanged("SetValue");
            }
        }

        private Double measured_value;

        public Double MeasuredValue
        {
            get { return measured_value; }
            set
            {
                measured_value = value;
                OnPropertyChanged("MeasuredValue");
            }
        }

        private String units = "";
        public String Units
        {
            get { return units; }
            set
            {
                units = value;
                OnPropertyChanged("Units");
            }
        }


        public override string ToString()
        {
            String d_e = "e5";
            return String.Format("|{0, 10}   {1, 10}  {2, 5}|", SetValue.ToString(d_e), MeasuredValue.ToString(d_e), Units);
        }

    }
}
