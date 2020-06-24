using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using youtube_dl.WPF.Core;
using youtube_dl.WPF.Core.Queue;
using youtube_dl.WPF.Core.Services;

namespace youtube_dl.WPF.Presentation.ViewModels
{
    public class AddDownloadQueueEntryViewModel : ReactiveScreen
    {
        private readonly DownloadCommandsQueue _downloadCommandsQueue;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public AddDownloadQueueEntryViewModel(DownloadCommandsQueue downloadCommandsQueue)
        {
            this._downloadCommandsQueue = downloadCommandsQueue ?? throw new ArgumentNullException(nameof(downloadCommandsQueue));

            this.ReadClipboard = ReactiveCommand.Create(() => Clipboard.GetText(TextDataFormat.Text));
            this.ReadClipboard.Subscribe(pastedUrl => this.Url = pastedUrl).DisposeWith(this._disposables);
            this.ReadClipboard.ThrownExceptions.Subscribe(ex => Console.WriteLine(ex.ToString())).DisposeWith(this._disposables);
            this.ReadClipboard.DisposeWith(this._disposables);

            this.EnqueueFromClipboard = ReactiveCommand.Create(
                () =>
                {
                    var clipboardText = Clipboard.GetText(TextDataFormat.Text);
                    if (!string.IsNullOrWhiteSpace(clipboardText))
                    {
                        this._downloadCommandsQueue.Enqueue(new DownloadCommand(this.Url, new DownloadCommandOptions(this.SelectedDownloadMode)));
                    }
                });
            this.EnqueueFromClipboard.ThrownExceptions.Subscribe(ex => Console.WriteLine(ex.ToString())).DisposeWith(this._disposables);
            this.EnqueueFromClipboard.DisposeWith(this._disposables); 

            this.Enqueue = ReactiveCommand.Create(
                () =>
                {
                    this._downloadCommandsQueue.Enqueue(new DownloadCommand(this.Url, new DownloadCommandOptions(this.SelectedDownloadMode)));
                    this.Url = null;
                },
                this.WhenAnyValue(vm => vm.Url).DistinctUntilChanged().Select(url => !string.IsNullOrEmpty(url)));
            this.Enqueue.ThrownExceptions.Subscribe(ex => Console.WriteLine(ex.ToString())).DisposeWith(this._disposables);
            this.Enqueue.DisposeWith(this._disposables);
        }

        private string _url;
        public string Url
        {
            get { return this._url; }
            set { this.RaiseAndSetIfChanged(ref this._url, value); }
        }

        private readonly IReadOnlyList<DownloadMode> _downloadModes = new[] { DownloadMode.AudioOnly, DownloadMode.AudioVideo };
        public IReadOnlyList<DownloadMode> DownloadModes
        {
            get { return this._downloadModes ?? (this.DownloadModes); }
        }

        private DownloadMode _selectedDownloadMode = DownloadMode.AudioOnly;
        public DownloadMode SelectedDownloadMode
        {
            get { return this._selectedDownloadMode; }
            set { this.RaiseAndSetIfChanged(ref this._selectedDownloadMode, value); }
        }

        public ReactiveCommand<Unit, string> ReadClipboard { get; }
        public ReactiveCommand<Unit, Unit> EnqueueFromClipboard { get; }
        public ReactiveCommand<Unit, Unit> Enqueue { get; }
    }
}
