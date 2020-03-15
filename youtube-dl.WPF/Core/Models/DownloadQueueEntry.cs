using System;
using System.Collections.Generic;

namespace youtube_dl.WPF.Core.Models
{
    public class DownloadQueueEntry : ValueObject<DownloadQueueEntry>
    {
        public DownloadQueueEntry(
            string url,
            DownloadOptions options)
        {
            // TODO: check more in deep
            if (url == null)
                throw new ArgumentNullException(nameof(url));
            if (url == string.Empty)
                throw new ArgumentException(nameof(url));
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            this.Url = url;
            this.Options = options;
        }

        public string Url { get; }

        public DownloadOptions Options { get; }

        protected override IEnumerable<object> GetValueIngredients()
        {
            yield return this.Url;
            yield return this.Options;
        }
    }
}