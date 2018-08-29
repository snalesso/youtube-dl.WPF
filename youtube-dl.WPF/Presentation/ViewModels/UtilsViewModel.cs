using Caliburn.Micro;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using youtube_dl.WPF.Core.Services;

namespace youtube_dl.WPF.Presentation.ViewModels
{
    public class UtilsViewModel : Screen
    {
        private readonly IYouTubeDLService _youTubeDLService;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public UtilsViewModel(
            IYouTubeDLService youTubeDLService)
        {
            this._youTubeDLService = youTubeDLService ?? throw new ArgumentNullException(nameof(youTubeDLService));

            this.Update = ReactiveCommand.CreateFromTask(this._youTubeDLService.UpdateAsync, outputScheduler: RxApp.MainThreadScheduler);
            this.Update.ThrownExceptions.Subscribe(ex => Console.WriteLine(ex.ToString())).DisposeWith(this._disposables);
        }

        public ReactiveCommand<Unit, bool> Update { get; }
    }
}
