using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ush4.Services.Serialization;

namespace ush4.ViewModels.Disp
{

    public class DeviceWithStatusesViewModel : DeviceViewModel 
    {
      

        private ObservableCollection<DeviceStateViewModel> statusesCollection = new ObservableCollection<DeviceStateViewModel>();
        public ObservableCollection<DeviceStateViewModel> DeviceStatusesCollection
        {
            get { return statusesCollection; }
            set
            {
                statusesCollection = value;
                OnPropertyChanged("DeviceStatusesCollection");
            }
        }


        protected virtual void SetStatesToCollection(List<DeviceStateViewModel> states)
        {
            DeviceStateViewModel tmp_status = new DeviceStateViewModel()
            {
                DeviceState = DeviceStateViewModel.enDeviceStates.Ok,
                StateDescription = "Ok"
            };
            DeviceStatusesCollection.Clear();
            DeviceStatusesCollection.Add(tmp_status);

            foreach (var item in states)
            {
                DeviceStatusesCollection.Add(item);
                if ((int)tmp_status.DeviceState <= (int)item.DeviceState)
                    tmp_status = item;
            }
            if (tmp_status != this.State)
                SetNewStatusDispatcher(tmp_status);
        }

    }


    public class DeviceWithStatusesViewModel<T>: DeviceWithStatusesViewModel where T : new()
    {
        protected T model;               

        protected override void CreateSettingsVM()
        {
            model = new T();

            Common.Instruments.FileAndDirectotyManipulation.CreateDirectory(SETTINGS_FOLDER);
            String path = Path.Combine(SETTINGS_FOLDER, DeviceName);
            SaveSettingsVM(path, model);

            SetNewStatus(DeviceStateViewModel.enDeviceStates.Error, "Configuration file of device does not found");
        }


        protected override void LoadSettingsVM(string file_name)
        {
            //controllerModel = (Model.ControllerModel)SaveAndLoadBySerialization.LoadData(file_name, typeof(Model.ControllerModel));

            //String jsonString = File.ReadAllText(file_name);
            //model = JsonConvert.DeserializeObject<T>(jsonString);
            model = Services.Serialization.SerializationByJson.Deserialize<T>(file_name); // AlexM 250324
        }

        protected override void SaveSettingsVM(string file_name, object data)
        {

            //string jsonString;
            //jsonString = JsonConvert.SerializeObject(data);//, new JsonSerializerOptions()
            ////{
            ////    WriteIndented = true,
            ////    IgnoreReadOnlyProperties = true
            ////});
            //File.WriteAllText(file_name, jsonString);
            Services.Serialization.SerializationByJson.Serialize(file_name, data); // AlexM 250324

        }
    }

    
}
