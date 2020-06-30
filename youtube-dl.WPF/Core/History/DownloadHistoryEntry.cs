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
            DownloadCommand command,
            string downloadedFilePath,
            DateTime downloadedDateTime)
        {
            this.Command = command ?? throw new ArgumentNullException(nameof(command));
            this.DownloadedFilePath = !string.IsNullOrWhiteSpace(downloadedFilePath)
                ? downloadedFilePath
                : throw new ArgumentException($"{nameof(downloadedFilePath)} cannot be null", downloadedFilePath);
        }

        public DownloadHistoryEntry(
            DownloadCommand command,
            string downloadFilePath)
            : this(
                  command,
                  downloadFilePath,
                  DateTime.Now)
        { }

        public DownloadCommand Command { get; }
        public string ErrorMessage { get; }
        public string DownloadedFilePath { get; }
        public DateTime DownloadedDateTime { get; }
    }
}