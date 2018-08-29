using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace youtube_dl.WPF.Core.Models
{
    public class DownloadHistoryEntry
    {
        public DownloadHistoryEntry(
            string url,
            string downloadFilePath)
        {
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentException($"{nameof(url)} cannot be null", url);
            if (string.IsNullOrWhiteSpace(downloadFilePath)) throw new ArgumentException($"{nameof(downloadFilePath)} cannot be null", downloadFilePath);

            this.Url = url ?? throw new ArgumentNullException(nameof(url));
            this.DownloadFilePath = downloadFilePath;
        }

        public string Url { get; }

        public string DownloadFilePath { get; }
    }
}