using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Caliburn.Micro.ReactiveUI;
using ReactiveUI;
using youtube_dl.WPF.Core;

namespace youtube_dl.WPF.Presentation.ViewModels
{
    public class UtilsViewModel : ReactiveScreen
    {
        private readonly YouTubeDL _youTubeDL;
        private readonly IFileSystemService _fileSystemService;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public UtilsViewModel(
            YouTubeDL youTubeDL,
            IFileSystemService fileSystemService)
        {
            this._youTubeDL = youTubeDL ?? throw new ArgumentNullException(nameof(youTubeDL));
            this._fileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));

            this.UpdateDependencies = ReactiveCommand.CreateFromTask(async () =>
            {
                return await this._youTubeDL.UpdateAsync();
            }, outputScheduler: RxApp.MainThreadScheduler);
            this.UpdateDependencies.ThrownExceptions.Subscribe(ex => Console.WriteLine(ex.ToString())).DisposeWith(this._disposables);
            this.UpdateDependencies.DisposeWith(this._disposables);

            this.OpenDownloadsFolder = ReactiveCommand.Create(() => this._fileSystemService.OpenFolder(this._youTubeDL.DownloadsFolderLocation.LocalPath));
            this.OpenDownloadsFolder.ThrownExceptions.Subscribe(ex => Console.WriteLine($"++ThrownExceptions: {ex.ToString()}")).DisposeWith(this._disposables);
            this.OpenDownloadsFolder.Where(couldOpenFolder => !couldOpenFolder).Subscribe(couldOpenFolder => Console.WriteLine("Downloads folder not found"));
            this.OpenDownloadsFolder.DisposeWith(this._disposables);

            this.NavigateToYoutubeDLDocumentationWebPage = ReactiveCommand.Create(() => this._fileSystemService.NavigateUrl(YouTubeDL.DocumentationWebPage));
            this.NavigateToYoutubeDLDocumentationWebPage.ThrownExceptions.Subscribe(ex => Console.WriteLine(ex.ToString())).DisposeWith(this._disposables);
            this.NavigateToYoutubeDLDocumentationWebPage.DisposeWith(this._disposables);
        }

        public ReactiveCommand<Unit, bool> UpdateDependencies { get; }
        public ReactiveCommand<Unit, bool> OpenDownloadsFolder { get; }
        public ReactiveCommand<Unit, Unit> NavigateToYoutubeDLDocumentationWebPage { get; }
    }
}
