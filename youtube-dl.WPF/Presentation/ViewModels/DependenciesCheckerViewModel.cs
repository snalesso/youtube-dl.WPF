using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using youtube_dl.WPF.Core;
using youtube_dl.WPF.Core.Services;

namespace youtube_dl.WPF.Presentation.ViewModels
{
    public class DependenciesCheckerViewModel : ReactiveScreen
    {
        private readonly IWindowManager _windowManager;
        private readonly YouTubeDL _youTubeDL;
        private readonly IFileSystemService _fileSystemService;
        private readonly Func<ShellViewModel> _shellViewModelFactory;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public DependenciesCheckerViewModel(
            IWindowManager windowManager,
            YouTubeDL youTubeDL,
            IFileSystemService fileSystemService,
            Func<ShellViewModel> shellViewModelFactory)
        {
            this._windowManager = windowManager ?? throw new ArgumentNullException(nameof(windowManager));
            this._youTubeDL = youTubeDL ?? throw new ArgumentNullException(nameof(youTubeDL));
            this._fileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));
            this._shellViewModelFactory = shellViewModelFactory ?? throw new ArgumentNullException(nameof(shellViewModelFactory));

            this.CheckForYouTubeDLPresence = ReactiveCommand.Create(() =>
            {
                this.Status = $"Checking for {YouTubeDL.OfficialName} ...";
                var isYTDLPresent = File.Exists(this._youTubeDL.ExeFileLocation.LocalPath);
                this.Status = isYTDLPresent ? $"{YouTubeDL.OfficialName} found" : $"{YouTubeDL.OfficialName} not found";

                return isYTDLPresent;
            });
            this.CheckForYouTubeDLPresence.ThrownExceptions.Subscribe(ex => Console.WriteLine(ex.ToString())).DisposeWith(this._disposables);
            this.CheckForYouTubeDLPresence.DisposeWith(this._disposables);

            this.DownloadYouTubeDL = ReactiveCommand.CreateFromTask(async () =>
            {
                //var youTubeDLExeDirPath = Path.GetDirectoryName(Path.GetFullPath(this._youTubeDLService.ExeFilePath));
                //if (!Directory.Exists(youTubeDLExeDirPath))
                //{
                //    Directory.CreateDirectory(youTubeDLExeDirPath);
                //}

                this.Status = $"Downloading {YouTubeDL.OfficialName} ...";
                var downloaded = await this._fileSystemService.DownloadFileAsync(YouTubeDL.DirectDownloadUrl, this._youTubeDL.ExeFileLocation.LocalPath, true);

                this.Status = downloaded ? $"{YouTubeDL.OfficialName} downloaded" : $"{YouTubeDL.OfficialName} download failed";

                return downloaded;
            });
            this.DownloadYouTubeDL.ThrownExceptions.Subscribe(ex => Console.WriteLine(ex.ToString())).DisposeWith(this._disposables);
            this.CheckForYouTubeDLPresence.DisposeWith(this._disposables);

            this.TryUpdateYouTubeDL = ReactiveCommand.CreateFromTask(async () =>
            {
                this.Status = $"Updating {YouTubeDL.OfficialName} ...";
                var didUpdate = await this._youTubeDL.UpdateAsync();
                //await Task.Delay(3000);
                this.Status = didUpdate ? $"{YouTubeDL.OfficialName} updated!" : $"{YouTubeDL.OfficialName} is already on the latest vesion";

                return didUpdate;
            });
            this.TryUpdateYouTubeDL.ThrownExceptions.Subscribe(ex => Console.WriteLine(ex.ToString())).DisposeWith(this._disposables);
            this.TryUpdateYouTubeDL.DisposeWith(this._disposables);

            this.CheckForFFmpegPresence = ReactiveCommand.Create(() =>
            {
                this.Status = "Checking for FFmpeg ...";
                var ffmpegParentDirectoryPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                var ffmpegRootDirectoryPath = Path.Combine(ffmpegParentDirectoryPath, Path.GetFileNameWithoutExtension(FFmpeg.DirectDownloadLink_64));
                //var ffmpegFilePath = Path.Combine(Path.GetDirectoryName(), Path.GetFileName(FFmpegConstants.DirectDownloadLike_64));

                var isFFmpegPresent = Directory.Exists(ffmpegRootDirectoryPath);

                this.Status = isFFmpegPresent ? "FFmpeg found" : "FFmpeg not found";

                return isFFmpegPresent;
            });
            this.CheckForFFmpegPresence.ThrownExceptions.Subscribe(ex => Console.WriteLine(ex.ToString())).DisposeWith(this._disposables);
            this.CheckForFFmpegPresence.DisposeWith(this._disposables);

