﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ush4.Services.Sios
{
    public static class SIOSEnums
    {
        public enum ErrorsCodes
        {
            IFM_ERROR_NONE = 0,
            IFM_ERROR_DEVICE_INVALID = -1,
            IFM_ERROR_BAD_CHANNEL = -2,
            IFM_ERROR_BAD_DEVICETYPE = -3,
            IFM_ERROR_DATALEN = -4,
            IFM_ERROR_UNKNOWN = -6,
            IFM_ERROR_DEVICECOUNT_OVERFOW = -10,
            IFM_ERROR_BAD_REQUESTTYPE = -11,
            IFM_INVALID_USB_ID = -12,
            IFM_ERROR_CREATE_HANDLE = -13,
            IFM_ERROR_NOT_IMPLEMENTED = -14,
            IFM_ERROR_I2C_IN_USE = -15,
            IFM_ERROR_I2C_WRITE = -16,
            IFM_ERROR_I2C_TIMEOUT = -17,
            IFM_ERROR_OWR_TO_HIGH = -18,
            IFM_ERROR_INFO_NOT_AVAILABLE = -19,
            IFM_BAD_SENSOR = -20,
            IFM_ERROR_I2C_READ = -21,
            IFM_ERROR_BAD_PARAMETER = -22
        }


        public enum StatusFlags
        {
            //the lower 16 bit are channel dependend
            IFM_STATUS_BEAMBREAK_QUADRANT = 0x0001,   // miscount detected: the interferometer counter has detected an invalid jump over more than one quadrants
            IFM_STATUS_BEAMBREAK_LEVEL = 0x0002,  // the signal amplitude is lower than a given threashold so that miscounts are likely
            IFM_STATUS_LASER_STABLE = 0x0010,   // the laser(s) is(are) stable (only in systems with stabilized lasers)
            IFM_STATUS_LASER_WAS_UNSTABLE = 0x0020,   // since last IfmSetZero the laser was at least one time unstable

            // the upper 16 bit are independend from the channel
            IFM_STATUS_BUFFER_OVERFLOW_DEV = 0x0100,   // the FIFO in the interferometer had an overflow; data loss by to large samplerate has occured
            IFM_STATUS_BUFFER_OVERFLOW_DLL = 0x0200,   // the measurement value buffer in the DLL had an overlow; data loss by infrequent call of IfmGetValue
            IFM_STATUS_BLOCKMODE = 0x0400,   // the device is in blockmode
            IFM_STATUS_BAD_REQUEST = 0x8000,   // the status request could not be answered, perhaps due to bad deviceNumber or invalid channel

        }

        public enum SignalQuality
        {
            IFM_SIGNALQ_A1 = 0x10,                //Amplitude of the signal 1
            IFM_SIGNALQ_O1 = 0x20,                //Offset of the signal 2
            IFM_SIGNALQ_A2 = 0x40,                //Amplitude of the signal 2
            IFM_SIGNALQ_O2 = 0x80,                //Offset of the signal 2
            IFM_SIGNALQ_SUM = 0x100,               //Overall quality from 0..100%
            IFM_SIGNALQ_FREQ = 0x200,               //Frequency of the reference mirror vibrator
            IFM_SIGNALQ_STATE = 0x400               //Status of the signal control card; for internal use
        }

        public enum DeviceInfo
        {
            IFM_DEVINFO_SRATE = 1,   //current sampleRate (internal, before reduction by filters)
            IFM_DEVINFO_OUTRATE = 2,   //output word rate, like set by IfmSetMeasurement or saved in the flash
            // this is the real output wird rate, which may be differ from the selected due to steps in possible sample rate
            IFM_DEVINFO_FILTERFLAGS = 3,   //filter flags, like set by IfmSetFilter or saved in the flash
            IFM_DEVINFO_MEASUREMENTFLAGS = 4,   //mesurement flags, like set by IfmSetMeasurement or saved in the flash
            IFM_DEVINFO_TRIGGERMODE = 5,   //trigger mode, like set by IfmSetTrigger or or saved in the flash
            IFM_DEVINFO_AVG1 = 6,   //filter option for the average 1, like set by IfmSetFilter or saved in the flash
            IFM_DEVINFO_AVG2 = 7,   //filter option for the average 2, like set by IfmSetFilter or saved in the flash
            IFM_DEVINFO_AVAILABLE = 8,   // infos available, if not wait until the DLL has get it from the device
            IFM_DEVINFO_SERIALNUMBER = 9,   // return the (USB-) serial number of the card
            IFM_DEVINFO_RESETSTATUS = 10,   // resets the given bits in the device status
            IFM_DEVINFO_READY = 11,   // ready for IfmStart
            IFM_DEVINFO_VERSIONSTRING = 12,   // Firmware-Version and compiling data
            IFM_DEVINFO_FPGAVERSION = 13,   // Version of the programmable hardware (FPGA)

            IFM_DEVINFO_CMDDELAY = 100,  // delay in ms between transmitted commands
            IFM_DEVINFO_I2CTIMEOUT = 101  // timeout in ms for IfmI2CRead and IfmI2CWrite
        }


        public enum FilterFlags
        {
            IFM_FILTER_STAGE1 = 0x01,                //Filter stage 1 (FIR) is on
            IFM_FILTER_STAGE2 = 0x02,                //Filter stage 2 (avg2) is on
            IFM_FILTER_STAGE3 = (IFM_FILTER_AVG9 | IFM_FILTER_AVG6 | IFM_FILTER_AVG5), //Filter stage 3 is on
            IFM_FILTER_AVG9 = 0x04,                // AVG9 in stage 3 is on
            IFM_FILTER_AVG6 = 0x08,               // AVG6 in stage 3 is on
            IFM_FILTER_AVG5 = 0x10               // AVG5 in stage 3 is on
        }

        public enum MeasurementFlags
        {
            IFM_MMASK_DEFAULT = 0x0106,  		//One channel, , SINX + COSX

            // Field "MeasurementFlags", mask definition
            IFM_MEAS_ONECHANNEL = 0x0100,              //Channel number 1 is on
            IFM_MEAS_TWOCHANNEL = 0x0300,               //Channel number 1 and 2 are on
            IFM_MEAS_THREECHANNEL = 0x0700,               //Channel number 1 to 3 are on
            IFM_MEAS_FOURCHANNEL = 0x0F00,               //Channel number 1 to 4 are on

            IFM_MEAS_TWOCHANNELS = IFM_MEAS_TWOCHANNEL,
            IFM_MEAS_THREECHANNELS = IFM_MEAS_THREECHANNEL,
            IFM_MEAS_FOURCHANNELS = IFM_MEAS_FOURCHANNEL,

            IFM_MEAS_CH1 = 0x0100,
            IFM_MEAS_CH2 = 0x0200,
            IFM_MEAS_CH3 = 0x0400,
            IFM_MEAS_CH4 = 0x0800,


            IFM_MEAS_USERMODE = 0x0001,               // Reserved
            IFM_MEAS_LENGTH = 0x0002,               // Full length information
            IFM_MEAS_SINCOS = 0x0004,               // SINX / COSX
            IFM_MEAS_CIRCLE = 0x0008,               // Amplitude of the lissagous figure
            IFM_MEAS_PATTERN = 0x0010,               // Test pattern
            IFM_MEAS_CLOCKCOUNT = 0x0020,               // Internal clock counter; 50MHz timebase
            IFM_MEAS_VAL_COUNTER = IFM_MEAS_CLOCKCOUNT,  // for compatibility
            IFM_MEAS_SAMPLECOUNT = 0x0080,               // Sample counter
            IFM_MEAS_VAL_MASK = (0x003F | 0x0080),      // Selection of measurement values is coded in these bits
            IFM_MEAS_BEAMBREAK_OFF = 0x0040,               // BeamBreak should not be monitored

            IFM_MEAS_FILTER_DEFAULT = 0x0000,             //The default filter is to use
            IFM_MEAS_FILTER_OFF = 0x1000,             //No filtering
            IFM_MEAS_FILTER_USER = 0x2000,             //The user filter is to use
            IFM_MEAS_FILTER_MASK = 0x3000,             //Filter flags are coded in these bits

            IFM_MEAS_RAWMODE = 0x4000             //Mode for testing purpose, do not use
        }


        public enum SensorProperty
        {
            IFM_ENVIR_SENSOR_TEMP = 0x10,
            IFM_ENVIR_SENSOR_HUMIDITY = 0x20,
            IFM_ENVIR_SENSOR_AIRPRESSURE = 0x30,

            // which channels is the sensor assigned?
            IFM_ENVIR_CHANNEL1 = 0x01,
            IFM_ENVIR_CHANNEL2 = 0x02,
            IFM_ENVIR_CHANNEL3 = 0x04,
            IFM_ENVIR_CHANNEL4 = 0x08,

            //Flag to mark the values for Edlen-corection
            IFM_ENVIR_EDLEN = 0x80,


            // channel and sensor type are coded in one byte
            // the following masks allow better to distinguish
            IFM_ENVIR_SENSORMASK = 0x0070,
            IFM_ENVIR_CHANNELMASK = 0x000F,
            IFM_ENVIR_VALID = 0x0100,
            IFM_ENVIR_CURRENT = 0x0200,

            //IfmSetOptions
            IFM_OPTION_DEBUGFILES = 0x0001,
            IFM_OPTION_POLLSELF = 0x0002,
            IFM_OPTION_BLOCKONCLOSE = 0x0003
        }


        public enum AuxValues
        {
            IFM_VALUETYPE_COS = 1,
            IFM_VALUETYPE_ADC1 = 1,
            IFM_VALUETYPE_SIN = 2,
            IFM_VALUETYPE_ADC2 = 2,
            IFM_VALUETYPE_NORM = 3,
            IFM_VALUETYPE_TESTPATTERN = 4,
            IFM_VALUETYPE_CLOCKCOUNT = 5,
            IFM_VALUETYPE_COUNTER = 5,
            IFM_VALUETYPE_SAMPLECOUNT = 6
        }

        public enum Trigger
        {
            IFM_TRIGGER_OFF = 0,

            // Trigger-Input:
            IFM_TRIGGER_START = 0x0001,   //start of measuring values after a valid (falling or rising) edge
            IFM_TRIGGER_STARTSTOP_PROC = 0x0002,   //the StartStop-input of processed values
            IFM_TRIGGER_STARTSTOP = 0x0004,  //the StartStop-input of raw values

            // Clock-Input
            IFM_TRIGGER_CLOCK = 0x0008,      // clocks in unprocessed values from the counter/interpolator unit
            // It can be used to use an external sample rate or
            // to synchronize the sampling with external hardware (for example, incremental encoders)

            //Event-Input
            IFM_TRIGGER_EVENT = 0x0010,      // takes a processed value and sends it to the PC

            // Edge definitions for Trigger
            IFM_TRIGGER_CLOCK_RISING_EDGE = 0x0100,                //rising edge in the ext. clock input is validly
            IFM_TRIGGER_STARTSTOP_RISING_EDGE = 0x0200,           //rising edge in the trigger  input is validly
            IFM_TRIGGER_EVENT_RISING_EDGE = 0x0400,                //rising edge in the event input is validly

            IFM_OWR_FAST = 0x1000,              //fast data rate (OWR>200 Hz)
            IFM_OWR_SLOW = 0x2000              //slow data rate (OWR<=200Hz)
        }

        public enum EnviromentFlags
        {
            IFM_ENVIRFLAG_SENSORMASK = 0x00FF,      // in these bits the sensor number is coded, where the value was measured
            IFM_ENVIRFLAG_MEASURED = 0x0100,      // the value was measured; otherwise it's a default value
            IFM_ENVIRFLAG_CURRENT = 0x0200,      // the value was measured with the last data set
            // (in a typical configuration it's not older than 4 secs)
            IFM_ENVIRFLAG_MANUAL = 0x0400      // value was given manually per IfmSet... function
        }
    }
}
