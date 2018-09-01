using Caliburn.Micro;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using youtube_dl.WPF.Core.Services;

namespace youtube_dl.WPF.Presentation.ViewModels
{
    public class UtilsViewModel : Screen
    {
        private readonly IYouTubeDLService _youTubeDLService;
        private readonly IFileSystemService _fileSystemService;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public UtilsViewModel(
            IYouTubeDLService youTubeDLService,
            IFileSystemService fileSystemService)
        {
            this._youTubeDLService = youTubeDLService ?? throw new ArgumentNullException(nameof(youTubeDLService));
            this._fileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));

            this.Update = ReactiveCommand.CreateFromTask(this._youTubeDLService.UpdateAsync, outputScheduler: RxApp.MainThreadScheduler);
            this.Update.ThrownExceptions.Subscribe(ex => Console.WriteLine(ex.ToString())).DisposeWith(this._disposables);

            this.CheckFFmpeg = ReactiveCommand.CreateFromTask(
                async () =>
                {
                    var ffmpegFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), Path.GetFileName(FFmpegConstants.DirectDownloadLike_64));
                    return await this._fileSystemService.DownloadFileAsync(
                        FFmpegConstants.DirectDownloadLike_64,
                        ffmpegFilePath,
                        (bytesSize, bytesDownloaded, progressPercent) => { Console.WriteLine($"{string.Format("{0:00}", progressPercent)}%\t({bytesDownloaded} of {bytesSize})"); });
                });
            this.CheckFFmpeg.ThrownExceptions.Subscribe(ex => Console.WriteLine(ex.ToString())).DisposeWith(this._disposables);

            this.OpenDownloadsFolder = ReactiveCommand.Create(() => this._fileSystemService.OpenFolder(this._youTubeDLService.DownloadsFolderPath));
            this.OpenDownloadsFolder.ThrownExceptions.Subscribe(ex => Console.WriteLine($"++ThrownExceptions: {ex.ToString()}")).DisposeWith(this._disposables);
            this.OpenDownloadsFolder.Where(couldOpenFolder => !couldOpenFolder).Subscribe(couldOpenFolder => Console.WriteLine("Downloads folder not found"));

            this.NavigateToYoutubeDLRepositoryWebSite = ReactiveCommand.Create(() => this._fileSystemService.NavigateUrl(YouTubeDLService.RepositoryWebSite));
            this.NavigateToYoutubeDLRepositoryWebSite.ThrownExceptions.Subscribe(ex => Console.WriteLine(ex.ToString())).DisposeWith(this._disposables);
        }

        public ReactiveCommand<Unit, bool> Update { get; }
        public ReactiveCommand<Unit, bool> OpenDownloadsFolder { get; }
        public ReactiveCommand<Unit, Unit> NavigateToYoutubeDLRepositoryWebSite { get; }

        public ReactiveCommand<Unit, bool> CheckFFmpeg { get; }
    }
}
