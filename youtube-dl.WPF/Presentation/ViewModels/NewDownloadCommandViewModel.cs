using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
    public class NewDownloadCommandViewModel : ReactiveScreen
    {
        private readonly DownloadCommandsQueue _downloadCommandsQueue;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public NewDownloadCommandViewModel(
            DownloadCommandsQueue downloadCommandsQueue,
            Func<IObservable<DownloadMode>, PostProcessingViewModel> postProcessingViewModel_FactoryMethod)
        {
            this._downloadCommandsQueue = downloadCommandsQueue ?? throw new ArgumentNullException(nameof(downloadCommandsQueue));
            this.PostProcessingViewModel = (postProcessingViewModel_FactoryMethod ?? throw new ArgumentNullException(nameof(postProcessingViewModel_FactoryMethod)))
                .Invoke(this.WhenAny(x => x.SelectedDownloadMode, x => x.Value));

            this.ReadClipboard = ReactiveCommand.Create(() => Clipboard.GetText(TextDataFormat.Text));
            this.ReadClipboard.Subscribe(
                clipboardText =>
                {
                    this.Url = URLsHelper.JoinUrlsList(URLsHelper.ParseUrlsList(clipboardText));
                })
                .DisposeWith(this._disposables);
            this.ReadClipboard.ThrownExceptions.Subscribe(ex => Console.WriteLine(ex.ToString())).DisposeWith(this._disposables);
            this.ReadClipboard.DisposeWith(this._disposables);

            this.EnqueueFromClipboard = ReactiveCommand.Create(
                () =>
                {
                    var clipboardText = Clipboard.GetText(TextDataFormat.Text);
                    if (clipboardText != null)
                    {
                        var clipboardUrls = URLsHelper.ParseUrlsList(clipboardText);
                        foreach (var clipboardUrl in clipboardUrls)
                        {
                            this._downloadCommandsQueue.Enqueue(
                                new DownloadCommand(
                                    clipboardUrl,
                                    this.GetDownloadCommandOptions()));
                        }
                    }
                });
            this.EnqueueFromClipboard.ThrownExceptions.Subscribe(ex => Console.WriteLine(ex.ToString())).DisposeWith(this._disposables);
            this.EnqueueFromClipboard.DisposeWith(this._disposables);

            this.Enqueue = ReactiveCommand.Create(
                () =>
                {
                    foreach (var url in URLsHelper.ParseUrlsList(this.Url))
                    {
                        this._downloadCommandsQueue.Enqueue(
                            new DownloadCommand(
                                url,
                                this.GetDownloadCommandOptions()));
                    }
                    this.Url = null;
                },
                this.WhenAnyValue(vm => vm.Url).DistinctUntilChanged().Select(url => !string.IsNullOrEmpty(url)));
            this.Enqueue.ThrownExceptions.Subscribe(ex => Console.WriteLine(ex.ToString())).DisposeWith(this._disposables);
            this.Enqueue.DisposeWith(this._disposables);
        }

        public PostProcessingViewModel PostProcessingViewModel { get; }

        private string _url;
        public string Url
        {
            get { return this._url; }
            set { this.RaiseAndSetIfChanged(ref this._url, value); }
        }

        //[Obsolete]
        //private bool _isAudioKept = true;
        //public bool IsAudioKept
        //{
        //    get { return this._isAudioKept; }
        //    set { this.RaiseAndSetIfChanged(ref this._isAudioKept, value); }
        //}

        //[Obsolete]
        //private bool _isVideoKept = true;
        //public bool IsVideoKept
        //{
        //    get { return this._isVideoKept; }
        //    set { this.RaiseAndSetIfChanged(ref this._isVideoKept, value); }
        //}

        private readonly IReadOnlyList<DownloadMode> _downloadModes = new[] { DownloadMode.AudioOnly, DownloadMode.AudioVideo };
        public IReadOnlyList<DownloadMode> DownloadModes
        {
            get { return this._downloadModes ?? (this.DownloadModes); }
        }

        // download options

        // output options
        private DownloadMode _selectedDownloadMode = DownloadMode.AudioOnly;
        public DownloadMode SelectedDownloadMode
        {
            get { return this._selectedDownloadMode; }
            set { this.RaiseAndSetIfChanged(ref this._selectedDownloadMode, value); }
        }
        public VideoContainerFormat? VideoContainerFormat { get; }

        public IReadOnlyList<IYouTubeDLQualitySelector> QualitySelectors { get; }
        //= new IYouTubeDLQualitySelector[]
        //{
        //    new VideoAudioQualitySelector(
        //        videoQuality: YouTubeDLQuality.Best,
        //        audioQuality: YouTubeDLQuality.Best),
        //    new GenericQualitySelector(YouTubeDLQuality.Best),
        //};
        = Workarounds.DefaultYouTubeDLQualitySelectors.ToImmutableArray();

        [Obsolete]
        public string QualitySelectorsString => this.QualitySelectors?.Serialize();

        private DownloadCommandOptions GetDownloadCommandOptions() =>
            new DownloadCommandOptions(
                qualitySelectors: this.QualitySelectors,
                outputAudioCodec: this.PostProcessingViewModel.SelectedOutputAudioCodec,
                videoContainerFormat: this.PostProcessingViewModel.SelectedVideoContainerFormat);

        public ReactiveCommand<Unit, string> ReadClipboard { get; }
        // TODO: this command duplicates implementation. Simplify.
        public ReactiveCommand<Unit, Unit> EnqueueFromClipboard { get; }
        public ReactiveCommand<Unit, Unit> Enqueue { get; }
    }
}
