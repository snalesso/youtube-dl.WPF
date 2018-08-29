using ReactiveUI;
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
using youtube_dl.WPF.Presentation.ViewModels;

namespace youtube_dl.WPF.Presentation.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ShellView : Window, IViewFor<ShellViewModel>, IDisposable
    {
        #region constants & fields

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        #endregion

        #region ctor

        public ShellView()
        {
            this.InitializeComponent();

            this._viewModelSubject = new BehaviorSubject<ShellViewModel>(this.DataContext as ShellViewModel);
            // when .DataContext changes => update .ViewModel
            this.Events()
                .DataContextChanged
                .Subscribe(dc => this._viewModelSubject.OnNext(dc.NewValue as ShellViewModel))
                .DisposeWith(this._disposables);
            this.WhenViewModelChanged = this._viewModelSubject.AsObservable().DistinctUntilChanged();
        }

        #endregion

        #region IViewFor

        private readonly BehaviorSubject<ShellViewModel> _viewModelSubject;
        public ShellViewModel ViewModel
        {
            get => this._viewModelSubject.Value;
            set => this.DataContext = value; // ?? throw new ArgumentNullException(nameof(value))); // TODO: localize
        }
        public IObservable<ShellViewModel> WhenViewModelChanged { get; }

        object IViewFor.ViewModel
        {
            get => this.ViewModel;
            set => this.ViewModel = (value as ShellViewModel);
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
