using ElmoMotionControl.GMAS.EASComponents.MMCLibDotNET.InternalArgs;
using SIOS_Interferometer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ush4.Models.SIOS
{
    public class SIOSManagerModel
    {
        SIOSManager sios_manager = new SIOSManager();

        private int _rate = 1000;

        public int Rate
        {
            get { return _rate; }
            set
            {
                _rate = value;
            }
        }

        public bool Start()
        {                        
            if(IsReadyForStart()) // 884
            {
                sios_manager.SetRefMirrorVibration(true);
                sios_manager.SetTrigger((uint)(SIOSEnums.Trigger.IFM_TRIGGER_OFF));
                sios_manager.SetMeasurement((int)(SIOSEnums.MeasurementFlags.IFM_MEAS_CH1 | SIOSEnums.MeasurementFlags.IFM_MEAS_LENGTH
                        | SIOSEnums.MeasurementFlags.IFM_MEAS_ONECHANNEL), _rate);

                //sios_manager.SetToZero();
                sios_manager.ResetBuffer(); // IfmClearBuffers() function in library

                //if (sios_manager.IsReadyForStart())
                {
                    sios_manager.Start();
                }

                return true;
            }

            return false;
        }

        public bool Reset()
        {
            sios_manager.SetToZero();
            //sios_manager.ResetBuffer(); // IfmClearBuffers() function in library

            return true;
        }

        public void Stop()
        {            
            if (sios_manager.IsDeviceValid())
            {
                sios_manager.Stop();                
            }
        }
        
        public double[] GetLenghtValues()
        {
            return sios_manager.GetLenghtValues();
        }

        public bool IsReadyForStart()
        {
            return sios_manager.IsReadyForStart();
        }

        public bool Connect()
        {
            return sios_manager.Open(sios_manager.SIOSSerialNumber);
        }

        public SIOSManagerModel()
        {
            sios_manager.SIOSSerialNumber = 883;            
        }

         ~SIOSManagerModel()
        {
            if (sios_manager.IsDeviceValid())
            {
                sios_manager.Stop();
                sios_manager.Close();
            }

            sios_manager.Close();
        }
    }
}
