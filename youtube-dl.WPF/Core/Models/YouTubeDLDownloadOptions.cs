using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace youtube_dl.WPF.Core.Models
{
    public class YouTubeDLDownloadOptions
    {
        [Description("-o")]
        public string OutputFolderPath { get; set; }

        [Description("-x")]
        public bool ExtractAudio { get; set; }

        [Description("--audio-format")]
        public AudioFormat AudioFormat { get; set; }

        [Description("--prefer-ffmpeg")]
        public bool PreferFFmpeg { get; set; } = true;

        [Description("--ffmpeg-location")]
        public string FFmpegLocation { get; set; }

        [Description("-f")]
        public string VideoFormat { get; set; }

        public string BuildParamsString()
        {
            var argsList = new List<string>();

            argsList.Add($"-o \"{Path.GetFullPath(this.OutputFolderPath)}\"");

            if (this.ExtractAudio)
            {
                argsList.Add("-x");
                argsList.Add($"--audio-format {Enum.GetName(typeof(AudioFormat), this.AudioFormat).ToLower()}");
            }
            else
            {
                argsList.Add($"-f {this.VideoFormat}");
            }

            if (this.PreferFFmpeg)
                argsList.Add("--prefer-ffmpeg");

            var ff = Path.GetFullPath(this.FFmpegLocation);
            argsList.Add($"--ffmpeg-location \"{ff}\"");

            var paramsString = string.Join(" ", argsList);

            return paramsString;
        }
    }
}