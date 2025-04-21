using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ush4.Services.Sios
{
    public static class APIWrapper
    {
        #region c++ import

        #region Functions for the initialization, opening and closing
        [DllImport("siosifm", EntryPoint = "IfmClose", CallingConvention = CallingConvention.Cdecl)]
        private static extern void IfmClose();

        [DllImport("siosifm", EntryPoint = "IfmCloseDevice", CallingConvention = CallingConvention.Cdecl)]
        private static extern void IfmCloseDevice(int devNumber);

        [DllImport("siosifm", EntryPoint = "IfmInit", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmInit();

        [DllImport("siosifm", EntryPoint = "IfmDeviceCount", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmDeviceCount();

        [DllImport("siosifm", EntryPoint = "IfmMaxDeviceCount", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmMaxDeviceCount();

        [DllImport("siosifm", EntryPoint = "IfmOpenCOM", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmOpenCOM(int comNumber);

        [DllImport("siosifm", EntryPoint = "IfmOpenUSB", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmOpenUSB(int uniqueId);

        [DllImport("siosifm", EntryPoint = "IfmSearchUSBDevices", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmSearchUSBDevices();

        [DllImport("siosifm", EntryPoint = "IfmUSBDeviceCount", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmUSBDeviceCount();

        [DllImport("siosifm", EntryPoint = "IfmUSBDeviceSerial", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmUSBDeviceSerial(int uniqueId);
        #endregion


        #region Functions for the measurement
        [DllImport("siosifm", EntryPoint = "IfmGetFilterCoeff", CallingConvention = CallingConvention.Cdecl)]
        private static extern double IfmGetFilterCoeff(int devNumber, int channel);

        [DllImport("siosifm", EntryPoint = "IfmGetFilterNotchFrequency", CallingConvention = CallingConvention.Cdecl)]
        private static extern double IfmGetFilterNotchFrequency(int devNumber, int channel);

        [DllImport("siosifm", EntryPoint = "IfmGetRecentValues", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmGetRecentValues(int devNumber, int index);

        [DllImport("siosifm", EntryPoint = "IfmGetValues", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmGetValues(int devNumber);

        [DllImport("siosifm", EntryPoint = "IfmLengthValue", CallingConvention = CallingConvention.Cdecl)]
        private static extern double IfmLengthValue(int devNumber, int channel);

        [DllImport("siosifm", EntryPoint = "IfmSetPreset", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmSetPreset(int devNumber, int channel, double presetValue);

        [DllImport("siosifm", EntryPoint = "IfmGetPreset", CallingConvention = CallingConvention.Cdecl)]
        private static extern double IfmGetPreset(int devNumber, int channel);

        [DllImport("siosifm", EntryPoint = "IfmSetFilter", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmSetFilter(int devNumber, uint filterFlags, int avg1, int avg2);

        [DllImport("siosifm", EntryPoint = "IfmSetFilterCoeff", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmSetFilterCoeff(int devNumer, int channel, double coeff);

        [DllImport("siosifm", EntryPoint = "IfmSetFilterNotchFrequency", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmSetFilterNotchFrequency(int devNumber, int channel, double freq);

        [DllImport("siosifm", EntryPoint = "IfmSetMeasurement", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmSetMeasurement(int devNumber, uint measurementFlags, double outputWordRate);

        [DllImport("siosifm", EntryPoint = "IfmSetToZero", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmSetToZero(int devNumber, int channelMask);

        [DllImport("siosifm", EntryPoint = "IfmSetTrigger", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmSetTrigger(int devNumber, uint triggerMode);

        [DllImport("siosifm", EntryPoint = "IfmStart", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmStart(int devNumber);

        [DllImport("siosifm", EntryPoint = "IfmStop", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmStop(int devNumber);

        [DllImport("siosifm", EntryPoint = "IfmResetBuffer", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmResetBuffer(int devNumber);

        [DllImport("siosifm", EntryPoint = "IfmValueCount", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmValueCount(int devNumber);

        [DllImport("siosifm", EntryPoint = "IfmAngleValue", CallingConvention = CallingConvention.Cdecl)]
        private static extern double IfmAngleValue(int devNumber, int channel1, int channel2, int unit);

        [DllImport("siosifm", EntryPoint = "IfmAngleAvailable", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmAngleAvailable(int devNumber, int channels);

        [DllImport("siosifm", EntryPoint = "IfmCancelBlock", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmCancelBlock(int devNumber);

        [DllImport("siosifm", EntryPoint = "IfmIsBlockAvailable", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmIsBlockAvailable(int devNumber);

        [DllImport("siosifm", EntryPoint = "IfmSetBlockMode", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmSetBlockMode(int devNumber, int measurementFlags, int triggerMode, int outputWordRate);

        [DllImport("siosifm", EntryPoint = "IfmSetBlockModeFilter", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmSetBlockModeFilter(int devNumber, uint filterFlags, int avg1, int avg2);

        [DllImport("siosifm", EntryPoint = "IfmSetBlockModeFilterCoeff", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmSetBlockModeFilterCoeff(int devNumer, int channel, double coeff);

        [DllImport("siosifm", EntryPoint = "IfmStartBlock", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmStartBlock(int devNumber, int blockLen);

        #endregion


        #region Functions for the controlling of the interferometers
        [DllImport("siosifm", EntryPoint = "IfmGetAGC", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmGetAGC(int devNumber, int channel);

        [DllImport("siosifm", EntryPoint = "IfmGetRefMirrorVibration", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmGetRefMirrorVibration(int devNumber, int channel);

        [DllImport("siosifm", EntryPoint = "IfmNewSignalQualityAvailable", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmNewSignalQualityAvailable(int devNumber);

        [DllImport("siosifm", EntryPoint = "IfmSetAGC", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmSetAGC(int devNumber, int channel, int on);

        [DllImport("siosifm", EntryPoint = "IfmSetRefMirrorVibration", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmSetRefMirrorVibration(int devNumber, int channel, int on);

        [DllImport("siosifm", EntryPoint = "IfmSignalQuality", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmSignalQuality(int devNumber, int channel, int select);

        [DllImport("siosifm", EntryPoint = "IfmStatus", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmStatus(int devNumber, int channel);

        [DllImport("siosifm", EntryPoint = "IfmWasBeamBreak", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmWasBeamBreak(int devNumber, int channel);

        [DllImport("siosifm", EntryPoint = "IfmWasLaserUnstable", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmWasLaserUnstable(int devNumber, int channel);

        [DllImport("siosifm", EntryPoint = "IfmWasLostValues", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmWasLostValues(int devNumber);
        #endregion


        #region Functions for the communication with other devices

        [DllImport("siosifm", EntryPoint = "IfmI2CRead", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmI2CRead(int devNumber, int i2cAddr, int ramAddr, int count);

        [DllImport("siosifm", EntryPoint = "IfmI2CRequestRead", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmI2CRequestRead(int devNumber, int i2cAddr, int ramAddr, int count);

        [DllImport("siosifm", EntryPoint = "IfmI2CReadBuffer", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr IfmI2CReadBuffer(int devNumber);

        [DllImport("siosifm", EntryPoint = "IfmI2CReadValue", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr IfmI2CReadValue(int devNumber, int index);

        [DllImport("siosifm", EntryPoint = "IfmI2CReadReady", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmI2CReadReady(int devNumber);

        [DllImport("siosifm", EntryPoint = "IfmI2CRequestWrite", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmI2CRequestWrite(int devNumber, int i2cAddr, int ramAddr, int count, IntPtr buffer);

        [DllImport("siosifm", EntryPoint = "IfmI2CStatus", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmI2CStatus(int devNumber);

        [DllImport("siosifm", EntryPoint = "IfmI2CWrite", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmI2CWrite(int devNumber, int i2cAddr, int ramAddr, int count, IntPtr buffer);

        #endregion


        #region Functions for the environment values
        [DllImport("siosifm", EntryPoint = "IfmAirPressure", CallingConvention = CallingConvention.Cdecl)]
        private static extern double IfmAirPressure(int devNumber, int channel);

        [DllImport("siosifm", EntryPoint = "IfmAirPressureFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmAirPressureFlags(int devNumber, int channel);

        [DllImport("siosifm", EntryPoint = "IfmAirRefraction", CallingConvention = CallingConvention.Cdecl)]
        private static extern double IfmAirRefraction(int devNumber, int channel);

        [DllImport("siosifm", EntryPoint = "IfmConversionCoeff", CallingConvention = CallingConvention.Cdecl)]
        private static extern double IfmConversionCoeff(int devNumber, int channel);

        [DllImport("siosifm", EntryPoint = "IfmDeadpathCoeff", CallingConvention = CallingConvention.Cdecl)]
        private static extern double IfmDeadpathCoeff(int devNumber, int channel);

        [DllImport("siosifm", EntryPoint = "IfmEnableEdlenCorrection", CallingConvention = CallingConvention.Cdecl)]
        private static extern void IfmEnableEdlenCorrection(int devNumber, int channel, int on);

        [DllImport("siosifm", EntryPoint = "IfmEnvSensorCount", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmEnvSensorCount(int devNumber);

        [DllImport("siosifm", EntryPoint = "IfmHumidity", CallingConvention = CallingConvention.Cdecl)]
        private static extern double IfmHumidity(int devNumber, int channel);

        [DllImport("siosifm", EntryPoint = "IfmHumidityFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmHumidityFlags(int devNumber, int channel);

        [DllImport("siosifm", EntryPoint = "IfmIsEdlenEnabled", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmIsEdlenEnabled(int devNumber, int channel);

        [DllImport("siosifm", EntryPoint = "IfmGetDeadPath", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmGetDeadPath(int devNumber, int channel);

        [DllImport("siosifm", EntryPoint = "IfmNewEnvValuesAvailable", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmNewEnvValuesAvailable(int devNumber);

        [DllImport("siosifm", EntryPoint = "IfmResetManualEnvironment", CallingConvention = CallingConvention.Cdecl)]
        private static extern void IfmResetManualEnvironment(int devNumber, int channel);

        [DllImport("siosifm", EntryPoint = "IfmSensorProperty", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint IfmSensorProperty(int devNumber, int sensor);

        [DllImport("siosifm", EntryPoint = "IfmSensorValue", CallingConvention = CallingConvention.Cdecl)]
        private static extern double IfmSensorValue(int devNumber, int sensor);

        [DllImport("siosifm", EntryPoint = "IfmSetAirPressure", CallingConvention = CallingConvention.Cdecl)]
        private static extern void IfmSetAirPressure(int devNumber, int channel, double value);

        [DllImport("siosifm", EntryPoint = "IfmSetConvertionCoeff", CallingConvention = CallingConvention.Cdecl)]
        private static extern void IfmSetConvertionCoeff(int devNumber, int channel, double value);

        [DllImport("siosifm", EntryPoint = "IfmSetDeadPath", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmSetDeadPath(int devNumber, int channel, int deadPath);

        [DllImport("siosifm", EntryPoint = "IfmSetHumidity", CallingConvention = CallingConvention.Cdecl)]
        private static extern void IfmSetHumidity(int devNumber, int channel, double value);

        [DllImport("siosifm", EntryPoint = "IfmSetTemperature", CallingConvention = CallingConvention.Cdecl)]
        private static extern void IfmSetTemperature(int devNumber, int channel, double value);

        [DllImport("siosifm", EntryPoint = "IfmSetWavelength", CallingConvention = CallingConvention.Cdecl)]
        private static extern void IfmSetWavelength(int devNumber, int channel, double value);

        [DllImport("siosifm", EntryPoint = "IfmSetWaterVapourPressure", CallingConvention = CallingConvention.Cdecl)]
        private static extern void IfmSetWaterVapourPressure(int devNumber, int channel, double value);

        [DllImport("siosifm", EntryPoint = "IfmTemperature", CallingConvention = CallingConvention.Cdecl)]
        private static extern double IfmTemperature(int devNumber, int channel);

        [DllImport("siosifm", EntryPoint = "IfmTemperatureFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmTemperatureFlags(int devNumber, int channel);

        [DllImport("siosifm", EntryPoint = "IfmVacuumWavelength", CallingConvention = CallingConvention.Cdecl)]
        private static extern double IfmVacuumWavelength(int devNumber, int channel);

        [DllImport("siosifm", EntryPoint = "IfmWaterVapourPressure", CallingConvention = CallingConvention.Cdecl)]
        private static extern double IfmWaterVapourPressure(int devNumber, int channel);

        [DllImport("siosifm", EntryPoint = "IfmWavelength", CallingConvention = CallingConvention.Cdecl)]
        private static extern double IfmWavelength(int devNumber, int channel);
        #endregion


        #region Extended functions
        [DllImport("siosifm", EntryPoint = "IfmAuxValue", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmAuxValue(int devNumber, int channel, int valueType);

        [DllImport("siosifm", EntryPoint = "IfmChannels", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmChannels(int devNumber);

        [DllImport("siosifm", EntryPoint = "IfmDeviceInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmDeviceInfo(int devNumber, int requestedInfo);

        [DllImport("siosifm", EntryPoint = "IfmDeviceInterface", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmDeviceInterface(int devNumber);

        [DllImport("siosifm", EntryPoint = "IfmDeviceType", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmDeviceType(int devNumber);

        [DllImport("siosifm", EntryPoint = "IfmDeviceValid", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmDeviceValid(int devNumber);

        [DllImport("siosifm", EntryPoint = "IfmDLLVersionString", CallingConvention = CallingConvention.Cdecl)]
        private static extern string IfmDLLVersionString();

        [DllImport("siosifm", EntryPoint = "IfmFireTrigger", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmFireTrigger(int devNumber);

        [DllImport("siosifm", EntryPoint = "IfmFirmwareVersion", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmFirmwareVersion(int devNumber);

        [DllImport("siosifm", EntryPoint = "IfmGetError", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmGetError();

        [DllImport("siosifm", EntryPoint = "IfmGetErrorString", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr IfmGetErrorString(int errorNumber);

        [DllImport("siosifm", EntryPoint = "IfmRawValue", CallingConvention = CallingConvention.Cdecl)]
        private static extern long IfmRawValue(int devNumber, int channel);

        [DllImport("siosifm", EntryPoint = "IfmResetDevice", CallingConvention = CallingConvention.Cdecl)]
        private static extern void IfmResetDevice(int devNumber);

        [DllImport("siosifm", EntryPoint = "IfmSetDeviceInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmSetDeviceInfo(int devNumber, int infoNo, int newValue);

        [DllImport("siosifm", EntryPoint = "IfmSetOption", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IfmSetOption(int option, int param1);
        #endregion

        #endregion



        #region Functions for the initialization, opening and closing
        public static void Close()
        {
            IfmClose();
        }

        public static void CloseDevice(int devNumber)
        {
            IfmCloseDevice(devNumber);
        }

        public static int DeviceCount()
        {
            return IfmDeviceCount();
        }

        public static void Init()
        {
            SIOSLibExeption.RetCodeChecker(() => IfmInit());
        }

        public static int OpenCOM(int comNumber)
        {
            return SIOSLibExeption.RetCodeChecker(() => IfmOpenCOM(comNumber));
        }

        public static int OpenUSB(int uniqueId)
        {
            return SIOSLibExeption.RetCodeChecker(() => IfmOpenUSB(uniqueId));
        }

        public static int SearchUSBDevices()
        {
            return SIOSLibExeption.RetCodeChecker(() => IfmSearchUSBDevices());
        }

        public static int USBDeviceCount()
        {
            return SIOSLibExeption.RetCodeChecker(() => IfmUSBDeviceCount());
        }

        public static int USBDeviceSerial(int uniqueId)
        {
            return SIOSLibExeption.RetCodeChecker(() => IfmUSBDeviceSerial(uniqueId));
        }
        #endregion


        #region Functions for the measurement
        public static double GetFilterCoeff(int devNumber, int channel)
        {
            double coef = IfmGetFilterCoeff(devNumber, channel);
            if (coef == 0)
                throw new SIOSLibExeption("GetFilterCoeff return error code.");
            else
                return coef;
        }


        public static double GetFilterNotchFrequency(int devNumber, int channel)
        {
            double coef = IfmGetFilterNotchFrequency(devNumber, channel);
            if (coef == 0)
                throw new SIOSLibExeption("GetFilterNotchFrequency return error code.");
            else
                return coef;
        }


        public static bool GetRecentValues(int devNumber, int index)
        {
            int controller_state = SIOSLibExeption.RetCodeChecker(() => IfmGetRecentValues(devNumber, index));
            if (controller_state == 0)
                return false;
            else
                return true;
        }


        public static bool GetValues(int devNumber)
        {
            int controller_state = SIOSLibExeption.RetCodeChecker(() => IfmGetValues(devNumber));
            if (controller_state == 0)
                return false;
            else
                return true;
        }


        public static double LengthValue(int devNumber, int channel)
        {
            return IfmLengthValue(devNumber, channel);
        }


        public static void SetPreset(int devNumber, int channel, double presetValue)
        {
            SIOSLibExeption.RetCodeChecker(() => IfmSetPreset(devNumber, channel, presetValue));
        }


        public static double GetPreset(int devNumber, int channel)
        {
            double coef = IfmGetPreset(devNumber, channel);
            if (coef == 0)
                throw new SIOSLibExeption("GetPreset return error code.");
            else
                return coef;
        }


        public static void SetFilter(int devNumber, uint filterFlags, int avg1, int avg2)
        {
            SIOSLibExeption.RetCodeChecker(() => IfmSetFilter(devNumber, filterFlags, avg1, avg2));
        }


        public static void SetFilterCoeff(int devNumber, int channel, double coeff)
        {
            SIOSLibExeption.RetCodeChecker(() => IfmSetFilterCoeff(devNumber, channel, coeff));
        }


        public static void SetFilterNotchFrequency(int devNumber, int channel, double freq)
        {
            SIOSLibExeption.RetCodeChecker(() => IfmSetFilterNotchFrequency(devNumber, channel, freq));
        }


        public static void SetMeasurement(int devNumber, uint measurementFlags, double outputWordRate)
        {
            SIOSLibExeption.RetCodeChecker(() => IfmSetMeasurement(devNumber, measurementFlags, outputWordRate));
        }


        public static void SetToZero(int devNumber, int channelMask)
        {
            SIOSLibExeption.RetCodeChecker(() => IfmSetToZero(devNumber, channelMask));
        }

        public static void SetTrigger(int devNumber, uint triggerMode)
        {
            SIOSLibExeption.RetCodeChecker(() => IfmSetTrigger(devNumber, triggerMode));
        }


        public static void Start(int devNumber)
        {
            SIOSLibExeption.RetCodeChecker(() => IfmStart(devNumber));
        }


        public static void Stop(int devNumber)
        {
            SIOSLibExeption.RetCodeChecker(() => IfmStop(devNumber));
        }


        public static void ResetBuffer(int devNumber)
        {
            SIOSLibExeption.RetCodeChecker(() => IfmResetBuffer(devNumber));
        }

        public static int ValueCount(int devNumber)
        {
            return IfmValueCount(devNumber);
        }

        public static double AngleValue(int devNumber, int channel1, int channel2, int unit)
        {
            double angle = IfmAngleValue(devNumber, channel1, channel2, unit);
            if (angle == 0)
                throw new SIOSLibExeption("AngleValue return error code.");
            else
                return angle;
        }

        public static bool AngleAvailable(int devNumber, int channels)
        {
            int controller_state = SIOSLibExeption.RetCodeChecker(() => IfmAngleAvailable(devNumber, channels));
            if (controller_state == 0)
                return false;
            else
                return true;
        }
        #endregion


        #region Functions for the controlling of the interferometers
        public static bool GetAGC(int devNumber, int channel)
        {
            return Convert.ToBoolean(SIOSLibExeption.RetCodeChecker(() => IfmGetAGC(devNumber, channel)));

        }


        public static bool GetRefMirrorVibration(int devNumber, int channel)
        {
            return Convert.ToBoolean(SIOSLibExeption.RetCodeChecker(() => IfmGetRefMirrorVibration(devNumber, channel)));

        }


        public static bool NewSignalQualityAvailable(int devNumber)
        {
            return Convert.ToBoolean(SIOSLibExeption.RetCodeChecker(() => IfmNewSignalQualityAvailable(devNumber)));
        }


        public static void SetAGC(int devNumber, int channel, bool isOn)
        {

            SIOSLibExeption.RetCodeChecker(() => IfmSetAGC(devNumber, channel, Convert.ToInt32(isOn)));
        }

        public static void SetRefMirrorVibration(int devNumber, int channel, bool isOn)
        {
            SIOSLibExeption.RetCodeChecker(() => IfmSetRefMirrorVibration(devNumber, channel, Convert.ToInt32(isOn)));
        }

        public static int SignalQuality(int devNumber, int channel, SIOSEnums.SignalQuality select)
        {
            return SIOSLibExeption.RetCodeChecker(() => IfmSignalQuality(devNumber, channel, (int)select));
        }

        public static int Status(int devNumber, int channel)
        {
            return SIOSLibExeption.RetCodeChecker(() => IfmStatus(devNumber, channel));
        }

        public static bool WasBeamBreak(int devNumber, int channel)
        {
            return Convert.ToBoolean(SIOSLibExeption.RetCodeChecker(() => IfmWasBeamBreak(devNumber, channel)));
        }


        public static bool WasLaserUnstable(int devNumber, int channel)
        {
            return Convert.ToBoolean(SIOSLibExeption.RetCodeChecker(() => IfmWasLaserUnstable(devNumber, channel)));
        }

        public static bool WasLostValues(int devNumber)
        {
            return Convert.ToBoolean(SIOSLibExeption.RetCodeChecker(() => IfmWasLostValues(devNumber)));

        }
        #endregion


        #region Functions for the communication with other devices
        public static void I2CRead(int devNumber, int i2cAddr, int ramAddr, int count)
        {
            SIOSLibExeption.RetCodeChecker(() => IfmI2CRead(devNumber, i2cAddr, ramAddr, count));
        }


        public static void I2CRequestRead(int devNumber, int i2cAddr, int ramAddr, int count)
        {
            SIOSLibExeption.RetCodeChecker(() => IfmI2CRequestRead(devNumber, i2cAddr, ramAddr, count));
        }

        public static IntPtr I2CReadBuffer(int devNumber)
        {
            return IfmI2CReadBuffer(devNumber);
        }


        public static IntPtr I2CReadValue(int devNumber, int index)
        {
            return IfmI2CReadValue(devNumber, index);
        }

        public static bool I2CReadReady(int devNumber)
        {
            return Convert.ToBoolean(IfmI2CReadReady(devNumber));
        }

        public static void I2CRequestWrite(int devNumber, int i2cAddr, int ramAddr, int count, IntPtr buffer)
        {
            IfmI2CRequestWrite(devNumber, i2cAddr, ramAddr, count, buffer);
        }

        public static void I2CStatus(int devNumber)
        {
            SIOSLibExeption.RetCodeChecker(() => IfmI2CStatus(devNumber));
        }

        public static void I2CWrite(int devNumber, int i2cAddr, int ramAddr, int count, IntPtr buffer)
        {
            SIOSLibExeption.RetCodeChecker(() => IfmI2CWrite(devNumber, i2cAddr, ramAddr, count, buffer));
        }
        #endregion


        #region Functions for the environment values
        public static double AirPressure(int devNumber, int channel)
        {
            return IfmAirPressure(devNumber, channel);
        }

        public static int AirPressureFlags(int devNumber, int channel)
        {
            return IfmAirPressureFlags(devNumber, channel);
        }

        public static double AirRefraction(int devNumber, int channel)
        {
            return IfmAirRefraction(devNumber, channel);
        }

        public static double ConversionCoeff(int devNumber, int channel)
        {
            return IfmConversionCoeff(devNumber, channel);
        }

        public static double DeadpathCoeff(int devNumber, int channel)
        {
            return IfmDeadpathCoeff(devNumber, channel);
        }

        public static void EnableEdlenCorrection(int devNumber, int channel, bool isOn)
        {
            IfmEnableEdlenCorrection(devNumber, channel, Convert.ToInt32(isOn));
        }

        public static int EnvSensorCount(int devNumber)
        {
            return SIOSLibExeption.RetCodeChecker(() => IfmEnvSensorCount(devNumber));
        }

        public static double Humidity(int devNumber, int channel)
        {
            return IfmHumidity(devNumber, channel);
        }

        public static int HumidityFlagsFlags(int devNumber, int channel)
        {
            return IfmHumidityFlags(devNumber, channel);
        }

        public static bool IsEdlenEnabled(int devNumber, int channel)
        {
            return Convert.ToBoolean(IfmIsEdlenEnabled(devNumber, channel));
        }

        public static int GetDeadPath(int devNumber, int channel)
        {
            return SIOSLibExeption.RetCodeChecker(() => GetDeadPath(devNumber, channel));
        }

        public static bool NewEnvValuesAvailable(int devNumber)
        {
            return Convert.ToBoolean(IfmNewEnvValuesAvailable(devNumber));
        }

        public static void ResetManualEnvironment(int devNumber, int channel)
        {
            IfmResetManualEnvironment(devNumber, channel);
        }

        public static uint SensorProperty(int devNumber, int sensor)
        {
            return IfmSensorProperty(devNumber, sensor);
        }

        public static double SensorValue(int devNumber, int sensor)
        {
            return IfmSensorValue(devNumber, sensor);
        }

        public static void SetAirPressure(int devNumber, int channel, double value)
        {
            IfmSetAirPressure(devNumber, channel, value);
        }

        public static void SetConvertionCoeff(int devNumber, int channel, double value)
        {
            IfmSetConvertionCoeff(devNumber, channel, value);
        }

        public static void SetDeadPath(int devNumber, int channel, int deadPath)
        {
            SIOSLibExeption.RetCodeChecker(() => IfmSetDeadPath(devNumber, channel, deadPath));
        }

        public static void SetHumidity(int devNumber, int channel, double value)
        {
            IfmSetHumidity(devNumber, channel, value);
        }

        public static void SetTemperature(int devNumber, int channel, double value)
        {
            IfmSetTemperature(devNumber, channel, value);
        }

        public static void SetWavelength(int devNumber, int channel, double value)
        {
            IfmSetWavelength(devNumber, channel, value);
        }

        public static void SetWaterVapourPressure(int devNumber, int channel, double value)
        {
            IfmSetWaterVapourPressure(devNumber, channel, value);
        }

        public static double Temperature(int devNumber, int sensor)
        {
            return IfmTemperature(devNumber, sensor);
        }

        public static int TemperatureFlags(int devNumber, int sensor)
        {
            return TemperatureFlags(devNumber, sensor);
        }

        public static double VacuumWavelength(int devNumber, int sensor)
        {
            return IfmVacuumWavelength(devNumber, sensor);
        }

        public static double Wavelength(int devNumber, int sensor)
        {
            return IfmWavelength(devNumber, sensor);
        }
        #endregion


        #region Extended functions
        public static int AuxValue(int devNumber, int channel, int valueType)
        {
            return IfmAuxValue(devNumber, channel, valueType);
        }

        public static int Cannels(int devNumber)
        {
            return SIOSLibExeption.RetCodeChecker(() => IfmChannels(devNumber));
        }

        public static int DeviceInfo(int devNumber, int requestedInfo)
        {
            return IfmDeviceInfo(devNumber, requestedInfo);
        }

        public static int DeviceInterface(int devNumber)
        {
            return IfmDeviceInterface(devNumber);
        }

        public static int DeviceType(int devNumber)
        {
            return IfmDeviceType(devNumber);
        }

        public static Boolean DeviceValid(int devNumber)
        {
            return Convert.ToBoolean(IfmDeviceValid(devNumber));
        }

        public static String DLLVersionString()
        {
            return DLLVersionString();
        }

        public static int FireTrigger(int devNumber)
        {
            return IfmFireTrigger(devNumber);
        }

        public static int FirmwareVersion(int devNumber)
        {
            return FirmwareVersion(devNumber);
        }

        public static int GetError()
        {
            return IfmGetError();
        }

        public static String GetErrorString(int errorNumber)
        {
            return Marshal.PtrToStringAnsi(IfmGetErrorString(errorNumber));
        }

        public static long RawValue(int devNumber, int channel)
        {
            return IfmRawValue(devNumber, channel);
        }

        public static void ResetDevice(int devNumber)
        {
            IfmResetDevice(devNumber);
        }

        public static void SetDeviceInfo(int devNumber, int infoNo, int newValue)
        {
            SIOSLibExeption.RetCodeChecker(() => IfmSetDeviceInfo(devNumber, infoNo, newValue));
        }

        public static void SetOption(int option, int param1)
        {
            SIOSLibExeption.RetCodeChecker(() => IfmSetOption(option, param1));
        }
        #endregion
    }
}
