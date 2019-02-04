using Caliburn.Micro.ReactiveUI;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using youtube_dl.WPF.Core.Models;
using youtube_dl.WPF.Core.Services;

namespace youtube_dl.WPF.Presentation.ViewModels
{
    public class DownloadQueueEntryViewModel : ReactiveScreen
    {
        //private readonly DownloadQueueService _downloadQueueService;

        public DownloadQueueEntryViewModel(
            //DownloadQueueService downloadQueueService,
            DownloadQueueEntry downloadQueueEntry)
        {
            //this._downloadQueueService = downloadQueueService ?? throw new ArgumentNullException(nameof(downloadQueueService));

            this.DownloadQueueEntry = downloadQueueEntry;

            //this.Download = ReactiveCommand.Create(
            //    () =>
            //    {
            //    });
        }

        public DownloadQueueEntry DownloadQueueEntry { get; }

        private string _downloadModeKey;
        public string DownloadModeKey => this._downloadModeKey ?? (this._downloadModeKey = typeof(DownloadMode).GetEnumName(this.DownloadQueueEntry.DownloadMode));

        public string Url => this.DownloadQueueEntry.Url;

        //public ReactiveCommand<Unit, Unit> Download { get; }
    }
}
