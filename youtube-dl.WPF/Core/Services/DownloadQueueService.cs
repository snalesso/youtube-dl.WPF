using ReactiveUI;
using ReactiveUI.Wpf;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.PlatformServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using youtube_dl.WPF.Core.Models;
using DynamicData;

namespace youtube_dl.WPF.Core.Services
{
    public class DownloadQueueService : IDownloadQueueService
    {
        public DownloadQueueService() { }

        private readonly ISourceList<DownloadQueueEntry> _queueEntries = new SourceList<DownloadQueueEntry>(
            //new[]
            //{
            //    new DownloadQueueEntry(@"")
            //}
            );
        public IObservableList<DownloadQueueEntry> QueueEntries => this._queueEntries;

        public void Enqueue(DownloadQueueEntry entry)
        {
            this._queueEntries.Add(entry);
        }

        public void Enqueue(IReadOnlyList<DownloadQueueEntry> entry)
        {
            this._queueEntries.AddRange(entry);
        }

        public DownloadQueueEntry Dequeue()
        {
            var first = this._queueEntries.Items.ElementAtOrDefault(0);

            if (first != null)
                this._queueEntries.RemoveAt(0);

            return first;
        }

        public DownloadQueueEntry Extract(DownloadQueueEntry entry)
        {
            if (this._queueEntries.Items.Contains(entry))
                this._queueEntries.Remove(entry);

            return entry;
        }

        public void Empty()
        {
            this._queueEntries.Clear();
        }
    }
}
