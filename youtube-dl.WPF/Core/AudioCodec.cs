using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace youtube_dl.WPF.Core
{
    public enum AudioCodec
    {
        Best,

        MP3,
        AAC,
        FLAC,
        M4A,
        Opus,
        Vorbis,
        WAV
    }

    public enum YouTubeDLQuality
    {
        Best,
        Worst,
        //Best_AudioOnly,
        //Best_VideoOnly,
        //Worst_AudioOnly,
        //Worst_VideoOnly,
    }

    public enum VideoContainerFormat
    {
        MKV,
        MP4,
        WEBM,
        FLV,
        OGG,
        AVI
    }

    public class AudioFormat
    {
        public AudioCodec? Codec { get; }
        public AudioBitrate Bitrate { get; }
    }

    public class VideoFormat
    {
        public YouTubeDLQuality Quality { get; } = YouTubeDLQuality.Best;
        //public VideoCodec Codec { get; } = VideoCodec.MP4;
        public VideoResolution Resolution { get; } = VideoResolution.FHD;
        public VideoContainerFormat? Container { get; } = null; // VideoContainer.MP4;
    }

    public class AudioBitrate
    {
        public BitrateMode Mode { get; }
        public uint Value { get; }
    }

    public enum BitrateMode
    {
        Fixed,
        Variable
    }

    public enum VBRLevels
    {
        _0,
        _1,
        _2,
        _3,
        _4,
        _5,
        _6,
        _7,
        _8,
        _9
    }
}
