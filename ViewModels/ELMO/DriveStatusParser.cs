using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ush4.ViewModels.Disp;

namespace ush4.ViewModels.ELMO
{
    public static class DriveStatusParser
    {
        public enum enMotorFaultRegisters
        {
            MainFeedbackError = 1,
            CommProcessFail = 2,
            HallMismatch = 4,
            CurrenPeakLimit = 8,
            ExternalInhibit = 16,
            ACFail = 32,
            HallSpeedTooHigh = 64,
            SpeedTrackingError = 128,
            PositionTrackingError = 256,
            GantryYaw = 1024,
            HeartbeatEvent = 2048,
            Overspeed = 131072,
            MotorStuck = 2097152,
            FeedbackPositionLimit = 4194304,
            NumericOverflow = 8388608,
            GantrySlaveDisabled = 16777216,
            FailedToStart = 536870912,
        }

        public enum enstatusRegisters
        {
            DriveOk = 1,
            Undervoltage = 3,
            Overvoltage = 5,
            Safety = 7,
            ShortProtection = 11,
            Overtemperature = 13,
            ServoEnable = 16,
            MotorFault = 64,
            HomingEvent = 128,
            UserProgram = 4096,
            CurrentLimit = 8192,
            STI1 = 16384,
            STI2 = 32768,
            TargetReached = 262144,
            MotorEnable = 4194304,
            //MotorState = 8388608,
            StopActive = 268435456,
        }

        private static Dictionary<enstatusRegisters, DeviceStateViewModel> StatusRegister_OnlySet_Dict =
            new Dictionary<enstatusRegisters, DeviceStateViewModel>()
            {
                {enstatusRegisters.Undervoltage, new DeviceStateViewModel()
                { DeviceState = DeviceStateViewModel.enDeviceStates.Error, StateDescription = Properties.ResourcesE.Undervoltage} },
                {enstatusRegisters.Overvoltage, new DeviceStateViewModel()
                { DeviceState = DeviceStateViewModel.enDeviceStates.Error, StateDescription = Properties.ResourcesE.Overvoltage} },
                {enstatusRegisters.ShortProtection, new DeviceStateViewModel()
                { DeviceState = DeviceStateViewModel.enDeviceStates.Error, StateDescription = Properties.ResourcesE.ShortProtection} },
                {enstatusRegisters.Overtemperature, new DeviceStateViewModel()
                { DeviceState = DeviceStateViewModel.enDeviceStates.Error, StateDescription = Properties.ResourcesE.Overtemperature} },
                {enstatusRegisters.Safety, new DeviceStateViewModel()
                { DeviceState = DeviceStateViewModel.enDeviceStates.Warning, StateDescription = Properties.ResourcesE.Safety} },
            };

        private static Dictionary<enstatusRegisters, DeviceStateViewModel> StatusRegister_Set_Dict =
            new Dictionary<enstatusRegisters, DeviceStateViewModel>()
            {
                {enstatusRegisters.MotorFault, new DeviceStateViewModel()
                { DeviceState = DeviceStateViewModel.enDeviceStates.Error, StateDescription = Properties.ResourcesE.MotorFault} },
                {enstatusRegisters.HomingEvent, new DeviceStateViewModel()
                { DeviceState = DeviceStateViewModel.enDeviceStates.Work, StateDescription = Properties.ResourcesE.HomingEvent} },
                {enstatusRegisters.UserProgram, new DeviceStateViewModel()
                // AlexM
                { DeviceState = DeviceStateViewModel.enDeviceStates.Work, StateDescription = Properties.ResourcesE.UserProgram} },
                {enstatusRegisters.CurrentLimit, new DeviceStateViewModel()
                // AlexM
                { DeviceState = DeviceStateViewModel.enDeviceStates.Warning, StateDescription = Properties.ResourcesE.CurrentLimit} },
                // AlexM 
                {enstatusRegisters.MotorEnable, new DeviceStateViewModel()
                { DeviceState = DeviceStateViewModel.enDeviceStates.Work, StateDescription = Properties.ResourcesE.MotorEnable} },
                //{ DeviceState = DeviceStateViewModel.enDeviceStates.Work, StateDescription = ""} },
                // AlexM
                {enstatusRegisters.StopActive, new DeviceStateViewModel()
                { DeviceState = DeviceStateViewModel.enDeviceStates.Error, StateDescription = Properties.ResourcesE.StopActive} },
                {enstatusRegisters.ServoEnable, new DeviceStateViewModel()
                { DeviceState = DeviceStateViewModel.enDeviceStates.Work, StateDescription = Properties.ResourcesE.ServoEnabled} },
                {enstatusRegisters.TargetReached, new DeviceStateViewModel()
                { DeviceState = DeviceStateViewModel.enDeviceStates.Ok, StateDescription = Properties.ResourcesE.TargetReached} },
                //{enstatusRegisters.MotorState, new DeviceStateViewModel()
                //{ DeviceState = DeviceStateViewModel.enDeviceStates.Work, StateDescription = Properties.Resources.ActiveMovement} },

            };


