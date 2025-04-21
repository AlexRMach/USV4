using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ush4.Models;
using Common.WPF.ViewModels;

namespace ush4.ViewModels.SetPoint
{
    public class SetPointValue_VM : ViewModel
    {
        public event Action<Object, Double> ValueChanged;



        public enum enValueStates
        {
            Ok,
            NotGood,
            Error
        }

        //private SolidColorBrush brush = StateToBrushDict[enValueStates.Error];
        //public SolidColorBrush Brush
        //{
        //    get { return brush; }
        //    set { brush = value;
        //        OnPropertyChanged("Brush");
        //    }
        //}

        // AlexM 291123

        public SetPointValue_VM()
        {
            Units = "";
        }


        private String units;
        public String Units
        {
            get { return units; }
            set
            {
                units = value;
                OnPropertyChanged("DisplacementUnits");
            }
        }

        public Borders.enSetPointType SetPointType { get; set; }

        private enValueStates valueState = enValueStates.Error;
        public enValueStates ValueState
        {
            get { return valueState; }
            set
            {
                valueState = value;
                //  Brush = StateToBrushDict[value];
                OnPropertyChanged("ValueState");
            }
        }


        private Double _value = 0;
        public Double Value
        {
            get { return _value; }
            set
            {
                if (value == 0)
                    return;

                //_value =  CheckValue(value, Borders);
                _value = value;
                OnPropertyChanged("Value");

                if (ValueChanged != null)
                    ValueChanged(this, _value);
            }
        }

        [JsonIgnore]
        private Double newValue;
        public Double NewValue
        {
            get { return newValue; }
            set
            {

                if (value == 0)
                    return;

                newValue = value;

                OnPropertyChanged("NewValue");

                if (ValueChanged != null)
                    ValueChanged(this, newValue);
            }
        }


        private String tooltipStr = Properties.Resources.The_value_cannot_be_null;
        public String TooltipStr
        {
            get { return tooltipStr; }
            set
            {
                tooltipStr = value;
                OnPropertyChanged("TooltipStr");
            }
        }

        //private Tuple<double, double> borders;// = new Tuple<double, double>(0.01, 100);
        //public Tuple<double, double> Borders
        //{
        //    get { return borders; }
        //    set
        //    {
        //        borders = value;
        //        OnPropertyChanged("Borders");
        //    }
        //}


        public Boolean CheckAndSetValue(Double entered_value, Tuple<double, double> _borders)
        {
            //if (Value == entered_value)
            //    return true;


            _value = CheckValue(entered_value, _borders);
            OnPropertyChanged("Value");

            //newValue = Value;
            //OnPropertyChanged("NewValue");

            if (entered_value != Value)
            {
                //if (ValueChanged != null)
                //    ValueChanged(this, Value);
                return false;
            }
            else
                return true;
        }

        private Double CheckValue(Double entered_value, Tuple<double, double> _borders)
        {
            if (_borders == null)
            {
                ValueState = enValueStates.Error;
                TooltipStr = Properties.Resources.No_borders_were_set;
                return entered_value;
            }
            else if (entered_value == 0)
            {
                ValueState = enValueStates.Error;
                TooltipStr = Properties.Resources.The_value_cannot_be_null;
                return entered_value;
            }
            else if ((entered_value < _borders.Item1))
            {

                ValueState = enValueStates.NotGood;
                TooltipStr = String.Format(Properties.Resources.Value_out_of_borders, entered_value, _borders.Item1, _borders.Item2);
                //if (ValueChanged!= null)
                //    ValueChanged(this, _borders.Item1);
                return _borders.Item1;
            }
            else if ((entered_value > _borders.Item2))
            {

                ValueState = enValueStates.NotGood;
                TooltipStr = String.Format(Properties.Resources.Value_out_of_borders, entered_value, _borders.Item1, _borders.Item2);
                //if (ValueChanged != null)
                //    ValueChanged(this, _borders.Item2);
                return _borders.Item2;
            }
            else
            {
                ValueState = enValueStates.Ok;
                TooltipStr = "";


                return entered_value;
            }
        }
    }
}
