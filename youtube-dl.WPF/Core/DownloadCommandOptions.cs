﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace youtube_dl.WPF.Core
{
    // TODO: add option to convert audio if downloaded audio's codec is lossless
    public class DownloadCommandOptions : ValueObject<DownloadCommandOptions>, IYouTubeDLCommandOptions
    {
        //[Obsolete]
        //public DownloadCommandOptions(DownloadMode downloadMode)
        //{
        //    this.DownloadMode = downloadMode;
        //}

        //[Obsolete]
        //public DownloadCommandOptions(bool isAudioKept, bool isVideoKept)
        //{
        //    this.IsAudioKept = isAudioKept;
        //    this.IsVideoKept = isVideoKept;
        //}

        //public DownloadCommandOptions(AudioFormat audioFormat, VideoFormat videoFormat)
        //{
        //    if (audioFormat == null && videoFormat == null)
        //        throw new ArgumentException($"At least one between {nameof(this.AudioFormat)} and {nameof(this.VideoFormat)} must be defined");

        //    this.AudioFormat = audioFormat;
        //    this.VideoFormat = videoFormat;
        //}

        public DownloadCommandOptions(
            IEnumerable<IYouTubeDLQualitySelector> qualitySelectors = null,
            AudioCodec? outputAudioCodec = null,
            VideoContainerFormat? videoContainerFormat = null,
            string outputFolderPath = null,
            string outputFileNameFormat =null)
        {
            this.QualitySelectors = qualitySelectors?.ToImmutableArray();
            this.OutputAudioCodec = outputAudioCodec;
            this.VideoContainerFormat = videoContainerFormat;
            this.OutputFolderPath = outputFolderPath ?? Path.Combine("E" + Path.VolumeSeparatorChar + Path.DirectorySeparatorChar, "Downloads", YouTubeDL.OfficialName);
            this.OutputFileNameFormat = outputFileNameFormat ?? "%(title)s -- %(uploader)s -- %(id)s.%(ext)s";
        }

        // FORMAT
        [YouTubeDLParamKeys("-f", "--format")]
        public IReadOnlyList<IYouTubeDLQualitySelector> QualitySelectors { get; }

        // FILESYSTEM
        public string OutputFolderPath { get; } 
        public string OutputFileNameFormat { get; }  

        // DOWNLOAD
        [YouTubeDLParamKeys("-r")]
        // TODO: make nullable?
        public uint DownloadSpeedLimit_Bps { get; } = 0;

        // POST PROCESSING
        public AudioCodec? OutputAudioCodec { get; }
        public VideoContainerFormat? VideoContainerFormat { get; }
        //public VideoFormat VideoFormat { get; }
        [YouTubeDLParamKeys("--embed-subs")]
        public bool EmbedSubs { get; } = false;
        [YouTubeDLParamKeys("--embed-thumbnail")]
        public bool EmbedThumbnail { get; } = false; // TODO: true is unsupported, triggers: "AtomicParsley was not found. Please install"
        [YouTubeDLParamKeys("--prefer-ffmpeg")]
        public bool PreferFFmpeg { get; } = true;
        [YouTubeDLParamKeys("--ffmpeg-location")]
        public string FFmpegLocation { get; } = Path.Combine(".", "ffmpeg-latest-win64-static", "bin", "ffmpeg.exe");

        // derived props
        public DownloadMode DownloadMode
        {
            get
            {
                if (this.OutputAudioCodec.HasValue & this.VideoContainerFormat.HasValue)
                {
                    return DownloadMode.AudioVideo;
                }
                if (!this.VideoContainerFormat.HasValue)
                {
                    return DownloadMode.AudioOnly;
                }
                return DownloadMode.VideoOnly;
            }
        }

        // TODO: cache
        public string Serialize()
        {
            // TODO: use a dictionary?
            var argsList = new List<string>
            {
                // FILESYSTEM

                // output location and file name format
                $"-o \"{Path.GetFullPath(Path.Combine(this.OutputFolderPath, this.OutputFileNameFormat))}\"",

                // POST PORCESSING

                // FFmpeg location for params that need it
                $"--ffmpeg-location \"{Path.GetFullPath(this.FFmpegLocation)}\"",

                // VERBOSITY/SIMULATION
                //"--print-json"
            };

            // POST PROCESSING

            argsList.AddIf(this.PreferFFmpeg, "--prefer-ffmpeg");
            argsList.AddIf(this.EmbedThumbnail, "--embed-thumbnail");
            argsList.AddIf(this.EmbedSubs, "--embed-subs");

            argsList.AddIf(this.QualitySelectors != null, $"--format \"{this.QualitySelectors.Serialize(this.DownloadMode)}\"");

            switch (this.DownloadMode)
            {
                case DownloadMode.AudioOnly:
                    //argsList.Add("--format bestaudio/best");
                    argsList.Add("--extract-audio");
                    argsList.AddIf(this.OutputAudioCodec.HasValue, $"--audio-format {this.OutputAudioCodec.ToString().ToLowerInvariant()}");
                    argsList.AddIf(this.OutputAudioCodec.HasValue, "--audio-quality 0");
                    break;

                case DownloadMode.AudioVideo:
                    //string heightFilter = $"[{nameof(VideoResolution.Height).ToLowerInvariant()}=?{this.VideoFormat.Resolution.Height}]";
                    //argsList.Add($"--format \"" +
                    //    // take worst video resolution BUT of at least the required resolution
                    //    // take the closest to selected resolution (approaching from the better side) & best audio available
                    //    $"bestvideo{heightFilter}+bestaudio" +
                    //    //$"/best[{nameof(VideoResolution.Width).ToLowerInvariant()}<=?{this.VideoFormat.Resolution.Height}]" +
                    //    // take best available of both video & audio
                    //    $"/best{heightFilter}" +
                    //    $"\"");

                    // this is a good starting point
                    // bestvideo[height=?1080]+bestaudio/bestvideo[height<=?1440]+bestaudio/best[height=?1080]/best

                    argsList.AddIf(this.OutputAudioCodec.HasValue, $"--recode-video {this.OutputAudioCodec.ToString().ToLowerInvariant()}");
                    break;

                case DownloadMode.VideoOnly:
                    throw new NotImplementedException();
            }

            var optionsString = string.Join(" ", argsList);

            return optionsString;
        }

        public override string ToString() => this.Serialize();

        protected override IEnumerable<object> GetValueIngredients()
        {
            yield return this.DownloadMode;
        }
    }
}