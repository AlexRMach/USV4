using ush4.Services.Serialization;
using ush4.ViewModels.Disp;
using ElmoMotionControlComponents.Drive.EASComponents.UploadsAndDownloads;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ush4.Models.ELMO;

namespace ush4.ViewModels.ELMO
{
    public class DriveViewModel<T>: DeviceWithStatusesViewModel<T> where T: DriveModel, new()
    {
        //protected T model;
        protected Thread statuStread;
       // protected Action<int> StatusRegisterAuxAction;

        //private ObservableCollection<DeviceStateViewModel> statusesCollection = new ObservableCollection<DeviceStateViewModel>();
        //public ObservableCollection<DeviceStateViewModel> DeviceStatusesCollection
        //{
        //    get { return statusesCollection; }
        //    set { statusesCollection = value;
        //        OnPropertyChanged("DeviceStatusesCollection");
        //    }
        //}


        private Boolean isConnected = false;

        // Свойство!
        public Boolean IsConnected
        {
            get { return isConnected; }
            protected set { isConnected = value; }
        }


        private Boolean isMotorOn = false;
        public Boolean IsMotorEnable
        {
            get { return isMotorOn; }
            protected set
            { isMotorOn = value;
                OnPropertyChanged("IsMotorEnable");
            }
        }

        private bool isSTO_Active = false;
        public Boolean IsSTO_Active
        {
            get { return isSTO_Active; }
            protected set { isSTO_Active = value;
                OnPropertyChanged("IsSTO_Active");
            }
        }

        private Boolean isMoving = false;

        // Свойство!
        public Boolean IsMoving
        {
            get { return isMoving; }
            set { isMoving = value; }
        }

        private Boolean getSinData = false;

        // Свойство!
        public Boolean GetSinData
        {
            get { return getSinData; }
            set { getSinData = value; }
        }

        public DriveViewModel()
        {
            DeviceName = "Elmo drive";
        }

        public Boolean Connect()
        {
            try
            {
                DeviceSettingsInitialization();
                model.Connect();
                statuStread = new Thread(StatusThread) { IsBackground = true };
                SetNewStatus(DeviceStateViewModel.enDeviceStates.Ok, Properties.ResourcesE.DeviceConnected);
                IsConnected = true;
                if(UploadPersonalityFromFile())
                {
                    statuStread.Start();
                    return true;
                }
                
                Boolean is_upload = StartUploadPersonalityFromDrive();
                if (!is_upload)
                    Dispose();

                return is_upload;
                

                //return true;
            }
            catch (Exception ex)
            {
                SetNewStatus(DeviceStateViewModel.enDeviceStates.Error, Properties.ResourcesE.UnableToConnect, ex);
                IsConnected = false;
                return false;
            }
        }

        public virtual async Task<Boolean> ConnectAsync()
        {
            try
            {
                DeviceSettingsInitialization();
                await Task.Run((Action)model.Connect);
                statuStread = new Thread(StatusThread) { IsBackground = true };
                SetNewStatus(DeviceStateViewModel.enDeviceStates.Ok, Properties.ResourcesE.DeviceConnected);
                IsConnected = true;
                if (UploadPersonalityFromFile())
                {
                    statuStread.Start();
                    return true;
                }

                Boolean is_upload = StartUploadPersonalityFromDrive();
                if (!is_upload)
                    Dispose();

                return is_upload;
            }
            catch (Exception ex)
            {
                SetNewStatus(DeviceStateViewModel.enDeviceStates.Error, Properties.ResourcesE.UnableToConnect, ex);
                IsConnected = false;
                return false;
            }
        }
        

        public override void Dispose()
        {
            try
            {
                if ((model != null) && (IsConnected))
                {
                    model.Disconnect();
                    IsConnected = false;
                    SetNewStatus(DeviceStateViewModel.enDeviceStates.Off, Properties.ResourcesE.DeviceDisconnected);
                }
            }
            catch (Exception ex)
            {
                SetNewStatus(DeviceStateViewModel.enDeviceStates.Error, Properties.ResourcesE.UnableToDisconnect, ex);
            }
        }

        private Boolean UploadPersonalityFromFile()
        {
            try
            {
                model.CreatePersonalityModelFromFile();
                SetNewStatus(DeviceStateViewModel.enDeviceStates.Ok, Properties.ResourcesE.PersonalityLoadedFromFile);
                return true;
            }
            catch (Exception ex)
            {
                SetNewStatus(DeviceStateViewModel.enDeviceStates.Warning, Properties.ResourcesE.UnableToLoadFromFile, ex);
                return false;
            }
        }


