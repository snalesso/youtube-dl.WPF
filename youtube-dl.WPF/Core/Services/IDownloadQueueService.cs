using System.Collections.Generic;
using DynamicData;
using youtube_dl.WPF.Core.Models;

namespace youtube_dl.WPF.Core.Services
{
    public interface IDownloadQueueService
    {
        IObservableList<DownloadQueueEntry> QueueEntries { get; }

        DownloadQueueEntry Dequeue();
        void Enqueue(DownloadQueueEntry item);
        void Enqueue(IReadOnlyList<DownloadQueueEntry> items);
        DownloadQueueEntry Extract(DownloadQueueEntry item);
        void Empty();
    }
}