        private static Dictionary<enstatusRegisters, DeviceStateViewModel> StatusRegister_NotSet_Dict =
            new Dictionary<enstatusRegisters, DeviceStateViewModel>()
            {
                {enstatusRegisters.DriveOk, new DeviceStateViewModel()
                { DeviceState = DeviceStateViewModel.enDeviceStates.Ok, StateDescription = Properties.ResourcesE.DriveOk} },
                {enstatusRegisters.STI1, new DeviceStateViewModel()
                { DeviceState = DeviceStateViewModel.enDeviceStates.Warning, StateDescription = Properties.ResourcesE.STI1} },
                // AlexM
                {enstatusRegisters.MotorEnable, new DeviceStateViewModel()
                { DeviceState = DeviceStateViewModel.enDeviceStates.Ok, StateDescription = Properties.ResourcesE.MotorNotEnable} },
                {enstatusRegisters.ServoEnable, new DeviceStateViewModel()
                { DeviceState = DeviceStateViewModel.enDeviceStates.Ok, StateDescription = Properties.ResourcesE.ServoNotEnabled} },
                // AlexM
                //{enstatusRegisters.STI2, new DeviceStateViewModel()
                //{ DeviceState = DeviceStateViewModel.enDeviceStates.Warning, StateDescription = Properties.Resources.STI2} },
                //{enstatusRegisters.MotorState, new DeviceStateViewModel()
                //{ DeviceState = DeviceStateViewModel.enDeviceStates.Ok, StateDescription = Properties.Resources.Standstill} },
            };


