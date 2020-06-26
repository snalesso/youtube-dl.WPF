using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ReactiveUI;
using youtube_dl.WPF.Presentation.ViewModels;

namespace youtube_dl.WPF.Presentation.Views
{
    /// <summary>
    /// Interaction logic for PostProcessingView.xaml
    /// </summary>
    public partial class PostProcessingView : UserControl, IViewFor<PostProcessingViewModel>
    {
        #region constants & fields

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        #endregion

        #region ctor

        public PostProcessingView()
        {
            this.InitializeComponent();

            // START - IViewFor<>

            this._viewModelSubject = new BehaviorSubject<PostProcessingViewModel>(this.DataContext as PostProcessingViewModel);
            // when .DataContext changes => update .ViewModel
            this.Events()
                .DataContextChanged
                .Subscribe(dc => this._viewModelSubject.OnNext(dc.NewValue as PostProcessingViewModel))
                .DisposeWith(this._disposables);
            this.WhenViewModelChanged = this._viewModelSubject.AsObservable().DistinctUntilChanged();

            // END - IViewFor<>
        }

        #endregion

        #region IViewFor

        private readonly BehaviorSubject<PostProcessingViewModel> _viewModelSubject;
        public PostProcessingViewModel ViewModel
        {
            get => this._viewModelSubject.Value;
            set => this.DataContext = value;
        }
        public IObservable<PostProcessingViewModel> WhenViewModelChanged { get; }

        object IViewFor.ViewModel
        {
            get => this.ViewModel;
            set => this.ViewModel = (value as PostProcessingViewModel);
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            nameof(IViewFor.ViewModel),
            typeof(PostProcessingViewModel),
            typeof(PostProcessingView));


        #endregion
    }
}