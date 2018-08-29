using Caliburn.Micro;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using youtube_dl.WPF.Core.Models;
using youtube_dl.WPF.Core.Services;

namespace youtube_dl.WPF.Presentation.ViewModels
{
    public class DownloadViewModel : Screen
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
                this._downloadQueueService.QueueEntries.CountChanged.StartWith(this._downloadQueueService.QueueEntries.Count()).Select(count => count > 0))
                .DisposeWith(this._disposables);
            this.StartDownload.ThrownExceptions.Subscribe(ex => Console.WriteLine(ex.ToString())).DisposeWith(this._disposables);
        }

        public ReactiveCommand<Unit, Unit> StartDownload { get; }
    }
}