using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro.ReactiveUI;
using youtube_dl.WPF.Core;

namespace youtube_dl.WPF.Presentation.ViewModels
{
    public class YouTubeDLInstanceHandlerViewModel : ReactiveScreen, IDisposable
    {
        #region constants & fields

        private readonly YouTubeDLInstanceHandler _youTubeDLInstanceHandler;

        #endregion

        #region ctors

        public YouTubeDLInstanceHandlerViewModel(YouTubeDLInstanceHandler youTubeDLInstanceHandler)
        {
            this._youTubeDLInstanceHandler = youTubeDLInstanceHandler ?? throw new ArgumentNullException(nameof(youTubeDLInstanceHandler));
        }

        #endregion

        #region properties

        public YouTubeDLInstanceHandler InstanceHandler => this._youTubeDLInstanceHandler;

        public IYouTubeDLCommand Command => this._youTubeDLInstanceHandler.Command;

        public float DownloadedPercent { get; }

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
