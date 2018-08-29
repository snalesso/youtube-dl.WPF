using Caliburn.Micro;
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
using youtube_dl.WPF.Core.Models;
using youtube_dl.WPF.Core.Services;

namespace youtube_dl.WPF.Presentation.ViewModels
{
    public class AddDownloadQueueEntryViewModel : Screen
    {
        private readonly IDownloadQueueService _downloadQueueService;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public AddDownloadQueueEntryViewModel(IDownloadQueueService downloadQueueService)
        {
            this._downloadQueueService = downloadQueueService ?? throw new ArgumentNullException(nameof(downloadQueueService));

            this.PastFromClipboard = ReactiveCommand.Create(() => Clipboard.GetText(TextDataFormat.Text)).DisposeWith(this._disposables);
            this.PastFromClipboard.Subscribe(pastedUrl => this.Url = pastedUrl).DisposeWith(this._disposables);
            this.PastFromClipboard.ThrownExceptions.Subscribe(ex => Console.WriteLine(ex.ToString())).DisposeWith(this._disposables);

            this.EnqueueFromClipboard = ReactiveCommand.Create(
                () =>
                {
                    var clipboardText = Clipboard.GetText(TextDataFormat.Text);
                    if (!string.IsNullOrWhiteSpace(clipboardText))
                    {
                        this._downloadQueueService.Enqueue(new DownloadQueueEntry(clipboardText));
                    }
                }).DisposeWith(this._disposables);
            this.EnqueueFromClipboard.ThrownExceptions.Subscribe(ex => Console.WriteLine(ex.ToString())).DisposeWith(this._disposables);

            this.Enqueue = ReactiveCommand.Create(
                () =>
                {
                    this._downloadQueueService.Enqueue(new DownloadQueueEntry(this.Url));
                    this.Url = null;
                },
                this.WhenAnyValue(vm => vm.Url).DistinctUntilChanged().Select(url => !string.IsNullOrEmpty(url))).DisposeWith(this._disposables);
            this.Enqueue.ThrownExceptions.Subscribe(ex => Console.WriteLine(ex.ToString())).DisposeWith(this._disposables);
        }

        private string _url;
        public string Url
        {
            get { return this._url; }
            set { this.Set(ref this._url, value); }
        }

        public ReactiveCommand<Unit, string> PastFromClipboard { get; }
        public ReactiveCommand<Unit, Unit> EnqueueFromClipboard { get; }
        public ReactiveCommand<Unit, Unit> Enqueue { get; }
    }
}
