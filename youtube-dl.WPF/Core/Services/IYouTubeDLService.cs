using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using youtube_dl.WPF.Core.Models;

namespace youtube_dl.WPF.Core.Services
{
    public interface IYouTubeDLService
    {
        string OfficialWebsite { get; }
        string RepositoryWebSite { get; }
        string DownloadsFolderPath { get; }

        bool IsBusy { get; }
        IObservable<bool> WhenIsBusyChanged { get; }

        IReadOnlyReactiveList<DownloadQueueEntry> DownloadingBatchQueue { get; }

        Task<DownloadHistoryEntry> DownloadAsync(DownloadQueueEntry entry);
        Task<bool> UpdateAsync();
    }
}