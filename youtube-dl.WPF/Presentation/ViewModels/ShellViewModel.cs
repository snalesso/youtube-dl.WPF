using Caliburn.Micro;
using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reflection;
using youtube_dl.WPF.Core.Services;

namespace youtube_dl.WPF.Presentation.ViewModels
{
    public class ShellViewModel : Conductor<Caliburn.Micro.IScreen>.Collection.AllActive
    {
        private readonly IYouTubeDLService _youTubeDLService;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public ShellViewModel(
            IYouTubeDLService youTubeDLService,
            AddDownloadQueueEntryViewModel addDownloadQueueEntryViewModel,
            DownloadQueueViewModel downloadQueueViewModel,
            DownloadViewModel downloadViewModel,
            UtilsViewModel utilsViewModel)
        {
            this._youTubeDLService = youTubeDLService ?? throw new ArgumentNullException(nameof(youTubeDLService));

            this.AddDownloadQueueEntryViewModel = addDownloadQueueEntryViewModel ?? throw new ArgumentNullException(nameof(addDownloadQueueEntryViewModel));
            this.DownloadQueueViewModel = downloadQueueViewModel ?? throw new ArgumentNullException(nameof(downloadQueueViewModel));
            this.DownloadViewModel = downloadViewModel ?? throw new ArgumentNullException(nameof(downloadViewModel));
            this.UtilsViewModel = utilsViewModel ?? throw new ArgumentNullException(nameof(utilsViewModel));

            this.ActivateItem(this.AddDownloadQueueEntryViewModel);
            this.ActivateItem(this.DownloadQueueViewModel);
            this.ActivateItem(this.DownloadViewModel);
            this.ActivateItem(this.UtilsViewModel);

            this.DisplayName = Assembly.GetEntryAssembly().GetAssemblyName();
        }

        public AddDownloadQueueEntryViewModel AddDownloadQueueEntryViewModel { get; }
        public DownloadQueueViewModel DownloadQueueViewModel { get; }
        public DownloadViewModel DownloadViewModel { get; }
        public UtilsViewModel UtilsViewModel { get; }
    }
}
