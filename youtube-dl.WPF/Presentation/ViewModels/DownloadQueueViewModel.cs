using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using DynamicData;
using DynamicData.Alias;
using DynamicData.PLinq;
using ReactiveUI;
using youtube_dl.WPF.Core;
using youtube_dl.WPF.Core.Queue;
using youtube_dl.WPF.Core.Services;

namespace youtube_dl.WPF.Presentation.ViewModels
{
    public class DownloadQueueViewModel : ReactiveScreen
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private readonly YouTubeDL _youTubeDL;
        private readonly DownloadCommandsQueue _downloadCommandsQueue;

        public DownloadQueueViewModel(
            YouTubeDL youTubeDL,
            DownloadCommandsQueue downloadQueueService)
        {
            this._youTubeDL = youTubeDL ?? throw new ArgumentNullException(nameof(youTubeDL));
            this._downloadCommandsQueue = downloadQueueService ?? throw new ArgumentNullException(nameof(downloadQueueService));

            this._downloadCommandsQueue.Entries.Connect()
                .Transform(qe => new DownloadQueueEntryViewModel(qe))
                .DisposeMany()
                .Bind(out this._downloadQueueEntryViewModels_rooc)
                .Subscribe()
                .DisposeWith(this._disposables);

            this.EmptyQueue = ReactiveCommand.Create(
                this._downloadCommandsQueue.EmptyQueue,
                this._downloadCommandsQueue.Entries.CountChanged.Select(count => count != 0));
            this.EmptyQueue.ThrownExceptions.Subscribe(ex => Console.WriteLine(ex.ToString())).DisposeWith(this._disposables);
            this.EmptyQueue.DisposeWith(this._disposables);

            this.RemoveSelected = ReactiveCommand.Create(
                () =>
                {
                    this._downloadCommandsQueue.Remove(this.SelectedDownloadQueueEntryViewModel.DownloadQueueEntry);
                },
                this.WhenAnyValue(vm => vm.SelectedDownloadQueueEntryViewModel).Select(entry => entry != null));
            this.RemoveSelected.ThrownExceptions.Subscribe(ex => Console.WriteLine(ex.ToString())).DisposeWith(this._disposables);
            this.RemoveSelected.DisposeWith(this._disposables);
        }

        private readonly ReadOnlyObservableCollection<DownloadQueueEntryViewModel> _downloadQueueEntryViewModels_rooc;
        public ReadOnlyObservableCollection<DownloadQueueEntryViewModel> DownloadQueueEntryViewModels => this._downloadQueueEntryViewModels_rooc;

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