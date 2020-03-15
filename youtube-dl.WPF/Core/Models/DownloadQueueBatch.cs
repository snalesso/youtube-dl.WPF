using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace youtube_dl.WPF.Core.Models
{
    public class DownloadQueueBatch
    {
        public DownloadQueueBatch(IEnumerable<DownloadQueueEntry> items)
        {
            this.Items = items?.ToImmutableList() ?? ImmutableList.Create<DownloadQueueEntry>();
        }

        public IReadOnlyList<DownloadQueueEntry> Items { get; }
    }
}
