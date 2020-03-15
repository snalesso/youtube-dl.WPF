using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace youtube_dl.WPF.Core.Models
{
    // TODO: make this a value object so 2 identical URIs with the same download options are evaluated as duplicates
    public class DownloadOptions : ValueObject<DownloadOptions>
    {
        public DownloadOptions(DownloadMode downloadMode)
        {
            this.DownloadMode = downloadMode;
        }

        public virtual DownloadMode DownloadMode { get; }

        protected override IEnumerable<object> GetValueIngredients()
        {
            yield return this.DownloadMode;
        }
    }
}
