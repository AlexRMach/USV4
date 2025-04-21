using ElmoMotionControlComponents.Drive.EASComponents;
using ElmoMotionControlComponents.Drive.EASComponents.Locator;
using ElmoMotionControlComponents.Drive.EASComponents.UploadsAndDownloads;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ush4.Models.ELMO
{
    public class DriveModel
    {
        protected IDriveCommunication driveCommunication;
        //protected Object _lock = new Object();
        public event Action<IUploadDownloadModel, EventArgs> UploadModelEvent;
        protected ElmoHandler elmoHandler;
        private String personalityFilePath;
        public String PersonalityFilePath
        {
            get { return personalityFilePath; }
            set { personalityFilePath = value; }
        }

        [XmlIgnore]
        public ElmoHandler Commands { get { return elmoHandler;} protected set { elmoHandler = value; } }

        
        private String ip_address;
        
        public String IP_Address
        {
            get { return ip_address; }
            set { ip_address = value; }
        }

        private int updateTime;

        public int UpdateTime_ms
        {
            get { return updateTime; }
            set { updateTime = value; }
        }

        public String SerialNumber { get; set; }


        public DriveModel()
        {
            PersonalityFilePath = "Personality.xml";//Path.Combine(Directory.GetCurrentDirectory(), "Personality.xml");
            UpdateTime_ms = 500;// 500;
            IP_Address = "";
            SerialNumber = "";
        }

        

        public virtual void Connect()
        {
            IPAddress iPAddress;
            if (SerialNumber != "")
                driveCommunication = GetCommunication(SerialNumber);
            else if (IPAddress.TryParse(IP_Address, out iPAddress))
                driveCommunication = GetCommunication(iPAddress);
            else
                driveCommunication = GetCommunication();
            IDriveErrorObject err;
            if (!driveCommunication.Connect(out err))
            {
                throw new Exception(Properties.ResourcesE.UnableToConnect +
                    String.Format(Properties.ResourcesE.DeviceInfo, driveCommunication.ProductInfo.Name, driveCommunication.CommunicationInfo.CommType) +
                    String.Format(Properties.ResourcesE.Connection_info, driveCommunication.CommunicationInfo.IPCommInfo.Address) +
                    ElmoCommandsEnum.DriveErrorObjectToString(err));
            }
            elmoHandler = new ElmoHandler(driveCommunication);

           // return communication;
        }

        public virtual void Disconnect()
        {
            if (driveCommunication != null)
            {
                if (!driveCommunication.Disconnect())
                    throw new Exception(Properties.ResourcesE.UnableToDisconnect);
            }
        }


        protected IDriveCommunication GetCommunication(String serialNumber)
        {
            IList<KeyValuePair<string, IDriveCommunicationInfo>> devices = DriveLocator.GetUDPDevices();
            if (devices.Count > 0)
            {
                foreach (var d in devices)
                {
                    var ipinfo = d.Value;
                    if (d.Key == serialNumber)
                        return DriveCommunicationFactory.CreateCommunication(ipinfo);
                }
                throw new Exception(Properties.ResourcesE.DeviceDoesNotFound);
            }
            else
                throw new Exception(Properties.ResourcesE.DeviceDoesNotFound);
        }

        protected IDriveCommunication GetCommunication(IPAddress iPAddress)
        {
            IList<KeyValuePair<string, IDriveCommunicationInfo>> devices = DriveLocator.GetUDPDevices();
            if (devices.Count > 0)
            {
                foreach (var d in devices)
                {
                    var ipinfo = d.Value;
                    if (ipinfo.IPCommInfo.Address == iPAddress)
                        return DriveCommunicationFactory.CreateCommunication(ipinfo);
                }
                throw new Exception(Properties.ResourcesE.DeviceDoesNotFound);
            }
            else
                throw new Exception(Properties.ResourcesE.DeviceDoesNotFound);
        }


        protected IDriveCommunication GetCommunication()
        {
            IList<KeyValuePair<string, IDriveCommunicationInfo>> devices = DriveLocator.GetUDPDevices();
            if (devices.Count > 0)
            {
                if (devices.Count == 1)
                    return DriveCommunicationFactory.CreateCommunication(devices[0].Value);
                throw new Exception("More then one device found");
            }
            else
                throw new Exception(Properties.ResourcesE.DeviceDoesNotFound);
        }

        public virtual void CreatePersonalityModelFromFile()
        {
            IDriveErrorObject errorObject;
            if (!driveCommunication.CreatePersonalityModel(PersonalityFilePath, out errorObject))
            {
                
                if (errorObject == null)
                    throw new Exception(Properties.ResourcesE.Cannot_create_Personality_Model_from_file);
                else
                    throw new Exception(Properties.ResourcesE.Cannot_create_Personality_Model_from_file + 
                        ElmoCommandsEnum.DriveErrorObjectToString(errorObject));
            }
        }

        public virtual void UploadPersonalityFromDrive()
        {

            IDriveErrorObject errorObject;
            //if (!File.Exists(PersonalityFilePath))
            //    File.Create(PersonalityFilePath);

            IUploadDownloadModel model = driveCommunication.UploadPersonality(Path.Combine(Environment.CurrentDirectory, PersonalityFilePath), 
                out errorObject);
            if (model == null)
            {
                throw new Exception(Properties.ResourcesE.Cannot_start_upload_Personality_Model_from_drive + 
                    ElmoCommandsEnum.DriveErrorObjectToString(errorObject));   
            }

            model.OnStart += UploadModelEventHandle;
            model.OnProgress += UploadModelEventHandle;
            model.OnFinish += UploadModelEventHandle;
            model.OnFailed += UploadModelEventHandle;
            model.OnCancel += UploadModelEventHandle;

            model.Start(out errorObject);

            
        }

        protected virtual void UploadModelEventHandle(object sender, EventArgs e)
        {
            IUploadDownloadModel model = sender as IUploadDownloadModel;
            if (UploadModelEvent != null)
                UploadModelEvent(model, e);

            if((model.OperationStatus != OPERATION_STATUS.PROGRESSED) && (model.OperationStatus != OPERATION_STATUS.STARTED))
            {
                model.OnStart -= UploadModelEventHandle;
                model.OnProgress -= UploadModelEventHandle;
                model.OnFinish -= UploadModelEventHandle;
                model.OnFailed -= UploadModelEventHandle;
                model.OnCancel -= UploadModelEventHandle;
                if(model.OperationStatus == OPERATION_STATUS.FINISHED)
                {
                    CreatePersonalityModelFromFile();
                }
            }
            
        }


        //protected void SendCommandToDrive(String command, out string reply)
        //{
        //    IDriveErrorObject err;
            
        //    if (!driveCommunication.SendCommandAnalyzeError(command, out reply, out err, timeout))
        //    {
        //        if (err != null)
        //            throw new Exception(Properties.Resources.ErrorWhileWrite + DriveErrorObjectToString(err));
        //        else
        //            throw new Exception(Properties.Resources.ErrorWhileWrite);
        //    }
            
        //}

        
    }
}
