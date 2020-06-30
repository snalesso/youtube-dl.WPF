using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace youtube_dl.WPF.Core.Queue
{
    public class EnqueuedDownloadCommand : ValueObject<EnqueuedDownloadCommand>
    {
        public EnqueuedDownloadCommand(DownloadCommand command)
        {
            this.Command = command ?? throw new ArgumentNullException(nameof(command));
        }

        DownloadCommand Command { get; }
        DateTime EnqueuedDateTime { get; } = DateTime.Now;

        protected override IEnumerable<object> GetValueIngredients()
        {
            yield return this.Command;
            yield return this.EnqueuedDateTime;
        }

        //private readonly BehaviorSubject<DownloadStatus > behaviorSubject
        //IObservable<DownloadStatus> DownloadStatus { get; }
    }
}
