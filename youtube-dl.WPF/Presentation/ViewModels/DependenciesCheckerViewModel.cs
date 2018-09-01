using Caliburn.Micro;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using youtube_dl.WPF.Core.Services;

namespace youtube_dl.WPF.Presentation.ViewModels
{
    public class DependenciesCheckerViewModel : Screen
    {
        private readonly IYouTubeDLService _youTubeDLService;
        private readonly IFileSystemService _fileSystemService;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public DependenciesCheckerViewModel(
            IYouTubeDLService youTubeDLService,
            IFileSystemService fileSystemService)
        {
            this._youTubeDLService = youTubeDLService ?? throw new ArgumentNullException(nameof(youTubeDLService));
            this._fileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));

            //this.CheckYouTubeDLPresence = ReactiveCommand.Create(File.Exists(this._youTubeDLService.);
            //this.CheckYouTubeDLPresence.ThrownExceptions.Subscribe(ex => Console.WriteLine(ex.ToString())).DisposeWith(this._disposables);

            //this.DownloadYouTubeDL = ReactiveCommand.Create(() => this._fileSystemService.OpenFolder(this._youTubeDLService.DownloadsFolderPath));
            //this.DownloadYouTubeDL.ThrownExceptions.Subscribe(ex => Console.WriteLine($"++ThrownExceptions: {ex.ToString()}")).DisposeWith(this._disposables);
            //this.DownloadYouTubeDL.Where(couldOpenFolder => !couldOpenFolder).Subscribe(couldOpenFolder => Console.WriteLine("Downloads folder not found"));

            //this.CheckFFmpegPresence = ReactiveCommand.CreateFromTask(File.Exists();
            //this.CheckFFmpegPresence.ThrownExceptions.Subscribe(ex => Console.WriteLine(ex.ToString())).DisposeWith(this._disposables);
        }

        private string _status;
        public string Status
        {
            get { return this._status; }
            private set { this.Set(ref this._status, value); }
        }

        private double _progressionPercent;
        public double ProgressionPercent
        {
            get { return this._progressionPercent; }
            private set { this.Set(ref this._progressionPercent, value); }
        }

        public ReactiveCommand<Unit, bool> CheckYouTubeDLPresence { get; }
        public ReactiveCommand<Unit, bool> DownloadYouTubeDL { get; }
        public ReactiveCommand<Unit, bool> CheckFFmpegPresence { get; }
        public ReactiveCommand<Unit, bool> DownloadFFmpeg { get; }
    }
}