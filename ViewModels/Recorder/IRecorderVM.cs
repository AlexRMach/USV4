using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ush4.ViewModels.Recorder
{
    public interface IRecorderVM
    {
        Boolean IsConnected { get; }
        Boolean IsRecording { get; }
        TimeSpan RecordCountdown { get; }

        Task<Boolean> ConnectAsync();

        void Dispose();

        Boolean ConfigureRecorder(Double duration);

        Boolean IsRecordingNow();

        Boolean StartRecorder();

        Boolean StopRecorder();

        Task<DataPoint[]> DownloadRecorderedData();

        //RelayCommand ConnectCommand { get; }

        //RelayCommand DisconnectCommand { get; }

    }
}
