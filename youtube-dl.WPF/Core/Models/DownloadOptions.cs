using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace youtube_dl.WPF.Core.Models
{
    public abstract class DownloadOptions
    {
        public abstract DownloadMode DownloadMode { get; }
    }

    public class AudioDownloadOptions : DownloadOptions
    {
        public override DownloadMode DownloadMode => DownloadMode.AudioOnly;
    }
}
