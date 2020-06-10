using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using DynamicData;
using youtube_dl.WPF.Process;

namespace youtube_dl.WPF.Core.Models
{
    public class YouTubeDLInstanceObserver : IDisposable
    {
        //private readonly DownloadCommandOptions _downloadOptions;
        private readonly Uri _youtubeDlExeFilePath;
        private readonly IObservable<Unit> _pollTicker = Observable.Interval(TimeSpan.FromMilliseconds(500)).Select(x => Unit.Default);

        public YouTubeDLInstanceObserver(
            Uri youtubeDlExeFilePath,
            DownloadCommand command)
        {
            this._youtubeDlExeFilePath = youtubeDlExeFilePath ?? throw new ArgumentNullException(nameof(youtubeDlExeFilePath));
            this.Command = command ?? throw new ArgumentNullException(nameof(command));

            this._outputsSourceList = new SourceList<string>().DisposeWith(this._disposables);
            this._errorsSourceList = new SourceList<string>().DisposeWith(this._disposables);

            this._whenDownloadStatusChanged_behaviorSubject = new BehaviorSubject<YouTubeDLInstanceObserverStatus>(YouTubeDLInstanceObserverStatus.Ready).DisposeWith(this._disposables);
            this.WhenStatusChanged = this._whenDownloadStatusChanged_behaviorSubject.DistinctUntilChanged();
        }

        public DownloadCommand Command { get; }

        // TODO: ensure, when process ends, all disposables linked to process life time get disposed and the object becomes "frozen"
        public Task ExecuteAsync()
        {
            var psi = new ProcessStartInfo()
            {
                Arguments = this.Command.Options.ToString(),
                FileName = this._youtubeDlExeFilePath.LocalPath,
                CreateNoWindow = true
            };

            return ProcessUtils.RunAsync(
                psi,
                output => this._outputsSourceList.Edit(list => list.Add(output)),
                error => this._errorsSourceList.Edit(list => list.Add(error)),
                (exitCode) => this._whenDownloadStatusChanged_behaviorSubject.OnNext(exitCode == 0 ? YouTubeDLInstanceObserverStatus.Completed : YouTubeDLInstanceObserverStatus.Failed),
                CancellationToken.None);
        }

        private readonly BehaviorSubject<YouTubeDLInstanceObserverStatus> _whenDownloadStatusChanged_behaviorSubject;
        public IObservable<YouTubeDLInstanceObserverStatus> WhenStatusChanged { get; }

        private readonly ISourceList<string> _outputsSourceList;
        public IObservableList<string> Status => this._outputsSourceList.AsObservableList();
        private readonly ISourceList<string> _errorsSourceList;
        public IObservableList<string> Errors => this._errorsSourceList.AsObservableList();

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