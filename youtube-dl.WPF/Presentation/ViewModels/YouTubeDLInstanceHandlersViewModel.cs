using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro.ReactiveUI;
using DynamicData;
using ReactiveUI;
using youtube_dl.WPF.Core;

namespace youtube_dl.WPF.Presentation.ViewModels
{
    // TODO: handle download interruption on close
    public class YouTubeDLInstanceHandlersViewModel : ReactiveScreen, IDisposable
    {
        #region constants & fields

        private readonly YouTubeDL _youTubeDL;

        #endregion

        #region ctors

        public YouTubeDLInstanceHandlersViewModel(
            YouTubeDL youTubeDL            )
        {
            this._youTubeDL = youTubeDL ?? throw new ArgumentNullException(nameof(youTubeDL));

            this._youTubeDL.ExecutingInstances.Connect()
                .Transform(instanceHandler => new YouTubeDLInstanceHandlerViewModel(instanceHandler))
                .DisposeMany()
                .Bind(out this._youTubeDLInstanceHandlerViewModel_rooc)
                .Subscribe()
                .DisposeWith(this._disposables);
        }

        #endregion

        #region properties

        private readonly ReadOnlyObservableCollection<YouTubeDLInstanceHandlerViewModel> _youTubeDLInstanceHandlerViewModel_rooc;
        public ReadOnlyObservableCollection<YouTubeDLInstanceHandlerViewModel> YouTubeDLInstanceHandlerViewModels => this._youTubeDLInstanceHandlerViewModel_rooc;

        private YouTubeDLInstanceHandlerViewModel _selectedYouTubeDLInstanceHandlerViewModel;
        public YouTubeDLInstanceHandlerViewModel SelectedYouTubeDLInstanceHandlerViewModel
        {
            get { return this._selectedYouTubeDLInstanceHandlerViewModel; }
            set { this.RaiseAndSetIfChanged(ref this._selectedYouTubeDLInstanceHandlerViewModel, value); }
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
