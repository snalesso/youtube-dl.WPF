using System;
using System.Linq;
using System.Reactive.Linq;

namespace youtube_dl.WPF.Core
{
    public static class YouTubeDLInstanceHandlerMixins
    {
        public static IObservable<YouTubeDLInstanceStatus> WhenProcessEnded(this YouTubeDLInstanceHandler youTubeDLInstanceHandler)
        {
            if (youTubeDLInstanceHandler == null)
                throw new ArgumentNullException(nameof(youTubeDLInstanceHandler));

            return youTubeDLInstanceHandler.WhenStatusChanged
                //.Where(status =>
                //{
                //    return status == YouTubeDLInstanceStatus.Completed
                //        || status == YouTubeDLInstanceStatus.Canceled
                //        || status == YouTubeDLInstanceStatus.Failed;
                //})
                .LastAsync();
        }
    }
}