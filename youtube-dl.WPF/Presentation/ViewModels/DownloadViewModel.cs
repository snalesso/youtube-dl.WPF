using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Caliburn.Micro.ReactiveUI;
using ReactiveUI;
using youtube_dl.WPF.Core.Services;

namespace youtube_dl.WPF.Presentation.ViewModels
{
    public class DownloadViewModel : ReactiveScreen
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        private readonly IDownloadQueueService _downloadQueueService;
        private readonly IYouTubeDLService _youTubeDLService;

        public DownloadViewModel(
            IDownloadQueueService downloadQueueService,
            IYouTubeDLService youTubeDLService)
        {
            this._downloadQueueService = downloadQueueService ?? throw new ArgumentNullException(nameof(downloadQueueService));
            this._youTubeDLService = youTubeDLService ?? throw new ArgumentNullException(nameof(youTubeDLService));

            this.StartDownload = ReactiveCommand.CreateFromTask(
                async () =>
                {
                    await this._youTubeDLService.DownloadAsync(this._downloadQueueService.Dequeue());
                },
                this._downloadQueueService.QueueEntries.CountChanged.Select(count => count != 0));
            this.StartDownload.ThrownExceptions.Subscribe(ex => Console.WriteLine(ex.ToString())).DisposeWith(this._disposables);
            this.StartDownload.DisposeWith(this._disposables);

            this._isDownloading_OAPH = this.WhenAnyObservable(x => x.StartDownload.IsExecuting)
                .ToProperty(this, x => x.IsDownloading)
                .DisposeWith(this._disposables);
        }

        private ObservableAsPropertyHelper<bool> _isDownloading_OAPH;
        public bool IsDownloading => this._isDownloading_OAPH.Value;

        public ReactiveCommand<Unit, Unit> StartDownload { get; }
    }
}