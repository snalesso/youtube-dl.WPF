﻿using ReactiveUI;
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
    public class YouTubeDLService : IYouTubeDLService
    {
        public static readonly string RepositoryLink = "https://github.com/rg3/youtube-dl/blob/master/README.md#readme";
        public static readonly string HomePageLink = "https://rg3.github.io/youtube-dl/";

        public YouTubeDLService(string youtubeDLExeFilePath = "youtube-dl\\youtube-dl.exe")
        {
            //if (!File.Exists(youtubeDLExePath))
            //    throw new FileNotFoundException(youtubeDLExePath);

            this.YouTubeDLExeFilePath = youtubeDLExeFilePath;
        }

        public string YouTubeDLExeFilePath { get; }

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
            var x = await this.ExecuteYouTubeDL($"{configParams} {entry.Url}");

            return null;
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
            var configPath = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(this.YouTubeDLExeFilePath)), $"config-{configFileVariant}.txt");
            var configParams = $"--config-location \"{configPath}\" {entry.Url}";

            return configParams;
        }

        public async Task<bool> UpdateAsync()
        {
            try
            {
                return (await this.ExecuteYouTubeDL("-u")).ExitCode == 0;
            }
            catch (FileNotFoundException)
            {
                return await this.TryDownload();
            }
            catch (Win32Exception)
            {
                return await this.TryDownload();
            }
            catch (Exception)
            {
                // TODO: handle & log
                return false;
                //return Task.FromResult(false);
            }
        }

        private async Task<ProcessResults> ExecuteYouTubeDL(string arguments)
        {
            this.IsBusy = true;

            var res = await ProcessEx.RunAsync(this.YouTubeDLExeFilePath, arguments);

            this.IsBusy = false;

            return res;
        }

        private async Task<bool> TryDownload()
        {
            this.IsBusy = true;

            var client = new WebClient();
            try
            {
                var youTubeDLExeDirPath = Path.GetDirectoryName(Path.GetFullPath(this.YouTubeDLExeFilePath));
                if (!Directory.Exists(youTubeDLExeDirPath))
                {
                    Directory.CreateDirectory(youTubeDLExeDirPath);
                }
                await client.DownloadFileTaskAsync("https://yt-dl.org/latest/youtube-dl.exe", this.YouTubeDLExeFilePath);
                return true;
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
    }
}