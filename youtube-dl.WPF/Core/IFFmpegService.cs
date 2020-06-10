using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace youtube_dl.WPF.Core
{
    public interface IFFmpegService
    {
        string OfficialWebsite { get; }
        string DirectDownloadLink { get; }

        bool IsBusy { get; }
        IObservable<bool> WhenIsBusyChanged { get; }

        Task<DownloadHistoryEntry> DownloadAsync(DownloadQueueEntry entry);
        Task<bool> UpdateAsync();
    }
}
