using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace youtube_dl.WPF.Core.Models
{
    public class DownloadQueueEntry
    {
        public DownloadQueueEntry(string url)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));
            if (url == "") throw new ArgumentException(nameof(url));

            this.Url = url;
        }

        public string Url { get; }
    }
}