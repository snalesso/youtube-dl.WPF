using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace youtube_dl.WPF.Core
{
    public class AppSettings
    {
        public string YouTubeDLExePath { get; set; } = @".\youtube-dl\youtube-dl.exe";
        public string YouTubeDLVideoDownloadsFolderPath { get; set; } = @".\youtube-dl\video";
        public string YouTubeDLAudioDownloadsFolderPath { get; set; } = @".\youtube-dl\audio";
        public string FFmpegFolderPath { get; set; } = @".\ffmpeg-latest-win64-static\bin\ffmpeg.exe";
    }
}
