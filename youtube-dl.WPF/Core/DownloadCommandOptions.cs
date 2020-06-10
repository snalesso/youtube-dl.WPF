using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace youtube_dl.WPF.Core.Models
{
    public class DownloadCommandOptions : ValueObject<DownloadCommandOptions>
    {
        public DownloadCommandOptions(DownloadMode downloadMode)
        {
            this.DownloadMode = downloadMode;
        }

        public virtual DownloadMode DownloadMode { get; }

        public string OutputFolderPath { get; } = "E:\\Downloads\\youtube-dl";

        public AudioFormat AudioFormat { get; }

        public bool PreferFFmpeg { get; } = true;

        public string FFmpegLocation { get; } = ".\\ffmpeg-latest-win64-static\\bin\\ffmpeg.exe";

        public string VideoFormat { get; }

        public uint DownloadSpeedLimit_Kbps { get; } = 0;

        public override string ToString()
        {
            // TODO: use a dictionary?
            var argsList = new List<string>
            {
                $"-o \"{Path.GetFullPath("E:\\Downloads\\youtube-dl\\video\\%(title)s -- %(uploader)s -- %(id)s.%(ext)s")}\""
            };

            switch (this.DownloadMode)
            {
                case DownloadMode.AudioOnly:
                    argsList.Add("-x");
                    argsList.Add($"--audio-format {Enum.GetName(typeof(AudioFormat), this.AudioFormat).ToLower()}");
                    break;

                case DownloadMode.AudioVideo:
                    argsList.Add($"-f bestvideo[height<=?{(int)(VideoResolutionHeight._1080p)}]+bestaudio/best");
                    break;

                default:
                    throw new NotSupportedException();
            }

            if (this.PreferFFmpeg)
            {
                argsList.Add("--prefer-ffmpeg");
            }

            var ff = Path.GetFullPath(this.FFmpegLocation);
            argsList.Add($"--ffmpeg-location \"{ff}\"");

            var paramsString = string.Join(" ", argsList);

            return paramsString;
        }

        protected override IEnumerable<object> GetValueIngredients()
        {
            yield return this.DownloadMode;
        }
    }
}
