using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using DynamicData;
using ReactiveUI;

namespace youtube_dl.WPF.Core.Queue
{
    public class DownloadCommandsQueue : IDisposable
    {
        private readonly YouTubeDL _youTubeDL;

        public DownloadCommandsQueue(
            YouTubeDL youTubeDL)
        {
            this._youTubeDL = youTubeDL ?? throw new ArgumentNullException(nameof(youTubeDL));

            //this._youTubeDLService.Downloading.Connect().CountChanged().Subscribe(x=>x.)
            this._whenAreDownloadsAutomaticallyStartedChanged = new BehaviorSubject<bool>(false).DisposeWith(this._disposables);
            this.WhenAreDownloadsAutomaticallyStartedChanged = this._whenAreDownloadsAutomaticallyStartedChanged.DistinctUntilChanged();

            // TODO: what if no async-await on handler?
            //this.Entries
            //    .Connect()
            //    .OnItemAdded(async x =>
            //    {
            //        if (this.AreDownloadsAutomaticallyStarted && this._youTubeDL.ExecutingInstances.Count < this.MaxConcurrentInstancesCount)
            //        {
            //            await this._youTubeDL.ExecuteCommandAsync(this.Dequeue());
            //        }
            //    });
            //this._youTubeDL.ExecutingInstances
            //    .Connect()
            //    .CountChanged()
            //    .Subscribe(async x =>
            //    {
            //        if (this.AreDownloadsAutomaticallyStarted && this._youTubeDL.ExecutingInstances.Count < this.MaxConcurrentInstancesCount)
            //        {
            //            await this._youTubeDL.ExecuteCommandAsync(this.Dequeue());
            //        }
            //    })
            //    .DisposeWith(this._disposables);
            Observable.Merge(
                this.Entries
                    .Connect()
                    .WhereReasonsAre(new[] { ListChangeReason.Add, ListChangeReason.AddRange, ListChangeReason.Refresh })
                    .Select(_ => Unit.Default),
                this._youTubeDL.ExecutingInstances
                    .Connect()
                    .WhereReasonsAre(new[] { ListChangeReason.Remove, ListChangeReason.RemoveRange, ListChangeReason.Refresh, ListChangeReason.Clear })
                    .Select(_ => Unit.Default))
                    .Subscribe(async x =>
                    {
                        if (this.AreDownloadsAutomaticallyStarted && this._youTubeDL.ExecutingInstances.Count < this.MaxConcurrentInstancesCount)
                        {
                            await this._youTubeDL.ExecuteCommandAsync(this.Dequeue());
                        }
                    })
                    .DisposeWith(this._disposables);
        }

        public ushort MaxConcurrentInstancesCount { get; } = 5;

        public bool AreDownloadsAutomaticallyStarted
        {
            get { return this._whenAreDownloadsAutomaticallyStartedChanged.Value; }
            set { this._whenAreDownloadsAutomaticallyStartedChanged.OnNext(value); }
        }
        private readonly BehaviorSubject<bool> _whenAreDownloadsAutomaticallyStartedChanged;
        public IObservable<bool> WhenAreDownloadsAutomaticallyStartedChanged { get; }

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

        public Task StartDownloadsAsync()
        {
            var comm = this.Dequeue();
            return this._youTubeDL.ExecuteCommandAsync(comm);
        }

        #region IDisposable

        // https://docs.microsoft.com/en-us/dotnet/api/system.idisposable?view=netframework-4.8
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isDisposed = false;

        // use this in derived class
        // protected override void Dispose(bool isDisposing)
        // use this in non-derived class
        protected virtual void Dispose(bool isDisposing)
        {
            if (this._isDisposed)
                return;

            if (isDisposing)
            {
                // free managed resources here
                this._disposables.Dispose();
            }

            // free unmanaged resources (unmanaged objects) and override a finalizer below.
            // set large fields to null.

            this._isDisposed = true;

            // remove in non-derived class
            //base.Dispose(isDisposing);
        }

        // remove if in derived class
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool isDisposing) above.
            this.Dispose(true);
        }

        #endregion
    }
}