            this.DownloadFFmpeg = ReactiveCommand.CreateFromTask(async () =>
            {
                this.Status = "Downloading FFmpeg ...";
                var ffmpegParentDirectoryPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                var ffmpegArchiveFilePath = Path.Combine(ffmpegParentDirectoryPath, Path.GetFileName(FFmpeg.DirectDownloadLink_64));

                var downloaded = await this._fileSystemService.DownloadFileAsync(FFmpeg.DirectDownloadLink_64, ffmpegArchiveFilePath);
                //, (bytesSize, bytesDownloaded, progressPercent) => { Console.WriteLine($"{string.Format("{0:00}", progressPercent)}%\t({bytesDownloaded} of {bytesSize})"); }
                this.Status = "FFmpeg downloaded";

                if (!downloaded)
                {
                    this.Status = "FFmpeg download failed";
                    return false;
                }

                this.Status = "Extracting FFmpeg ...";
                var extracted = await Task.Run(() => this._fileSystemService.UncompressArchive(ffmpegArchiveFilePath, ffmpegParentDirectoryPath)).ConfigureAwait(false); // TODO: studiare .ConfigureAwait
                this.Status = extracted ? "FFmpeg extracted" : "FFmpeg extraction failed";

                return extracted;
            });
            this.DownloadFFmpeg.ThrownExceptions.Subscribe(ex => Console.WriteLine(ex.ToString())).DisposeWith(this._disposables);
            this.DownloadFFmpeg.DisposeWith(this._disposables);

            this._isBusy_OAPH = this.WhenAnyObservable(
                x => x.CheckForYouTubeDLPresence.IsExecuting,
                x => x.DownloadYouTubeDL.IsExecuting,
                x => x.TryUpdateYouTubeDL.IsExecuting,
                x => x.CheckForFFmpegPresence.IsExecuting,
                x => x.DownloadFFmpeg.IsExecuting)
                .ToProperty(this, x => x.IsBusy)
                .DisposeWith(this._disposables);

            //this._isBusy_OAPH =
            //    Observable.CombineLatest(
            //    this.WhenAnyObservable(@this => @this.EnsureYouTubeDLPresence.IsExecuting),
            //    this.WhenAnyObservable(@this => @this.CheckForYouTubeDLUpdates.IsExecuting),
            //    this.WhenAnyObservable(@this => @this.EnsureFFmpegPresence.IsExecuting),
            //    (isEnsuringYouTubeDLPresence, isCheckingForYouTubeDLUpdates, isEnsuringFFmpegPresence) =>
            //    isEnsuringYouTubeDLPresence
            //    || isCheckingForYouTubeDLUpdates
            //    || isEnsuringFFmpegPresence)
            //    //.StartWith(false)
            //    .ToProperty(this, @this => @this.IsBusy)
            //    .DisposeWith(this._disposables);

            this.DisplayName = Assembly.GetEntryAssembly().GetAssemblyName();
        }

        private string _status;
        public string Status
        {
            get { return this._status; }
            private set { this.RaiseAndSetIfChanged(ref this._status, value); }
        }

        private double _progressionPercent;
        public double ProgressionPercent
        {
            get { return this._progressionPercent; }
            private set { this.RaiseAndSetIfChanged(ref this._progressionPercent, value); }
        }

        private ObservableAsPropertyHelper<bool> _isBusy_OAPH;
        public bool IsBusy => this._isBusy_OAPH.Value;

        public ReactiveCommand<Unit, bool> CheckForYouTubeDLPresence { get; }
        public ReactiveCommand<Unit, bool> DownloadYouTubeDL { get; }
        public ReactiveCommand<Unit, bool> TryUpdateYouTubeDL { get; }
        public ReactiveCommand<Unit, bool> CheckForFFmpegPresence { get; }
        public ReactiveCommand<Unit, bool> DownloadFFmpeg { get; }

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            if (!Debugger.IsAttached)
            {
                var isYTDLPresent = await this.CheckForYouTubeDLPresence.Execute();
                if (isYTDLPresent)
                    await this.TryUpdateYouTubeDL.Execute();
                else
                    isYTDLPresent = await this.DownloadYouTubeDL.Execute();

                var isFFmpegPresent = await this.CheckForFFmpegPresence.Execute();
                if (!isFFmpegPresent)
                    isFFmpegPresent = await this.DownloadFFmpeg.Execute();

                if (!isYTDLPresent || !isFFmpegPresent)
                {
                    throw new Exception("WHAT?");
                }
            }

            this.SwitchToClient();
        }

        private void SwitchToClient()
        {
            this._windowManager.ShowWindow(this._shellViewModelFactory.Invoke());
            this.TryClose();
        }

        //public override void TryClose(bool? dialogResult = null)
        //{
        //    // TODO: add this._fileSystemService.IsBusy
        //    // TODO: add interruption dialogs
        //    var canClose = !(this._youTubeDLService.IsBusy /*&& this._fileSystemService)*/ );
        //    base.TryClose(canClose);
        //}
    }
}