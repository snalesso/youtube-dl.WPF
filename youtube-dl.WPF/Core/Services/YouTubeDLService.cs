using ReactiveUI;
using RunProcessAsTask;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using youtube_dl.WPF.Core.Models;

namespace youtube_dl.WPF.Core.Services
{
    public sealed class YouTubeDLService : IYouTubeDLService
    {
        public const string DirectDownloadUrl = "https://yt-dl.org/latest/youtube-dl.exe";
        public const string OfficialWebsite = "https://rg3.github.io/youtube-dl/";
        public const string RepositoryWebSite = "https://github.com/rg3/youtube-dl";

        private readonly IFileSystemService _fileSystemService;

        // TODO: settings class
        public YouTubeDLService(
            IFileSystemService fileSystemService,
            string youtubeDLExeFilePath = "youtube-dl\\youtube-dl.exe")
        {
            this._fileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));

            this.ExeFilePath = Path.GetFullPath(youtubeDLExeFilePath);
        }


        public string ExeFilePath { get; }
        public string DownloadsFolderPath => Path.Combine(Path.GetDirectoryName(this.ExeFilePath), "downloads");

        public bool IsBusy
        {
            get { return this._whenIsBusyChanged_Subject.Value; }
            private set { this._whenIsBusyChanged_Subject.OnNext(value); }
        }
        private readonly BehaviorSubject<bool> _whenIsBusyChanged_Subject = new BehaviorSubject<bool>(false);
        public IObservable<bool> WhenIsBusyChanged => this._whenIsBusyChanged_Subject.DistinctUntilChanged();

        public IReadOnlyReactiveList<DownloadQueueEntry> DownloadingBatchQueue { get; } = new ReactiveList<DownloadQueueEntry>();

        public async Task<DownloadHistoryEntry> DownloadAsync(DownloadQueueEntry entry)
        {
            var configParams = this.BuildDownloadParamsString(entry);
            var x = await this.ExecuteYouTubeDLAsync($"{configParams} {entry.Url}");

            return null;
        }

        public async Task<bool> UpdateAsync()
        {
            try
            {
                return (await this.ExecuteYouTubeDLAsync("-u")).ExitCode == 0;
            }
            catch (FileNotFoundException)
            {
                return await this.DownloadAsync();
            }
            catch (Win32Exception)
            {
                return await this.DownloadAsync();
            }
            catch (Exception)
            {
                // TODO: handle & log
                return false;
                //return Task.FromResult(false);
            }
        }

        private async Task<ProcessResults> ExecuteYouTubeDLAsync(string arguments)
        {
            this.IsBusy = true;

            var res = await ProcessEx.RunAsync(this.ExeFilePath, arguments);

            this.IsBusy = false;

            return res;
        }

        private async Task<bool> DownloadAsync()
        {
            this.IsBusy = true;

            var client = new WebClient();
            try
            {
                var youTubeDLExeDirPath = Path.GetDirectoryName(Path.GetFullPath(this.ExeFilePath));
                if (!Directory.Exists(youTubeDLExeDirPath))
                {
                    Directory.CreateDirectory(youTubeDLExeDirPath);
                }
                //await client.DownloadFileTaskAsync(YouTubeDLService.DirectDownloadLink, this.ExeFilePath);
                //return true;
                return await this._fileSystemService.DownloadFileAsync(YouTubeDLService.DirectDownloadUrl, this.ExeFilePath);
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

        // TODO: move to settings class with .BuildParamsString()
        private string BuildDownloadParamsString(DownloadQueueEntry entry)
        {
            string configFileVariant;
            switch (entry.DownloadMode)
            {
                case DownloadMode.AudioOnly:
                    configFileVariant = "audio";
                    break;
                case DownloadMode.AudioVideo:
                    configFileVariant = "video";
                    break;
                //case DownloadMode.VideoOnly:
                //    configFileName = "config-video.txt";
                //    break;
                default:
                    throw new NotSupportedException("Format not supported");
            }
            var configPath = Path.Combine(Path.GetDirectoryName(this.ExeFilePath), $"config-{configFileVariant}.txt");
            var configParams = $"--config-location \"{configPath}\" {entry.Url}";

            return configParams;
        }
    }
}
