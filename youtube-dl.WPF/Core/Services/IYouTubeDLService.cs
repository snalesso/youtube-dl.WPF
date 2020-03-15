using DynamicData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using youtube_dl.WPF.Core.Models;

namespace youtube_dl.WPF.Core.Services
{
    // TODO: consider making it a base class + dummy extending class
    public interface IYouTubeDLService
    {
        string DownloadsFolderPath { get; }
        string ExeFilePath { get; }

        bool IsBusy { get; }
        IObservable<bool> WhenIsBusyChanged { get; }

        IObservableList<DownloadQueueEntry> DownloadingBatchQueue { get; }

        Task<DownloadHistoryEntry> DownloadAsync(DownloadQueueEntry entry);
        Task<bool> UpdateAsync();
    }
}