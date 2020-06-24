using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace youtube_dl.WPF.Core
{
    [Flags]
    public enum DownloadMode
    {
        [Description(nameof(AudioOnly))]
        AudioOnly = 1,
        [Description(nameof(VideoOnly))]
        VideoOnly,
        [Description(nameof(AudioVideo))]
        AudioVideo = AudioOnly | VideoOnly
    }
}
