using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.IO;
using ush4.Services.NLogger;
using ush4.Services.Serialization;
using ush4.ViewModels;

namespace ush4.ViewModels.Disp
{
    public class DeviceViewModel : ViewModel
    {
        protected static String SETTINGS_FOLDER = Path.Combine(Environment.CurrentDirectory, "Settings");
        public static event Action<Object, DeviceStateViewModel> NewDeviceStatusEvent;
        protected Dispatcher CDispatcher = Dispatcher.CurrentDispatcher;
        //protected DispatcherTaskScheduler taskScheduler = new DispatcherTaskScheduler();
        //protected ta
        private DeviceStateViewModel state = new DeviceStateViewModel();
        public DeviceStateViewModel State
        {
            get { return state; }
            set
            {
                state = value;
                OnPropertyChanged("State");
            }
        }

        private String deviceName = "Default";

        public String DeviceName
        {
            get { return deviceName; }
            set
            {
                deviceName = value;
                OnPropertyChanged("DeviceName");
            }
        }       

        //private String settings_folder = Path.Combine(Environment.CurrentDirectory, "Settings");
        //protected String SettingsFolder
        //{
        //    get { return settings_folder; }
        //    set { settings_folder = value; }
        //}


        public virtual void SetNewStatus(DeviceStateViewModel deviceStatus)
        {
            if (deviceStatus == State)
                return;

            State = deviceStatus;
            State.Owner = DeviceName;

            ErrorStatusToLog(deviceStatus);
        }

        private void ErrorStatusToLog(DeviceStateViewModel deviceStatus)
        {
            if ((int)deviceStatus.DeviceState >= (int)DeviceStateViewModel.enDeviceStates.Error)
                LoggerMessenger.Error(deviceStatus.ErrorException, deviceStatus.StateDescription);

            if (NewDeviceStatusEvent != null)
                NewDeviceStatusEvent(this, deviceStatus);
        }

        public void SetNewStatusDispatcher(DeviceStateViewModel deviceStateViewModel)
        {
            CDispatcher.BeginInvoke(
               (Action)(() =>
               {
                   SetNewStatus(deviceStateViewModel);

               }));
        }

        public virtual void SetNewStatus(DeviceStateViewModel.enDeviceStates state, String description, Exception exception)
        {
            DeviceStateViewModel deviceStatus = new DeviceStateViewModel();
            //deviceStatus.Owner = DeviceName;
            deviceStatus.DeviceState = state;
            deviceStatus.StateDescription = description;
            deviceStatus.ErrorException = exception;
            //State = deviceStatus;
            SetNewStatus(deviceStatus);
           // ErrorStatusToLog(deviceStatus);
        }

        public void SetNewStatus(DeviceStateViewModel.enDeviceStates state, String description)
        {
            SetNewStatus(state, description, null);
        }

        public void SetNewStatusDispatcher(DeviceStateViewModel.enDeviceStates state, String description, Exception exception)
        {
            CDispatcher.BeginInvoke(
               (Action)(() =>
               {
                   SetNewStatus(state, description, exception);

               }));
            
            
        }

        public void SetNewStatusDispatcher(DeviceStateViewModel.enDeviceStates state, String description)
        {
            SetNewStatusDispatcher(state, description, null);
        }

        public virtual void Dispose() { }




        protected virtual void CreateSettingsVM()
        {
            throw new Exception("Create settings method does not realized!");
        }

        protected virtual void LoadSettingsVM(String file_name)
        {

            throw new Exception("Load settings method does not realized!");
        }

        protected virtual void SaveSettingsVM(String file_name, Object data)
        {

            SaveAndLoadBySerialization.SaveData(file_name, data);
        }

        public virtual void DeviceSettingsInitialization()
        {
            String path = Path.Combine(SETTINGS_FOLDER, DeviceName);
            //if (File.Exists(path))
            //{
            //    LoadSettingsVM(path);
            //}
            //else
            //{
            //    CreateSettingsVM();
            //}
            try
            {
                LoadSettingsVM(path);
            }
            catch(Exception)
            {
                CreateSettingsVM();
            }
        }
    }
}
