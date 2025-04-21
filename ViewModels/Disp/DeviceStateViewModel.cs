using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ush4.ViewModels.Disp
{
    public class DeviceStateViewModel: ViewModel
    {
        protected Dispatcher CDispatcher = Dispatcher.CurrentDispatcher;
        const String TO_STRING_FORMAT = "{0, 10}: {1},  {2}";

        public enum enDeviceStates
        {
            Ok,
            Work,
            Warning,
            Error,
            Off
        }

        private enDeviceStates devState = enDeviceStates.Off;
        public enDeviceStates DeviceState
        {
            get { return devState; }
            set
            {
                devState = value;
                OnPropertyChanged("DeviceState");
            }
        }

        private String devStateDescription = "";
        public String StateDescription
        {
            get { return devStateDescription; }
            set
            {
                devStateDescription = value;
                OnPropertyChanged("StateDescription");
            }
        }

        private Exception exception = new Exception();
        public Exception ErrorException
        {
            get { return exception; }
            set
            {
                exception = value;
                OnPropertyChanged("ErrorException");
            }
        }

        private DateTime stateTime;
        public DateTime StateAppearingTime
        {
            get { return stateTime; }
            private set { stateTime = value;
                OnPropertyChanged("StateAppearingTime");
            }
        }


        //private String errorMessage = "";
        //public String ErrorMessage
        //{
        //    get { return errorMessage; }
        //    set
        //    {
        //        errorMessage = value;
        //        OnPropertyChanged("ErrorMessage");
        //    }
        //}




        private String owner = "";
        public String Owner
        {
            get { return owner; }
            set { owner = value; }
        }


        public DeviceStateViewModel()
        {
            StateAppearingTime = DateTime.Now;
        }

        public void SetValues(DeviceStateViewModel deviceStateViewModel)
        {
            StateAppearingTime = deviceStateViewModel.StateAppearingTime;
            Owner = deviceStateViewModel.Owner;
            DeviceState = deviceStateViewModel.DeviceState;
            ErrorException = deviceStateViewModel.ErrorException;
            StateDescription = deviceStateViewModel.StateDescription;
        }

        public static bool operator ==(DeviceStateViewModel a, DeviceStateViewModel b)
        {
            if (a.DeviceState != b.DeviceState)
                return false;
            if (a.StateDescription != b.StateDescription)
                return false;

            return true;
        }


        public static bool operator !=(DeviceStateViewModel a, DeviceStateViewModel b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (typeof(object) != typeof(DeviceStateViewModel))
                return false;

            return this == (DeviceStateViewModel)obj;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format(TO_STRING_FORMAT, Owner, DeviceState, StateDescription);
        }
    }
}
