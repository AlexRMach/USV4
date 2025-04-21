using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace ush4.ViewModels.Disp
{
    public class Log_ViewModel: DeviceViewModel
    {
        private ObservableCollection<DeviceStateViewModel> states_collection = new ObservableCollection<DeviceStateViewModel>();
        public ObservableCollection<DeviceStateViewModel> StateCollection
        {
            get { return states_collection; }
            private set { states_collection = value;
                OnPropertyChanged("StateCollection");
            }
        }

        private DeviceStateViewModel lastState;

        public DeviceStateViewModel LastState
        {
            get { return lastState; }
            set { lastState = value;
                OnPropertyChanged("LastState");
            }
        }

        public void NewDeviceStatusHandler(Object sender, DeviceStateViewModel deviceStateViewModel)
        {
            //StateCollection.Add(deviceStateViewModel);
            CDispatcher.BeginInvoke(
                       (Action)(() =>
                       {
                           if ((deviceStateViewModel.StateDescription != "The motor is not enabled.") && (deviceStateViewModel.StateDescription != "The motor is enabled.")
                           && (deviceStateViewModel.StateDescription != "Personality loaded from file."))
                           {
                               StateCollection.Add(deviceStateViewModel);

                               LastState = deviceStateViewModel;
                           }
                       }));
        }
    }
}
