using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Caliburn.Micro.ReactiveUI;
using ReactiveUI;
using youtube_dl.WPF.Core;
using youtube_dl.WPF.Core.Queue;
using youtube_dl.WPF.Core.Services;

namespace youtube_dl.WPF.Presentation.ViewModels
{
    public class DownloadViewModel : ReactiveScreen
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        private readonly DownloadCommandsQueue _downloadCommandsQueue;
        private readonly YouTubeDL _youTubeDL;
        private readonly IFileSystemService _fileSystemService;

        public DownloadViewModel(
            DownloadCommandsQueue downloadCommandsQueue,
            YouTubeDL youTubeDL,
            IFileSystemService fileSystemService)
        {
            this._downloadCommandsQueue = downloadCommandsQueue ?? throw new ArgumentNullException(nameof(downloadCommandsQueue));
            this._youTubeDL = youTubeDL ?? throw new ArgumentNullException(nameof(youTubeDL));
            this._fileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));

            this._isAutoDownloadEnabled_OAPH = this._downloadCommandsQueue
                .WhenAnyValue(x => x.AreDownloadsAutomaticallyStarted)
                .ToProperty(this, nameof(this.IsAutoDownloadEnabled))
                .DisposeWith(this._disposables);

            this.StartDownload = ReactiveCommand.CreateFromTask(
                () => this._downloadCommandsQueue.StartDownloadsAsync(),
                this._downloadCommandsQueue.Entries.CountChanged.Select(count => count != 0));
            this.StartDownload.ThrownExceptions.Subscribe(ex => { Console.WriteLine(ex.ToString()); throw ex; }).DisposeWith(this._disposables);
            this.StartDownload.DisposeWith(this._disposables);

            this._isDownloading_OAPH = this.WhenAnyObservable(x => x.StartDownload.IsExecuting)
                .ToProperty(this, x => x.IsDownloading)
                .DisposeWith(this._disposables);

            this.OpenDownloadsFolder = ReactiveCommand.Create(() => this._fileSystemService.OpenFolder(this._youTubeDL.DownloadsFolderLocation.LocalPath));
            this.OpenDownloadsFolder.ThrownExceptions.Subscribe(ex => Console.WriteLine($"++ThrownExceptions: {ex}")).DisposeWith(this._disposables);
            this.OpenDownloadsFolder.Where(couldOpenFolder => !couldOpenFolder).Subscribe(couldOpenFolder => Console.WriteLine("Downloads folder not found"));
            this.OpenDownloadsFolder.DisposeWith(this._disposables);
        }

        private ObservableAsPropertyHelper<bool> _isDownloading_OAPH;
        public bool IsDownloading => this._isDownloading_OAPH.Value;

        private ObservableAsPropertyHelper<bool> _isAutoDownloadEnabled_OAPH;
        public bool IsAutoDownloadEnabled => this._isAutoDownloadEnabled_OAPH.Value;

        public ReactiveCommand<Unit, Unit> StartDownload { get; }
        public ReactiveCommand<Unit, bool> OpenDownloadsFolder { get; }
    }
}