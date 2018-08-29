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
    public class DownloadQueueEntryViewModel
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

        public string Url => this.DownloadQueueEntry.Url;

        //public ReactiveCommand<Unit, Unit> Download { get; }
    }
}
