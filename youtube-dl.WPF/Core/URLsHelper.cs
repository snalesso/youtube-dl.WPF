using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace youtube_dl.WPF.Core
{
    public static class URLsHelper
    {
        public static IReadOnlyList<string> ParseUrlsList(string rawUrlsString)
        {
            if (rawUrlsString == null)
            {
                throw new ArgumentNullException(nameof(rawUrlsString));
            }

            return rawUrlsString
                .Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .TrimAll()
                .ToImmutableArray();
        }

        public static string JoinUrlsList(IEnumerable<string> urls)
        {
            if (urls == null)
                throw new ArgumentNullException(nameof(urls));

            return string.Join(
                Environment.NewLine,
                urls
                .TrimAll()
                .RemoveNullOrWhitespaces());
        }
    }
}
