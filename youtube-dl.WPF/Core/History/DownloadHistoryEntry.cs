using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace youtube_dl.WPF.Core.History
{
    public class DownloadHistoryEntry
    {
        public DownloadHistoryEntry(
            DownloadCommand downloadable,
            string downloadFilePath,
            DateTime downloadedDateTime)
        {
            if (string.IsNullOrWhiteSpace(downloadFilePath))
                throw new ArgumentException($"{nameof(downloadFilePath)} cannot be null", downloadFilePath);

            this.Downloadable = downloadable ?? throw new ArgumentNullException(nameof(downloadable));
            this.DownloadFilePath = downloadFilePath;
        }

        public DownloadHistoryEntry(
            DownloadCommand downloadable,
            string downloadFilePath)
            : this(
                  downloadable,
                  downloadFilePath,
                  DateTime.Now)
        { }

        public DownloadCommand Downloadable { get; }
        public string DownloadFilePath { get; }
        public DateTime DownloadedDateTime { get; }
    }
}