using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Linq;
using System.Reactive.Linq;

namespace youtube_dl.WPF.Core
{
    // TODO: rename to DownloadCommand?
    public class DownloadCommand : ValueObject<DownloadCommand>, IYouTubeDLCommand
    {
        //public DownloadCommand(
        //    IEnumerable<string> urls,
        //    DownloadCommandOptions options)
        //{
        //    this.Options = options ?? throw new ArgumentNullException(nameof(options));

        //    // TODO: check more in deep
        //    if (urls == null)
        //        throw new ArgumentNullException(nameof(urls));
        //    if (!urls.Any())
        //        throw new ArgumentException($"{nameof(urls)} cannot be empty");

        //    urls = urls.TrimAll();

        //    if (urls.Any(x => string.IsNullOrWhiteSpace(x)))
        //        throw new ArgumentException($"{nameof(urls)} contains empty urls");
        //    if (options == null)
        //        throw new ArgumentNullException(nameof(options));

        //    this.URLs = urls.ToImmutableArray();
        //}

        public DownloadCommand(
            string url,
            DownloadCommandOptions options)
        //: this(ImmutableArray.Create(url), options) { }
        {
            url = url?.Trim();

            this.URL = !string.IsNullOrWhiteSpace(url) ? url : throw new FormatException($"Argument \"{nameof(url)}\" is invalid");
            this.Options = options ?? throw new ArgumentNullException(nameof(options));
        }

        //public IReadOnlyList<string> URLs { get; }
        public string URL { get; }

        public DownloadCommandOptions Options { get; }
        IYouTubeDLCommandOptions IYouTubeDLCommand.Options => this.Options;
        public YouTubeDLCommandType Type => YouTubeDLCommandType.Download;

        public string Serialize()
        {
            return $"{this.URL} {this.Options.Serialize()}";
        }

        #region ValueObject

        protected override IEnumerable<object> GetValueIngredients()
        {
            yield return this.URL;
            yield return this.Options;
        }

        #endregion
    }
}