using DynamicData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using youtube_dl.WPF.Core.Models;
using youtube_dl.WPF.Process;

namespace youtube_dl.WPF.Core
{
    public class YouTubeDL : IDisposable
    {
        #region constants

        public const string OfficialName = "youtube-dl";
        public const string OfficialWebsite = "https://rg3.github.io/youtube-dl/";
        public const string DirectDownloadUrl = "https://yt-dl.org/latest/youtube-dl.exe";
        public const string RepositoryWebSite = "https://github.com/rg3/youtube-dl";
        public const string DocumentationWebPage = "https://github.com/rg3/youtube-dl/blob/master/README.md#readme";

        public const ushort MaxSimultaneousDownloadsCount = 1;

        #endregion

        private readonly IFileSystemService _fileSystemService;

        // TODO: settings class
        public YouTubeDL(
            Uri exeFileLocation,
            IFileSystemService fileSystemService)
        {
            this.ExeFileLocation = exeFileLocation ?? throw new ArgumentNullException(nameof(exeFileLocation));
            this._fileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));

            this._executingInstances_SourceList = new SourceList<YouTubeDLInstanceObserver>().DisposeWith(this._disposables);
            this.ExecutingInstances = this._executingInstances_SourceList.AsObservableList();
            this._whenIsBusyChanged_BehaviorSubject = new BehaviorSubject<bool>(false).DisposeWith(this._disposables);
        }

        public Uri ExeFileLocation { get; } // = new Uri( "youtube-dl\\youtube-dl.exe");
        public Uri DownloadsFolderLocation { get; } = new Uri($"E:\\Downloads\\{YouTubeDL.OfficialName}"); // Path.Combine(Path.GetDirectoryName(this.ExeFilePath), "downloads");

        // TODO: use semaphore
        private readonly SemaphoreSlim _isBusySemaphore = new SemaphoreSlim(1, 1);
        public bool IsBusy
        {
            get
            {
                //return this._isBusySemaphore.CurrentCount > 0;
                return this._whenIsBusyChanged_BehaviorSubject.Value;
            }
            private set { this._whenIsBusyChanged_BehaviorSubject.OnNext(value); }
        }
        private readonly BehaviorSubject<bool> _whenIsBusyChanged_BehaviorSubject;
        public IObservable<bool> WhenIsBusyChanged => this._whenIsBusyChanged_BehaviorSubject.DistinctUntilChanged();

        private readonly ISourceList<YouTubeDLInstanceObserver> _executingInstances_SourceList;
        public IObservableList<YouTubeDLInstanceObserver> ExecutingInstances { get; }

        private Task ExecuteCommandAsync(DownloadCommand command)
        {
            // TODO: verify what happens when concurrently downloading same URL + same options multiple times
            var obs = new YouTubeDLInstanceObserver(this.ExeFileLocation, command);

            this._executingInstances_SourceList.Edit(list =>
            {
                list.Add(obs);
            });

            obs.WhenStatusChanged.Where(status =>
                status == YouTubeDLInstanceObserverStatus.Completed
                || status == YouTubeDLInstanceObserverStatus.Canceled
                || status == YouTubeDLInstanceObserverStatus.Failed)
                .Subscribe(x =>
                {

                })
                .DisposeWith(this._disposables); // TODO: handle the best way, prevent closing, unsub when download canceled

            obs.ExecuteAsync().ConfigureAwait(false);

            return Task.CompletedTask;
        }

        private async Task<bool> DownloadYouTubeDLClientAsync()
        {
            this.IsBusy = true;

            var client = new WebClient();
            try
            {
                var youTubeDLExeDirPath = Path.GetDirectoryName(this.ExeFileLocation.LocalPath);
                if (!Directory.Exists(youTubeDLExeDirPath))
                {
                    Directory.CreateDirectory(youTubeDLExeDirPath);
                }
                //await client.DownloadFileTaskAsync(YouTubeDLService.DirectDownloadLink, this.ExeFilePath);
                //return true;
                return await this._fileSystemService.DownloadFileAsync(YouTubeDL.DirectDownloadUrl, this.ExeFileLocation.LocalPath);
            }
            catch (Exception)
            {
                // TODO: handle & log
                return false;
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private async Task<ProcessResults> ExecuteYouTubeDLAsync(string arguments)
        {
            this.IsBusy = true;

            var psi = new ProcessStartInfo()
            {
                Arguments = arguments,
                FileName = this.ExeFileLocation.LocalPath,
                CreateNoWindow = true
            };
            var res = await ProcessUtils.RunAsync(psi);

            this.IsBusy = false;

            return res;
        }

        public async Task<bool> UpdateAsync()
        {
            try
            {
                await this._isBusySemaphore.WaitAsync();

                this.IsBusy = true;

                var updateResult = (await this.ExecuteYouTubeDLAsync("-U")).ExitCode == 0;

                this.IsBusy = false;

                return updateResult;
            }
            catch (FileNotFoundException)
            {
                return await this.DownloadYouTubeDLClientAsync();
            }
            catch (Win32Exception)
            {
                return await this.DownloadYouTubeDLClientAsync();
            }
            catch (Exception)
            {
                // TODO: handle & log
                return false;
                //return Task.FromResult(false);
            }
        }

        #region IDisposable

        // https://docs.microsoft.com/en-us/dotnet/api/system.idisposable?view=netframework-4.8
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isDisposed = false;

        // use this in derived class
        // protected override void Dispose(bool isDisposing)
        // use this in non-derived class
        private void Dispose(bool isDisposing)
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
