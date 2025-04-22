using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ush4.Models;
using Common.WPF.ViewModels;
using System.Windows.Input;
using ush4.Infrastructure.Commands;
using System.Windows.Controls;
using System.Windows.Media;
using ush4.Infrastructure;

namespace ush4.ViewModels.SetPoint
{
    public class SetPoint_VM : ViewModel
    {
        public event Action<Object> SetPointChanged;

        public Models.Borders Borders { get; set; }

        private String units;
        public String Units
        {
            get { return units; }
            set
            {
                units = value;
                SetNewDisplacementUnits(value);
                OnPropertyChanged("DisplacementUnits");
            }
        }


        private void SetNewDisplacementUnits(String units)
        {
            //Displacement.Units = units;
            //Velocity.Units = String.Format(USH_C.Properties.Resources.Velocity_format, units);
            //Acceleration.Units = String.Format(USH_C.Properties.Resources.accleleration_format, units);
            // if(AmpsDict)

            foreach (var item in AmpsDict)
            {
                item.Value.Units = Instruments.DataConvertion.TypeToUnitConvertion(item.Key, units);
            }
        }

        private SetPointValue_VM fr = new SetPointValue_VM() { SetPointType = Models.Borders.enSetPointType.Frequency };
        public SetPointValue_VM Frequency
        {
            get { return fr; }
            set
            {
                fr = value;
                OnPropertyChanged("Frequency");
            }
        }


        private SetPointValue_VM disp = new SetPointValue_VM() { SetPointType = Models.Borders.enSetPointType.Displacement };
        public SetPointValue_VM Displacement
        {
            get { return disp; }
            set
            {
                disp = value;
                OnPropertyChanged("Displacement");
            }
        }

        private SetPointValue_VM vel = new SetPointValue_VM() { SetPointType = Models.Borders.enSetPointType.Velocity };
        public SetPointValue_VM Velocity
        {
            get { return vel; }
            set
            {
                vel = value;
                OnPropertyChanged("Velocity");
            }
        }


        private SetPointValue_VM acc = new SetPointValue_VM() { SetPointType = Models.Borders.enSetPointType.Acceleration };
        public SetPointValue_VM Acceleration
        {
            get { return acc; }
            set
            {
                acc = value;
                OnPropertyChanged("Acceleration");
            }
        }

        private Dictionary<Borders.enSetPointType, SetPointValue_VM> AmpsDict = new Dictionary<Borders.enSetPointType, SetPointValue_VM>();

        private Brush _color_vel;
        public Brush ColorVel
        {
            get => _color_vel;
            set
            {
                _color_vel = value;
                OnPropertyChanged("ColorVel");
            }
        }

        private Brush _color_displ;
        public Brush ColorDispl
        {
            get => _color_displ;
            set
            {
                _color_displ = value;
                OnPropertyChanged("ColorDispl");
            }
        }

        private Brush _color_accel;
        public Brush ColorAccel
        {
            get => _color_accel;
            set
            {
                _color_accel = value;
                OnPropertyChanged("ColorAccel");
            }
        }


        public SetPoint_VM()
        {
            AmpsDict.Add(Borders.enSetPointType.Frequency, Frequency);
            AmpsDict.Add(Borders.enSetPointType.Displacement, Displacement);
            AmpsDict.Add(Borders.enSetPointType.Velocity, Velocity);
            AmpsDict.Add(Borders.enSetPointType.Acceleration, Acceleration);
            foreach (var item in AmpsDict)
            {
                item.Value.ValueChanged += ValueChangedHandler;
            }

            SetFreqAsExecParamCommand = new LambdaCommand(OnSetFreqAsExecParamCommandExecuted, CanSetFreqAsExecParamCommandExecute);
            SetVelocAsExecParamCommand = new LambdaCommand(OnSetVelocAsExecParamCommandExecuted, CanSetVelocAsExecParamCommandExecute);
            SetAccelAsExecParamCommand = new LambdaCommand(OnSetAccelAsExecParamCommandExecuted, CanSetAccelAsExecParamCommandExecute);
            SetDisplAsExecParamCommand = new LambdaCommand(OnSetDisplAsExecParamCommandExecuted, CanSetDisplAsExecParamCommandExecute);

            ColorAccel = new SolidColorBrush(Colors.LightGray);
            ColorVel = new SolidColorBrush(Colors.LightGray);
            ColorDispl = new SolidColorBrush(Colors.LightGray);            
        }


