using DynamicData;
using Nito.AsyncEx;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.PeerToPeer.Collaboration;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

using youtube_dl.WPF.Process;

namespace youtube_dl.WPF.Core
{
    public partial class YouTubeDL : IDisposable
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
        private readonly AsyncReaderWriterLock _exeRWLock = new AsyncReaderWriterLock();

        // TODO: settings class
        public YouTubeDL(
            Uri exeFileLocation,
            IFileSystemService fileSystemService)
        {
            this.ExeFileLocation = exeFileLocation ?? throw new ArgumentNullException(nameof(exeFileLocation));
            this._fileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));

            this._executingInstances_SourceList = new SourceList<YouTubeDLInstanceHandler>().DisposeWith(this._disposables);
            this.ExecutingInstances = this._executingInstances_SourceList.AsObservableList();
            //this._whenIsBusyChanged_BehaviorSubject = new BehaviorSubject<bool>(false).DisposeWith(this._disposables);
        }

        public Uri ExeFileLocation { get; } // = new Uri( "youtube-dl\\youtube-dl.exe");
        [Obsolete("Each command has its own -o location")]
        public Uri DownloadsFolderLocation { get; } = new Uri(Path.Combine("E" + Path.VolumeSeparatorChar + Path.DirectorySeparatorChar, "Downloads", YouTubeDL.OfficialName)); // Path.Combine(Path.GetDirectoryName(this.ExeFilePath), "downloads");

        //public bool IsBusy
        //{
        //    get
        //    {
        //        //return this._isBusySemaphore.CurrentCount > 0;
        //        return this._whenIsBusyChanged_BehaviorSubject.Value;
        //    }
        //    private set { this._whenIsBusyChanged_BehaviorSubject.OnNext(value); }
        //}
        //private readonly BehaviorSubject<bool> _whenIsBusyChanged_BehaviorSubject;
        //public IObservable<bool> WhenIsBusyChanged => this._whenIsBusyChanged_BehaviorSubject.DistinctUntilChanged();

        private readonly ISourceList<YouTubeDLInstanceHandler> _executingInstances_SourceList;
        public IObservableList<YouTubeDLInstanceHandler> ExecutingInstances { get; }

        private async Task<bool> DownloadBinariesAsync()
        {
            using (var writeAccess = await this._exeRWLock.WriterLockAsync())
            {
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
                    //this.IsBusy = false;
                }
            }
        }
        public async Task<bool> UpdateAsync()
        {
            try
            {
                using (var writeAccess = await this._exeRWLock.WriterLockAsync())
                {
                    var wasUpdateSuccessful = (await this.ExecuteAsync("-U")).ExitCode == 0;
                    return wasUpdateSuccessful;
                }
            }
            // if cannot find youtube-dl client, dowlonad it
            catch (FileNotFoundException)
            {
                return await this.DownloadBinariesAsync();
            }
            // cant remember
            catch (Win32Exception)
            {
                return await this.DownloadBinariesAsync();
            }
            catch (Exception)
            {
                // TODO: handle & log
                return false;
                //return Task.FromResult(false);            
            }
        }
        public async Task ExecuteAsync(IYouTubeDLCommand command)
        {
            // TODO: verify what happens when concurrently downloading same URL + same options multiple times
            var instanceHandler = new YouTubeDLInstanceHandler(this.ExeFileLocation, command);

            IDisposable exeLock = null;

            // identify lock type
            switch (command.Type)
            {
                case YouTubeDLCommandType.Download:
                    exeLock = await this._exeRWLock.ReaderLockAsync();
                    break;

                case YouTubeDLCommandType.Update:
                    exeLock = await this._exeRWLock.WriterLockAsync();
                    break;
            }

            // configure YT-DL instance process handler
            // TODO: create a disposable that destroys the instance when downloads are canceled forcedly
            instanceHandler
                .WhenProcessEnded()
                //.LastAsync(x =>
                .ObserveOn(RxApp.MainThreadScheduler)
                //.SubscribeOnDispatcher()
                .Subscribe(x =>
                {
                    exeLock.Dispose();
                    this._executingInstances_SourceList.Edit(list => list.Remove(instanceHandler));
                })
                .DisposeWith(this._disposables); // TODO: handle the best way, prevent closing, unsub when download canceled

            this._executingInstances_SourceList.Edit(
                //async 
                list =>
            {
                list.Add(instanceHandler);
                //await instanceHandler.ExecuteAsync();
            });

            await instanceHandler.ExecuteAsync().ConfigureAwait(false);
        }

        private async Task<ProcessResults> ExecuteAsync(string arguments)
        {
            var psi = new ProcessStartInfo()
            {
                Arguments = arguments,
                FileName = this.ExeFileLocation.LocalPath,
#if DEBUG
                CreateNoWindow = true
#else
                CreateNoWindow = false
#endif
            };
            var res = await ProcessUtils.RunAsync(psi);

            return res;
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