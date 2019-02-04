using ReactiveUI;
using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using youtube_dl.WPF.Presentation.ViewModels;

namespace youtube_dl.WPF.Presentation.Views
{
    /// <summary>
    /// Interaction logic for BinariesCheckerView.xaml
    /// </summary>
    public partial class DependenciesCheckerView : Window, IViewFor<DependenciesCheckerViewModel>
    {
        #region constants & fields

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        #endregion

        #region ctor

        public DependenciesCheckerView()
        {
            this.InitializeComponent();

            this._viewModelSubject = new BehaviorSubject<DependenciesCheckerViewModel>(this.DataContext as DependenciesCheckerViewModel);

            // when .DataContext changes => update .ViewModel
            this.Events()
                .DataContextChanged
                .Subscribe(dc => this._viewModelSubject.OnNext(dc.NewValue as DependenciesCheckerViewModel))
                .DisposeWith(this._disposables);
            this.WhenViewModelChanged = this._viewModelSubject.AsObservable().DistinctUntilChanged();

            // when view is shown check for clients and updates

            //this.Events().Loaded.Subscribe(s => Console.WriteLine("Loaded")).DisposeWith(this._disposables);

            //this.WhenActivated(async disposables =>
            //{
            //    //await this.ViewModel.CheckForYouTubeDLUpdates.Execute();
            //    //var isFFmpegPresent = await this.ViewModel.EnsureFFmpegPresence.Execute();
            //    //if (!isFFmpegPresent)
            //    //    this.ViewModel.TryClose();
            //}).DisposeWith(this._disposables);
        }

        #endregion

        #region IViewFor

        private readonly BehaviorSubject<DependenciesCheckerViewModel> _viewModelSubject;
        public DependenciesCheckerViewModel ViewModel
        {
            get => this._viewModelSubject.Value;
            set => this.DataContext = value;
        }
        public IObservable<DependenciesCheckerViewModel> WhenViewModelChanged { get; }

        object IViewFor.ViewModel
        {
            get => this.ViewModel;
            set => this.ViewModel = (value as DependenciesCheckerViewModel);
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}