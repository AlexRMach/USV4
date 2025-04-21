using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.WPF.ViewModels;

namespace ush4.ViewModels
{
    public class RemoteControlVM : ViewModel
    {
        private bool _is_remote_en = false;

        public bool IsRemoteEnabled
        {
            get { return _is_remote_en; }
            set
            {
                if (value != _is_remote_en)
                {
                    //SetPlayPlot();

                    _is_remote_en = value;

                    OnPropertyChanged("IsRemoteEnabled");                    
                }
            }
        }

        private bool _is_remote = false;

        public bool IsRemote
        {
            get { return _is_remote; }
            set
            {
                if (value != _is_remote)
                {
                    //SetPlayPlot();

                    _is_remote = value;

                    if (_is_local)
                    {
                        _is_remote = false;
                    }

                    OnPropertyChanged("IsRemote");
                }
            }
        }

        private bool _is_local = false;

        public bool IsLocale
        {
            get { return _is_local; }
            set
            {
                if (value != _is_local)
                {
                    //SetPlayPlot();

                    _is_local = value;

                    if(_is_local)
                    {
                        _is_remote = false;
                    }

                    OnPropertyChanged("IsLocale");
                }
            }
        }

        public RemoteControlVM() 
        {
            IsRemoteEnabled = false;

            IsRemote = true;

            IsLocale = false;
        }
    }
}
