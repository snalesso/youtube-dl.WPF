using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using DynamicData;
using youtube_dl.WPF.Process;

namespace youtube_dl.WPF.Core
{
    public class YouTubeDLInstanceHandler : IDisposable
    {
        //private readonly DownloadCommandOptions _downloadOptions;
        private readonly Uri _youtubeDlExeFilePath;
        private readonly IObservable<Unit> _pollTicker = Observable.Interval(TimeSpan.FromMilliseconds(500)).Select(x => Unit.Default);

        public YouTubeDLInstanceHandler(
            Uri youtubeDlExeFilePath,
            IYouTubeDLCommand command)
        {
            this._youtubeDlExeFilePath = youtubeDlExeFilePath ?? throw new ArgumentNullException(nameof(youtubeDlExeFilePath));
            this.Command = command ?? throw new ArgumentNullException(nameof(command));

            this._outputsSourceList = new SourceList<string>().DisposeWith(this._disposables);
            this._errorsSourceList = new SourceList<string>().DisposeWith(this._disposables);

            this._whenStatusChanged_behaviorSubject = new BehaviorSubject<YouTubeDLInstanceStatus>(YouTubeDLInstanceStatus.Ready).DisposeWith(this._disposables);
            this.WhenStatusChanged = this._whenStatusChanged_behaviorSubject.DistinctUntilChanged();
        }

        public IYouTubeDLCommand Command { get; }

        // TODO: ensure, when process ends, all disposables linked to process life time get disposed and the object becomes "frozen"
        public Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var psi = new ProcessStartInfo()
            {
                Arguments = this.Command.Serialize(),
                FileName = this._youtubeDlExeFilePath.LocalPath,
                CreateNoWindow = false
            };

            this.Status = YouTubeDLInstanceStatus.Executing;

            return ProcessUtils.RunAsync(
               psi,
               output => this._outputsSourceList.Edit(list => list.Add(output)),
               error => this._errorsSourceList.Edit(list => list.Add(error)),
               (exitCode) =>
               {
                   this.Status = (exitCode == 0
                       ? YouTubeDLInstanceStatus.Completed
                       : YouTubeDLInstanceStatus.Failed);
               },
               cancellationToken);
        }
        public Task ExecuteAsync() { return this.ExecuteAsync(CancellationToken.None); }

        //private CancellationTokenSource _cancellationTokenSource;
        //public void Terminate()
        //{
        //    if (this._cancellationTokenSource.Token.CanBeCanceled)
        //    {
        //        this._cancellationTokenSource.Cancel();
        //    }
        //}

        public YouTubeDLInstanceStatus Status
        {
            get { return this._whenStatusChanged_behaviorSubject.Value; }
            private set
            {
                // TODO: validate status sequence. e.g. cannot set ready after executing
                this._whenStatusChanged_behaviorSubject.OnNext(value);

                if (this.Status == YouTubeDLInstanceStatus.Killed
                    || this.Status == YouTubeDLInstanceStatus.Failed
                    || this.Status == YouTubeDLInstanceStatus.Completed)
                {
                    this._whenStatusChanged_behaviorSubject.OnCompleted();
                }
            }
        }
        private readonly BehaviorSubject<YouTubeDLInstanceStatus> _whenStatusChanged_behaviorSubject;
        public IObservable<YouTubeDLInstanceStatus> WhenStatusChanged { get; }

        private readonly ISourceList<string> _outputsSourceList;
        public IObservableList<string> Outputs => this._outputsSourceList.AsObservableList();
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