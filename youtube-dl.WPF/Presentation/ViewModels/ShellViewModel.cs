using System;
using System.Reactive.Disposables;
using System.Reflection;
using Caliburn.Micro;
using youtube_dl.WPF.Core;
using youtube_dl.WPF.Presentation.Views;

namespace youtube_dl.WPF.Presentation.ViewModels
{
    public class ShellViewModel : Conductor<Caliburn.Micro.IScreen>.Collection.AllActive
    {
        private readonly YouTubeDL _youTubeDL;

        public ShellViewModel(
            YouTubeDL youTubeDLService,
            //AddDownloadQueueEntryViewModel addDownloadQueueEntryViewModel,
            NewDownloadCommandViewModel newDownloadCommandViewModel,
            DownloadQueueViewModel downloadQueueViewModel,
            YouTubeDLInstanceHandlersViewModel executingCommandsViewModel,
            DownloadViewModel downloadViewModel,
            UtilsViewModel utilsViewModel)
        {
            this._youTubeDL = youTubeDLService ?? throw new ArgumentNullException(nameof(youTubeDLService));

            //this.AddDownloadQueueEntryViewModel = addDownloadQueueEntryViewModel ?? throw new ArgumentNullException(nameof(addDownloadQueueEntryViewModel));
            this.NewDownloadCommandViewModel = newDownloadCommandViewModel ?? throw new ArgumentNullException(nameof(newDownloadCommandViewModel));
            this.DownloadQueueViewModel = downloadQueueViewModel ?? throw new ArgumentNullException(nameof(downloadQueueViewModel));
            this.ExecutingCommandsViewModel = executingCommandsViewModel ?? throw new ArgumentNullException(nameof(executingCommandsViewModel));
            this.DownloadViewModel = downloadViewModel ?? throw new ArgumentNullException(nameof(downloadViewModel));
            this.UtilsViewModel = utilsViewModel ?? throw new ArgumentNullException(nameof(utilsViewModel));

            //this.ActivateItem(this.AddDownloadQueueEntryViewModel);
            this.ActivateItem(this.NewDownloadCommandViewModel);
            this.ActivateItem(this.DownloadQueueViewModel);
            this.ActivateItem(this.DownloadViewModel);
            this.ActivateItem(this.UtilsViewModel);

            this.DisplayName = Assembly.GetEntryAssembly().GetAssemblyName();
        }

        //public AddDownloadQueueEntryViewModel AddDownloadQueueEntryViewModel { get; }
        public NewDownloadCommandViewModel NewDownloadCommandViewModel { get; }
        public DownloadQueueViewModel DownloadQueueViewModel { get; }
        public YouTubeDLInstanceHandlersViewModel ExecutingCommandsViewModel { get; }
        public DownloadViewModel DownloadViewModel { get; }
        public UtilsViewModel UtilsViewModel { get; }
    }
}
