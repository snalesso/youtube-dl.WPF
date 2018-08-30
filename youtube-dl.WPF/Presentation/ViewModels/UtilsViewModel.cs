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

            this.OpenDownloadsFolder = ReactiveCommand.Create(() => this._fileSystemService.OpenFolder(this._youTubeDLService.DownloadsFolderPath));
            this.OpenDownloadsFolder.ThrownExceptions.Subscribe(ex => Console.WriteLine($"ThrownExceptions: {ex.ToString()}")).DisposeWith(this._disposables);
            this.OpenDownloadsFolder.Where(couldOpenFolder => !couldOpenFolder).Subscribe(couldOpenFolder => Console.WriteLine("Could not open downloads folder"));

            this.NavigateToYoutubeDLRepositoryWebSite = ReactiveCommand.Create(() => this._fileSystemService.NavigateLink(this._youTubeDLService.RepositoryWebSite));
            this.NavigateToYoutubeDLRepositoryWebSite.ThrownExceptions.Subscribe(ex => Console.WriteLine(ex.ToString())).DisposeWith(this._disposables);
        }

        public ReactiveCommand<Unit, bool> Update { get; }
        public ReactiveCommand<Unit, bool> OpenDownloadsFolder { get; }
        public ReactiveCommand<Unit, Unit> NavigateToYoutubeDLRepositoryWebSite { get; }
    }
}
