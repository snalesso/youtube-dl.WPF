using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace youtube_dl.WPF.Core
{
    [Obsolete]
    public interface IYouTubeDLCommand
    {
        [Obsolete]
        YouTubeDLCommandType Type { get; }
        IYouTubeDLCommandOptions Options { get; }

        [Obsolete("Move to mixins/external serializer?")]
        string Serialize();
    }

    public interface IYouTubeDLCommandOptions
    {
        [Obsolete("Move to mixins/external serializer?")]
        string Serialize();
    }
}