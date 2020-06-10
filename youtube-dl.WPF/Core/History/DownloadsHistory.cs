using DynamicData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using youtube_dl.WPF.Core.Models;

namespace youtube_dl.WPF.Core.Services
{
    public class DownloadsHistory
    {
        public DownloadsHistory() { }

        private readonly ISourceList<DownloadHistoryEntry> _historyEntries = new SourceList<DownloadHistoryEntry>();
        public IObservableList<DownloadHistoryEntry> HistoryEntries => this._historyEntries;

        public void Log(DownloadHistoryEntry entry)
        {
            this._historyEntries.Add(entry);
        }

        public void Log(IReadOnlyList<DownloadHistoryEntry> entries)
        {
            this._historyEntries.AddRange(entries);
        }

        public void Remove(DownloadHistoryEntry entry)
        {
            if (this._historyEntries.Items.Contains(entry))
            {
                this._historyEntries.Remove(entry);
            }
        }

        public void Clear()
        {
            this._historyEntries.Clear();
        }
    }
}