        private void ValueChangedHandler(Object sender, Double value)
        {
            SetPointValue_VM setPointSender = (SetPointValue_VM)sender;

            if (setPointSender.SetPointType == Borders.enSetPointType.Frequency)
            {
                RegVars.Freq = value;
            }

            if (CheckAndRecalculate(sender, value))
            {
                if (SetPointChanged != null)
                    SetPointChanged(this);                

                if(setPointSender.SetPointType == Borders.enSetPointType.Displacement)
                {
                    ExecutedParam = 2;

                    ColorAccel = new SolidColorBrush(Colors.LightGray);
                    ColorVel = new SolidColorBrush(Colors.LightGray);
                    ColorDispl = new SolidColorBrush(Colors.LightGreen);
                }
                else
                {
                    if(setPointSender.SetPointType == Borders.enSetPointType.Velocity)
                    {
                        ExecutedParam = 3;

                        ColorAccel = new SolidColorBrush(Colors.LightGray);
                        ColorVel = new SolidColorBrush(Colors.LightGreen);
                        ColorDispl = new SolidColorBrush(Colors.LightGray);
                    }
                    else
                    {
                        if (setPointSender.SetPointType == Borders.enSetPointType.Acceleration)
                        {                            
                            ExecutedParam = 4;

                            ColorAccel = new SolidColorBrush(Colors.LightGreen);
                            ColorVel = new SolidColorBrush(Colors.LightGray);
                            ColorDispl = new SolidColorBrush(Colors.LightGray);
                        }                        
                    }
                }
            }

        }

        private Boolean CheckAndRecalculate(object sender, double value)
        {
            SetPointValue_VM setPointSender = (SetPointValue_VM)sender;
            if (setPointSender.SetPointType == Borders.enSetPointType.Frequency)
            {
                setPointSender.CheckAndSetValue(value, Borders.BordersDict[setPointSender.SetPointType]);

                if ((Frequency.Value != 0) && (Displacement.Value != 0))
                {
                    setPointSender = Displacement;
                    value = Displacement.Value;
                }
                else
                    return false;

            }
            for (int i = (int)Borders.enSetPointType.Displacement; i <= (int)Borders.enSetPointType.Acceleration; i++)
            {
                Borders.enSetPointType enSP_Type = (Borders.enSetPointType)i;
                SetPointValue_VM sp_item = AmpsDict[enSP_Type];
                //if (setPointSender.SetPointType != enSP_Type)
                //{
                if (!sp_item.CheckAndSetValue(value * Instruments.SimpleCalculations.DerivationCoeffOfSinusoidal(Frequency.Value,
                            (int)sp_item.SetPointType - (int)setPointSender.SetPointType), Borders.BordersDict[enSP_Type]))
                {
                    setPointSender = sp_item;
                    value = setPointSender.Value;
                    i = (int)Borders.enSetPointType.Frequency;
                }
                //}
            }
            return true;
        }

        public void CheckValuesAfterDeserialization(Borders borders)
        {
            Borders = borders;
            CheckAndRecalculate(Frequency, Frequency.Value);
        }

        private int _executed_param;

        public int ExecutedParam
        {
            get { return _executed_param; }
            set
            {
                _executed_param = value;
                OnPropertyChanged("ExecutedParam");
            }
        }

        #region SetFreqAsExecParamCommand
        public ICommand SetFreqAsExecParamCommand { get; }

        private bool CanSetFreqAsExecParamCommandExecute(object p) => true;

        private void OnSetFreqAsExecParamCommandExecuted(object p)
        {            
            ExecutedParam = 1;            
        }
        #endregion

        #region SetVelocAsExecParamCommand
        public ICommand SetVelocAsExecParamCommand { get; }

        private bool CanSetVelocAsExecParamCommandExecute(object p) => true;

        private void OnSetVelocAsExecParamCommandExecuted(object p)
        {
            ExecutedParam = 2;
        }
        #endregion

        #region SetAccelAsExecParamCommand
        public ICommand SetAccelAsExecParamCommand { get; }

        private bool CanSetAccelAsExecParamCommandExecute(object p) => true;

        private void OnSetAccelAsExecParamCommandExecuted(object p)
        {
            ExecutedParam = 3;
        }
        #endregion

        #region SetDisplAsExecParamCommand
        public ICommand SetDisplAsExecParamCommand { get; }

        private bool CanSetDisplAsExecParamCommandExecute(object p) => true;

        private void OnSetDisplAsExecParamCommandExecuted(object p)
        {
            ExecutedParam = 4;
        }
        #endregion


        public Boolean IsAllValuesIsOk()
        {
            foreach (var item in AmpsDict)
            {
                if (item.Value.ValueState == SetPointValue_VM.enValueStates.Error)
                    return false;

            }
            return true;
        }
    }
}
