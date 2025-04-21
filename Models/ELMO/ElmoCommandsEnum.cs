using ElmoMotionControlComponents.Drive.EASComponents;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ush4.Models.ELMO
{
    public class ElmoCommandsEnum
    {
        public enum enSignalsIndex
        {
            Velocity = 1,
            Position = 2,
            DC_Bus_Voltage = 7,

        }

        public enum enOnOff
        {
            Off = 0,
            On = 1
        }

        public enum enService
        {
            

            Socet1 = 1,
            Socet4 = 4,
            VirtualSin = 8,
            BiSS = 5,
            Socet4_in_CA = 44,
            SocetPosition_in_CA = 68,
            SocetVelocity_in_CA = 69,
            SocetCurrent_in_CA = 70,
            SinGenerator_in_TW = 80,
            TimeQuanta = 29,
        }

        public static String DriveErrorObjectToString(IDriveErrorObject err)
        {
            return String.Format(Properties.ResourcesE.ErrorFormat,
                err.ErrorCode, err.ErrorDescription, err.LibraryErrorCode, err.LibraryErrorDescription);
        }


        public static class ElmoCommands
        {

            public enum enRecordStatus
            {
                InvalidData = -1,
                Complete = 0,
                StartedByBG = 1,
                StartedImmidiately = 2,
                StartedByTrigger = 3
            }

            public enum enUnitModes
            {
                Torque = 1,
                Velocity = 2,
                Stepper_1 = 3,
                Reserved = 4,
                Position = 5,
                Stepper_2 = 6,
            }


            const String setFormat = "{0} = {1}";
            const String getFormat = "{0}";
            const String executeFormat = "{0}##{1}({2})";

            public const String OutputPort = "OP";
            public const String MotorOn = "MO";
            public const String MotorFault = "MF";
            public const String ResetSoft = "RS";
            public const String ErrorCode = "EC";
            public const String CPUDump = "CD";
            public const String StatusRegister = "SR";
            public const String RecordLength = "RL";
            public const String RecordGap = "RG";
            public const String GetRecorderSignal = "BH";
            public const String ActivateRecorder_Status = "RR";
            public const String SamplingTime = "TS";
            public const String ExecuteProgram = "XQ";
            public const String UnitMode = "UM";
            public const String KillUserProgram = "KL";

            public static String GetDataRequest(String cmd)
            {
                return String.Format(getFormat, cmd);
            }

            public static String SetDataRequest(String cmd, Int32 data)
            {
                return String.Format(setFormat, cmd, data.ToString());
            }

            public static String UserProgramExecuteRequest(String cmd, String programName, String data_args)
            {
                return String.Format(executeFormat, cmd, programName, data_args);
            }
        }


        public static class ElmoArrayCommands
        {
            public enum enHomingIndexes
            {
                ActivationMode = 1,
                AbsolutValue = 2,
                EventDefenition = 3,
                AfterEventBehavior = 4,
                PX = 5,
                OutputValue = 6,
                PX_captured = 7,
                PY_captured = 8
            }

            public enum enSineExcitationIndexes
            {
                Units = 1,
                Amplitude = 2,
                Frequency = 3,
            }

            public enum enSineExcitationUnits
            {
                Current = 1,
                Velocity = 2,
                Position = 3,
            }


            public enum enExtendetErrorIndexes
            {
                FeedbackError = 1,
                ProfilerInitializationError = 2,
                MotionOnError = 5,
            }

            public enum enWizardIntIndexes
            {
                MaxRecordLength = 20,
                ActualRecordength = 21
            }


            public enum enRecordParemeters
            {
                TimeQuantumBase = 0,
                TriggerVariable = 1,
                PretriggerPercentage = 2,
                TriggerType = 3,
                Level1_pos_slope = 4,
                Level2_neg_slope = 5,
                Level_digit_input = 6,
                Digit_input_mask = 7,
                Lower_buffer_index = 8,
                Higher_buffer_index = 9,
                Time_val_foe_start = 10,
                SelectedSignal = 11,
            }


            const String setFormat = "{0}[{1}] = {2}";
            const String getFormat = "{0}[{1}]";

            public const String SineExcitation = "SE";
            public const String CommutationArray = "CA";
            public const String PositionAbsoluteArray = "PA";
            public const String PositionRelative = "PR";
            public const String BeginMotion = "BG";
            public const String FeedbackPosition = "FP";
            public const String ExtendedError = "EE";
            public const String TW = "TW";
            public const String MiscellaneousReports = "WS";
            public const String RecorderParameters = "RP";
            public const String Homming = "HM";
            public const String UserFloat = "UF";
            public const String UserInt = "UI";
            public const String WizardInt = "WI";

            public static String GetDataRequest(String cmd, Int32 ind)
            {
                return String.Format(getFormat, cmd, ind);
            }

            public static String SetDataRequest(String cmd, Int32 ind, Int32 data)
            {
                return String.Format(setFormat, cmd, ind, data.ToString());
            }

            public static String SetDataRequest(String cmd, Int32 ind, Double data)
            {
                return String.Format(setFormat, cmd, ind, data.ToString("F9", CultureInfo.InvariantCulture)); // F5
            }
        }
    }
}
