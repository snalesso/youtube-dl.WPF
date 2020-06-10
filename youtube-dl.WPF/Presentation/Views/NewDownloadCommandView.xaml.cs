using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using youtube_dl.WPF.Presentation.ViewModels;

namespace youtube_dl.WPF.Presentation.Views
{
    /// <summary>
    /// Interaction logic for AddQueueEntryView.xaml
    /// </summary>
    public partial class NewDownloadCommandView : UserControl, IViewFor<NewDownloadCommandViewModel>
    {
        #region constants & fields

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        #endregion

        #region ctor

        public NewDownloadCommandView()
        {
            this.InitializeComponent();

            // START - IViewFor<>

            this._viewModelSubject = new BehaviorSubject<NewDownloadCommandViewModel>(this.DataContext as NewDownloadCommandViewModel);
            // when .DataContext changes => update .ViewModel
            this.Events()
                .DataContextChanged
                .Subscribe(dc => this._viewModelSubject.OnNext(dc.NewValue as NewDownloadCommandViewModel))
                .DisposeWith(this._disposables);
            this.WhenViewModelChanged = this._viewModelSubject.AsObservable().DistinctUntilChanged();

            // END - IViewFor<>
        }

        #endregion

        #region IViewFor

        private readonly BehaviorSubject<NewDownloadCommandViewModel> _viewModelSubject;
        public NewDownloadCommandViewModel ViewModel
        {
            get => this._viewModelSubject.Value;
            set => this.DataContext = value;
        }
        public IObservable<NewDownloadCommandViewModel> WhenViewModelChanged { get; }

        object IViewFor.ViewModel
        {
            get => this.ViewModel;
            set => this.ViewModel = (value as NewDownloadCommandViewModel);
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            nameof(IViewFor.ViewModel), 
            typeof(NewDownloadCommandViewModel), 
            typeof(NewDownloadCommandView));


        #endregion
    }
}