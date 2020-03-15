using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using ReactiveUI;
using youtube_dl.WPF.Core.Models;

namespace youtube_dl.WPF.Core.Services
{
    internal class DummyDownloadQueueService : IDownloadQueueService
    {
        private DummyDownloadQueueService()
        {
            var dummyEntries = new[]
            {
                new DownloadQueueEntry("https://www.youtube.com/watch?v=1BoJtPp9oQY", new DownloadOptions( DownloadMode.AudioOnly)),
                new DownloadQueueEntry("https://www.youtube.com/watch?v=8UVNT4wvIGY", new DownloadOptions( DownloadMode.AudioVideo)),
                new DownloadQueueEntry("https://www.youtube.com/watch?v=s31XTrGJchQ", new DownloadOptions( DownloadMode.AudioOnly)),
            };

            var sl = new SourceList<DownloadQueueEntry>();
            sl.AddRange(dummyEntries);
            this.QueueEntries = sl;
        }

        private static DummyDownloadQueueService _instance = new DummyDownloadQueueService();
        public static DummyDownloadQueueService Instance => DummyDownloadQueueService._instance;

        public IObservableList<DownloadQueueEntry> QueueEntries { get; }

        public DownloadQueueEntry Dequeue()
        {
            throw new NotImplementedException();
        }

        public void Enqueue(DownloadQueueEntry item)
        {
            throw new NotImplementedException();
        }

        public void Enqueue(IReadOnlyList<DownloadQueueEntry> items)
        {
            throw new NotImplementedException();
        }

        public DownloadQueueEntry Extract(DownloadQueueEntry item)
        {
            throw new NotImplementedException();
        }

        public void Empty()
        {
            throw new NotImplementedException();
        }
    }
}
