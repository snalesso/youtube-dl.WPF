using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;

namespace youtube_dl.WPF.Core.Models
{
    // TODO: rename to DownloadCommand?
    public class DownloadCommand : ValueObject<DownloadCommand>
    {
        public DownloadCommand(
            IEnumerable<string> urls,
            DownloadCommandOptions options)
        {
            this.Options = options ?? throw new ArgumentNullException(nameof(options));

            // TODO: check more in deep
            if (urls == null)
                throw new ArgumentNullException(nameof(urls));
            if (!urls.Any())
                throw new ArgumentException($"{nameof(urls)} cannot be empty");

            urls = urls.TrimAll();

            if (urls.Any(x => string.IsNullOrWhiteSpace(x)))
                throw new ArgumentException($"{nameof(urls)} contains empty urls");
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            this.Urls = urls.ToImmutableArray();
        }

        public DownloadCommand(
            string url,
            DownloadCommandOptions options)
            : this(ImmutableArray.Create(url), options) { }

        public IReadOnlyList<string> Urls { get; }

        public DownloadCommandOptions Options { get; }

        protected override IEnumerable<object> GetValueIngredients()
        {
            yield return this.Urls;
            yield return this.Options;
        }
    }
}