        private void StatusThread()
        {
            while (IsConnected)
            {
                try
                {
                    int status = model.Commands.StatusRegister();

                    StatusRegisterToDVM_Properties(status);

                    CDispatcher.BeginInvoke(
                        (Action)(()=> 
                        {
                            ParseAndSetStatus(status);
                        }));
                    Thread.Sleep(model.UpdateTime_ms);
                }
                catch (Exception ex)
                {
                    SetNewStatusDispatcher(DeviceStateViewModel.enDeviceStates.Error, 
                        Properties.ResourcesE.An_error_occurred_while_polling_the_device_status, ex);
                    Dispose();
                }
            }
        }

        private void StatusRegisterToDVM_Properties(int status_register)
        {
            IsMotorEnable = DriveStatusParser.IsBitsIsSet(status_register, (int)DriveStatusParser.enstatusRegisters.MotorEnable);
            IsSTO_Active = !DriveStatusParser.IsBitsIsSet(status_register, (int)DriveStatusParser.enstatusRegisters.STI1);
        }

        private void ParseAndSetStatus(int status)
        {
            List<DeviceStateViewModel> deviceStates = DriveStatusParser.ParseStatus(status);
            DeviceStateViewModel tmp_status = new DeviceStateViewModel()
            {
                DeviceState = DeviceStateViewModel.enDeviceStates.Ok,
                StateDescription = "Ok"
            };
            DeviceStatusesCollection.Clear();
            foreach (var item in deviceStates)
            {
                DeviceStatusesCollection.Add(item);
                if ((int)tmp_status.DeviceState <= (int)item.DeviceState)
                    tmp_status = item;
            }
            if (tmp_status != this.State)
                SetNewStatusDispatcher(tmp_status);
        }

        private Boolean StartUploadPersonalityFromDrive()
        {
            try
            {
                model.UploadModelEvent += UploadPersonalityHandle;
                model.UploadPersonalityFromDrive();
                return true;
            }
            catch (Exception ex)
            {
                model.UploadModelEvent -= UploadPersonalityHandle;
                SetNewStatus(DeviceStateViewModel.enDeviceStates.Warning, 
                    Properties.ResourcesE.Unable_to_start_upload_personality_from_drive, ex);
                return false;
            }
        }

        private void UploadPersonalityHandle(IUploadDownloadModel uploadDownloadModel, Object event_args)
        {
           
            switch(uploadDownloadModel.OperationStatus)
            {
                case OPERATION_STATUS.STARTED:
                    SetNewStatusDispatcher(DeviceStateViewModel.enDeviceStates.Work,
                        Properties.ResourcesE.UploadStarted);
                    return;
                case OPERATION_STATUS.PROGRESSED:
                    SetNewStatusDispatcher(DeviceStateViewModel.enDeviceStates.Work,
                        String.Format(Properties.ResourcesE.Uploaded_from_drive, uploadDownloadModel.Percent));
                    return;
                case OPERATION_STATUS.FINISHED:
                    SetNewStatusDispatcher(DeviceStateViewModel.enDeviceStates.Ok, Properties.ResourcesE.UploadFinished);
                    statuStread.Start();
                    break;
                default:
                    SetNewStatusDispatcher(DeviceStateViewModel.enDeviceStates.Error,
                    String.Format(Properties.ResourcesE.UploadInterrapted, uploadDownloadModel.OperationStatus.ToString()));
                    this.Dispose();
                    break;
            }

            model.UploadModelEvent -= UploadPersonalityHandle;
        }

        public override void DeviceSettingsInitialization()
        {
            base.DeviceSettingsInitialization();
        }

        //#region LoadAndCreation
        //protected override void CreateSettingsVM()
        //{
        //    driveModel = new Models.DriveModel();

        //    Common.Instruments.FileAndDirectotyManipulation.CreateDirectory(SETTINGS_FOLDER);
        //    String path = Path.Combine(SETTINGS_FOLDER, DeviceName);
        //    SaveSettingsVM(path, driveModel);

        //    SetNewStatus(DeviceStateViewModel.enDeviceStates.Error, Properties.Resources.Configuration_file_of_device_does_not_found);
        //}

        //protected override void LoadSettingsVM(string file_name)
        //{
        //    driveModel = (Models.DriveModel)SaveAndLoadBySerialization.LoadData(file_name, typeof(Models.DriveModel));
        //}

        //protected override void SaveSettingsVM(string file_name, object data)
        //{
        //    SaveAndLoadBySerialization.SaveData(file_name, data);
        //}
        //#endregion
    }
}
