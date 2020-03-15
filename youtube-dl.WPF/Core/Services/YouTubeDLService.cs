using DynamicData;
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
        public const string DocumentationWebPage = "https://github.com/rg3/youtube-dl/blob/master/README.md#readme";

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
        public string DownloadsFolderPath => "E:\\Downloads\\youtube-dl"; // Path.Combine(Path.GetDirectoryName(this.ExeFilePath), "downloads");

        public bool IsBusy
        {
            get { return this._whenIsBusyChanged_Subject.Value; }
            private set { this._whenIsBusyChanged_Subject.OnNext(value); }
        }
        private readonly BehaviorSubject<bool> _whenIsBusyChanged_Subject = new BehaviorSubject<bool>(false);
        public IObservable<bool> WhenIsBusyChanged => this._whenIsBusyChanged_Subject.DistinctUntilChanged();

        public IObservableList<DownloadQueueEntry> DownloadingBatchQueue { get; } = new SourceList<DownloadQueueEntry>();

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
                return (await this.ExecuteYouTubeDLAsync("-U")).ExitCode == 0;
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

        private async Task<ProcessResults> ExecuteYouTubeDLAsync(string arguments)
        {
            this.IsBusy = true;

            var psi = new System.Diagnostics.ProcessStartInfo()
            {
                Arguments = arguments,
                FileName = this.ExeFilePath,
                CreateNoWindow = true
            };
            var res = await ProcessEx.RunAsync(psi);

            this.IsBusy = false;

            return res;
        }

        private async Task<bool> DownloadYouTubeDLClientAsync()
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
            var paramsList = new List<string>();
            var dlOptions = new YouTubeDLDownloadOptions();

            dlOptions.PreferFFmpeg = true;
            dlOptions.FFmpegLocation = ".\\ffmpeg-latest-win64-static\\bin\\ffmpeg.exe"; // TODO: add x86 / x64 support

            switch (entry.Options.DownloadMode)
            {
                case DownloadMode.AudioOnly:
                    dlOptions.OutputFolderPath = $"E:\\Downloads\\youtube-dl\\audio\\%(title)s -- %(uploader)s -- %(id)s.%(ext)s";
                    dlOptions.ExtractAudio = true;
                    dlOptions.AudioFormat = AudioFormat.MP3;
                    break;

                case DownloadMode.AudioVideo:
                    dlOptions.OutputFolderPath = $"E:\\Downloads\\youtube-dl\\video\\%(title)s -- %(uploader)s -- %(id)s.%(ext)s";
                    dlOptions.VideoFormat = $"bestvideo[height<=?{(int)(VideoResolutionHeight._1080p)}]+bestaudio/best";
                    break;

                default:
                    throw new NotSupportedException("Format not supported");
            }

            var paramsString = dlOptions.BuildParamsString();

            return paramsString;
        }
    }
}
