using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace youtube_dl.WPF.Core
{
    public interface IYouTubeDLCommand
    {
        YouTubeDLCommandType Type { get; }
        IYouTubeDLCommandOptions Options { get; }
    }

    public interface IYouTubeDLCommandOptions
    {
        string Serialize();
    }
}