using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro.ReactiveUI;
using ReactiveUI;
using youtube_dl.WPF.Core;

namespace youtube_dl.WPF.Presentation.ViewModels
{
    public class PostProcessingViewModel : ReactiveScreen, IDisposable
    {
        #region constants & fields
        #endregion

        #region ctors

        public PostProcessingViewModel(IObservable<DownloadMode> whenDownloadModeChanged)
        {
            this._whenDownloadModeChanged = whenDownloadModeChanged ?? throw new ArgumentNullException(nameof(whenDownloadModeChanged));

            //this.VideoContainerFormats = new VideoContainerFormat?[] {null ,  }.ToImmutableArray();
            this.AudioCodecOptions = new AudioCodec?[] { /*null*/ }.Concat(Enum.GetValues(typeof(AudioCodec)).Cast<AudioCodec?>()).ToImmutableArray();
            this.VideoContainerFormatOptions = new VideoContainerFormat?[] { /*null */}.Concat(Enum.GetValues(typeof(VideoContainerFormat)).Cast<VideoContainerFormat?>()).ToImmutableArray();

            this._isAudioCodecSelectable_OAPH = this._whenDownloadModeChanged
                .Select(dm => dm == DownloadMode.AudioOnly)
                .ToProperty(this, nameof(this.IsAudioCodecSelectable))
                .DisposeWith(this._disposables);
            this._isVideoContainerFormatSelectable_OAPH = this._whenDownloadModeChanged
                .Select(dm => dm.HasFlag(DownloadMode.VideoOnly))
                .ToProperty(this, nameof(this.IsVideoContainerFormatSelectable))
                .DisposeWith(this._disposables);
        }

        #endregion

        #region properties

        private readonly IObservable<DownloadMode> _whenDownloadModeChanged;

        //private DownloadMode _downloadMode;
        //public DownloadMode DonwloadMode
        //{
        //    get { return this._downloadMode; }
        //    set { this.RaiseAndSetIfChanged(ref this._downloadMode, value); }
        //}

        private readonly ObservableAsPropertyHelper<bool> _isAudioCodecSelectable_OAPH;
        public bool IsAudioCodecSelectable => this._isAudioCodecSelectable_OAPH.Value;

        private readonly ObservableAsPropertyHelper<bool> _isVideoContainerFormatSelectable_OAPH;
        public bool IsVideoContainerFormatSelectable => this._isVideoContainerFormatSelectable_OAPH.Value;

        // audio

        public IReadOnlyList<AudioCodec?> AudioCodecOptions { get; }

        private AudioCodec? _selectedAudioCodec;
        public AudioCodec? SelectedAudioCodec
        {
            get { return this._selectedAudioCodec; }
            set { this.RaiseAndSetIfChanged(ref this._selectedAudioCodec, value); }
        }

        // video

        public IReadOnlyList<VideoContainerFormat?> VideoContainerFormatOptions { get; }

        private VideoContainerFormat? _selectedVideoContainerFormat;
        public VideoContainerFormat? SelectedVideoContainerFormat
        {
            get { return this._selectedVideoContainerFormat; }
            set { this.RaiseAndSetIfChanged(ref this._selectedVideoContainerFormat, value); }
        }

        #endregion

        #region methods
        #endregion

        #region commands
        #endregion

        #region IDisposable

        // https://docs.microsoft.com/en-us/dotnet/api/system.idisposable?view=netframework-4.8
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isDisposed = false;

        // use this in derived class
        // protected override void Dispose(bool isDisposing)
        // use this in non-derived class
        protected virtual void Dispose(bool isDisposing)
        {
            if (this._isDisposed)
                return;

            if (isDisposing)
            {
                // free managed resources here
                this._disposables.Dispose();
            }

            // free unmanaged resources (unmanaged objects) and override a finalizer below.
            // set large fields to null.

            this._isDisposed = true;

            // remove in non-derived class
            //base.Dispose(isDisposing);
        }

        // remove if in derived class
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool isDisposing) above.
            this.Dispose(true);
        }

        #endregion
    }
}