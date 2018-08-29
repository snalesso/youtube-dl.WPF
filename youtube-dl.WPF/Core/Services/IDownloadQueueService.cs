using System.Collections.Generic;
using ReactiveUI;
using youtube_dl.WPF.Core.Models;

namespace youtube_dl.WPF.Core.Services
{
    public interface IDownloadQueueService
    {
        IReadOnlyReactiveList<DownloadQueueEntry> QueueEntries { get; }

        DownloadQueueEntry Dequeue();
        void Enqueue(DownloadQueueEntry item);
        void Enqueue(IReadOnlyList<DownloadQueueEntry> items);
        DownloadQueueEntry Extract(DownloadQueueEntry item);
        void Empty();
    }
}