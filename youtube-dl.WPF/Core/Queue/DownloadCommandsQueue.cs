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
using System.Reactive.Subjects;

namespace youtube_dl.WPF.Core.Services
{
    public class DownloadCommandsQueue
    {
        private readonly YouTubeDL _youTubeDL;

        public DownloadCommandsQueue(
            YouTubeDL youTubeDL)
        {
            this._youTubeDL = youTubeDL ?? throw new ArgumentNullException(nameof(youTubeDL));

            //this._youTubeDLService.Downloading.Connect().CountChanged().Subscribe(x=>x.)
        }

        public ushort MaxConcurrentInstancesCount { get; } = 5;

        private readonly ISourceList<DownloadCommand> _queueEntries = new SourceList<DownloadCommand>();
        public IObservableList<DownloadCommand> Entries => this._queueEntries;

        public void Enqueue(DownloadCommand entry)
        {
            this._queueEntries.Add(entry);
        }
        public void Enqueue(params DownloadCommand[] entries)
        {
            foreach (var entry in entries)
            {
                this._queueEntries.Add(entry);
            }
        }

        public DownloadCommand Dequeue()
        {
            DownloadCommand firstItem = null;

            this._queueEntries.Edit(list =>
            {
                firstItem = list.FirstOrDefault();

                if (firstItem != null)
                {
                    list.RemoveAt(0);
                }
            });

            return firstItem;
        }

        public void EmptyQueue()
        {
            this._queueEntries.Clear();
        }

        public void Remove(int index)
        {
            this._queueEntries.Edit(list => list.RemoveAt(index));
        }
        public void Remove(DownloadCommand entry)
        {
            this._queueEntries.Edit(list => list.Remove(entry));
        }
    }
}
