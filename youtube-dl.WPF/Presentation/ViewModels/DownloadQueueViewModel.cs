using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using youtube_dl.WPF.Core.Models;
using youtube_dl.WPF.Core.Services;

namespace youtube_dl.WPF.Presentation.ViewModels
{
    public class DownloadQueueViewModel : ReactiveScreen
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        private readonly IDownloadQueueService _downloadQueueService;

        public DownloadQueueViewModel(IDownloadQueueService downloadQueueService)
        {
            this._downloadQueueService = downloadQueueService ?? throw new ArgumentNullException(nameof(downloadQueueService));

            this.DownloadQueueEntryViewModels = this._downloadQueueService.QueueEntries.CreateDerivedCollection(qe => new DownloadQueueEntryViewModel(qe));

            this.EmptyQueue = ReactiveCommand.Create(
                this._downloadQueueService.Empty,
                this._downloadQueueService.QueueEntries.CountChanged.StartWith(this._downloadQueueService.QueueEntries.Count()).Select(count => count > 0));
            this.EmptyQueue.ThrownExceptions.Subscribe(ex => Console.WriteLine(ex.ToString())).DisposeWith(this._disposables);
            this.RemoveSelected = ReactiveCommand.Create(
                () =>
                {
                    this._downloadQueueService.Extract(this.SelectedDownloadQueueEntryViewModel.DownloadQueueEntry);
                },
                this.WhenAnyValue(vm => vm.SelectedDownloadQueueEntryViewModel).Select(entry => entry != null));
            this.EmptyQueue.ThrownExceptions.Subscribe(ex => Console.WriteLine(ex.ToString())).DisposeWith(this._disposables);
        }

        public IReactiveDerivedList<DownloadQueueEntryViewModel> DownloadQueueEntryViewModels { get; }

        private DownloadQueueEntryViewModel _selectedDownloadQueueEntryViewModel;
        public DownloadQueueEntryViewModel SelectedDownloadQueueEntryViewModel
        {
            get { return this._selectedDownloadQueueEntryViewModel; }
            set { this.RaiseAndSetIfChanged(ref this._selectedDownloadQueueEntryViewModel, value); }
        }

        public ReactiveCommand<Unit, Unit> EmptyQueue { get; }

        public ReactiveCommand<Unit, Unit> RemoveSelected { get; }
    }
}