        private static Dictionary<enMotorFaultRegisters, DeviceStateViewModel> MotorFault_Set_Dict =
            new Dictionary<enMotorFaultRegisters, DeviceStateViewModel>()
            {
                {enMotorFaultRegisters.ACFail, new DeviceStateViewModel()
                { DeviceState = DeviceStateViewModel.enDeviceStates.Error, StateDescription = Properties.ResourcesE.ACFail} },
                {enMotorFaultRegisters.CommProcessFail, new DeviceStateViewModel()
                { DeviceState = DeviceStateViewModel.enDeviceStates.Error, StateDescription = Properties.ResourcesE.CommProcessFail} },
                {enMotorFaultRegisters.CurrenPeakLimit, new DeviceStateViewModel()
                { DeviceState = DeviceStateViewModel.enDeviceStates.Error, StateDescription = Properties.ResourcesE.CurrenPeakLimit} },
                {enMotorFaultRegisters.ExternalInhibit, new DeviceStateViewModel()
                { DeviceState = DeviceStateViewModel.enDeviceStates.Error, StateDescription = Properties.ResourcesE.ExternalInhibit} },
                {enMotorFaultRegisters.FailedToStart, new DeviceStateViewModel()
                { DeviceState = DeviceStateViewModel.enDeviceStates.Error, StateDescription = Properties.ResourcesE.FailedToStart} },
                {enMotorFaultRegisters.FeedbackPositionLimit, new DeviceStateViewModel()
                { DeviceState = DeviceStateViewModel.enDeviceStates.Error, StateDescription = Properties.ResourcesE.FeedbackPositionLimit} },
                {enMotorFaultRegisters.GantrySlaveDisabled, new DeviceStateViewModel()
                { DeviceState = DeviceStateViewModel.enDeviceStates.Error, StateDescription = Properties.ResourcesE.GantrySlaveDisabled} },

                {enMotorFaultRegisters.GantryYaw, new DeviceStateViewModel()
                { DeviceState = DeviceStateViewModel.enDeviceStates.Error, StateDescription = Properties.ResourcesE.GantryYaw} },
                {enMotorFaultRegisters.HallMismatch, new DeviceStateViewModel()
                { DeviceState = DeviceStateViewModel.enDeviceStates.Error, StateDescription = Properties.ResourcesE.HallMismatch} },
                {enMotorFaultRegisters.HallSpeedTooHigh, new DeviceStateViewModel()
                { DeviceState = DeviceStateViewModel.enDeviceStates.Error, StateDescription = Properties.ResourcesE.HallSpeedTooHigh} },
                {enMotorFaultRegisters.HeartbeatEvent, new DeviceStateViewModel()
                { DeviceState = DeviceStateViewModel.enDeviceStates.Error, StateDescription = Properties.ResourcesE.HeartbeatEvent} },

                {enMotorFaultRegisters.MainFeedbackError, new DeviceStateViewModel()
                { DeviceState = DeviceStateViewModel.enDeviceStates.Error, StateDescription = Properties.ResourcesE.MainFeedbackError} },
                {enMotorFaultRegisters.MotorStuck, new DeviceStateViewModel()
                { DeviceState = DeviceStateViewModel.enDeviceStates.Error, StateDescription = Properties.ResourcesE.MotorStuck} },
                {enMotorFaultRegisters.NumericOverflow, new DeviceStateViewModel()
                { DeviceState = DeviceStateViewModel.enDeviceStates.Error, StateDescription = Properties.ResourcesE.NumericOverflow} },
                {enMotorFaultRegisters.Overspeed, new DeviceStateViewModel()
                { DeviceState = DeviceStateViewModel.enDeviceStates.Error, StateDescription = Properties.ResourcesE.Overspeed} },


            };

        public static List<DeviceStateViewModel> ParseStatus(int status)
        {
            List<DeviceStateViewModel> to_ret = new List<DeviceStateViewModel>();
            foreach (var item in Enum.GetValues(typeof(enstatusRegisters)))
            {
                enstatusRegisters enstatus = (enstatusRegisters)item;
                if (IsBitsIsSet(status, (int)item))
                {
                    if (StatusRegister_Set_Dict.ContainsKey(enstatus))
                        to_ret.Add(StatusRegister_Set_Dict[(enstatusRegisters)item]);
                }
                else
                {
                    if (StatusRegister_NotSet_Dict.ContainsKey(enstatus))
                        to_ret.Add(StatusRegister_NotSet_Dict[(enstatusRegisters)item]);
                }

                if (IsOnlyBitsIsSet(status, (int)item))
                {
                    if (StatusRegister_OnlySet_Dict.ContainsKey(enstatus))
                        to_ret.Add(StatusRegister_OnlySet_Dict[(enstatusRegisters)item]);
                }
            }
            return to_ret;
        }


        public static List<DeviceStateViewModel> ParseMotorFault(int mfault)
        {
            List<DeviceStateViewModel> to_ret = new List<DeviceStateViewModel>();
            foreach (var item in Enum.GetValues(typeof(enMotorFaultRegisters)))
            {
                enMotorFaultRegisters enstatus = (enMotorFaultRegisters)item;
                if (IsBitsIsSet(mfault, (int)item))
                {
                    if (MotorFault_Set_Dict.ContainsKey(enstatus))
                        to_ret.Add(MotorFault_Set_Dict[enstatus]);
                }
            }
            return to_ret;
        }

        public static Boolean IsBitsIsSet(Int32 status_Register, Int32 bits)
        {
            if ((status_Register & bits) == bits)
                return true;

            return false;
        }

        public static Boolean IsOnlyBitsIsSet(Int32 status_Register, Int32 bits)
        {
            if (status_Register == bits)
                return true;

            return false;
        }
    }
}
