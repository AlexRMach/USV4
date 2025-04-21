using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ush4.Services.Sios
{
    public class SIOSManager
    {

        int devNumber = Int32.MinValue;
        const int channel = 0;
        object _lock = new object();

        private int sIOSSerialNumber;
        public int SIOSSerialNumber
        {
            get { return sIOSSerialNumber; }
            set { sIOSSerialNumber = value; }
        }
        // Конструктор!
        public SIOSManager()
        {
            APIWrapper.Init();
        }

        // Деструктор!
        ~SIOSManager()
        {
            APIWrapper.Close();
        }

        public int[] SearchUSB()
        {
            int dev_count = APIWrapper.SearchUSBDevices();
            int[] to_ret = new int[dev_count];
            for (int i = 0; i < dev_count; i++)
            {
                to_ret[i] = APIWrapper.USBDeviceSerial(i);
            }
            return to_ret;
        }

        public void Open(int serialNumber)
        {
            int[] connected_devices = SearchUSB();
            int dev_index = Array.IndexOf(connected_devices, serialNumber);
            if (dev_index < 0)
                throw new Exception(String.Format("Interferometer with serial number {0} not found.", serialNumber));
            devNumber = APIWrapper.OpenUSB(dev_index);
        }


        public bool IsConfigurationAvaible()
        {
            lock (_lock)
            {
                return Convert.ToBoolean(APIWrapper.DeviceInfo(devNumber, (int)SIOSEnums.DeviceInfo.IFM_DEVINFO_AVAILABLE));
            }
        }

        public bool IsReadyForStart()
        {
            lock (_lock)
            {
                return Convert.ToBoolean(APIWrapper.DeviceInfo(devNumber, (int)SIOSEnums.DeviceInfo.IFM_DEVINFO_READY));
            }
        }

        public bool IsDeviceValid()
        {
            lock (_lock)
            {
                return Convert.ToBoolean(APIWrapper.DeviceValid(devNumber));
            }

        }


        public void ResetDevice()
        {
            lock (_lock)
            {
                APIWrapper.ResetDevice(devNumber);
            }
        }

        public void ResetBuffer()
        {
            lock (_lock)
            {
                APIWrapper.ResetBuffer(devNumber);
            }
        }


        public void SetTrigger(uint triggerFlags)
        {
            lock (_lock)
            {
                APIWrapper.SetTrigger(devNumber, triggerFlags);
            }
        }

        public void SetMeasurement(uint measurementFlags, double wordRate)
        {
            lock (_lock)
            {
                APIWrapper.SetMeasurement(devNumber, measurementFlags, wordRate);
            }
        }


        public void SetRefMirrorVibration(bool isOn)
        {
            lock (_lock)
            {
                APIWrapper.SetRefMirrorVibration(devNumber, channel, isOn);
            }
        }


        public bool GetRefMirrorVibration()
        {
            lock (_lock)
            {
                return APIWrapper.GetRefMirrorVibration(devNumber, channel);
            }
        }

        public void Start()
        {
            lock (_lock)
            {
                APIWrapper.Start(devNumber);
            }
        }


        public void Stop()
        {
            lock (_lock)
            {
                APIWrapper.Stop(devNumber);
            }
        }

        public double[] GetLenghtValues()
        {
            lock (_lock)
            {
                int value_count = APIWrapper.ValueCount(devNumber);
                if (value_count == 0)
                    return new double[0];

                APIWrapper.GetValues(devNumber);

                double[] to_ret = new double[value_count];
                for (int i = 0; i < value_count; i++)
                {
                    to_ret[i] = APIWrapper.LengthValue(devNumber, channel);
                }
                return to_ret;
            }
        }

        public void Close()
        {
            if (IsDeviceValid())
                APIWrapper.CloseDevice(devNumber);
        }
    }
}
