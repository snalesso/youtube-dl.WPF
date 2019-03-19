using System;

namespace youtube_dl.WPF.Core.Models
{
    public class DownloadQueueEntry
    {
        public DownloadQueueEntry(
            string url,
            DownloadMode downloadMode)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));
            if (url == string.Empty) throw new ArgumentException(nameof(url));

            this.Url = url;
            this.DownloadMode = downloadMode;
        }

        public string Url { get; }

        public DownloadMode DownloadMode { get; }
    }
}