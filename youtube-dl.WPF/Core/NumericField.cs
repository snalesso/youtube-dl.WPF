using System.ComponentModel;

namespace youtube_dl.WPF.Core
{
    public enum NumericField
    {
        [Description("The number of bytes, if known in advance")]
        filesize,
        [Description("Width of the video, if known")]
        width,
        [Description("Height of the video, if known")]
        height,
        [Description("Average bitrate of audio and video in KBit/s")]
        tbr,
        [Description("Average audio bitrate in KBit/s")]
        abr,
        [Description("Average video bitrate in KBit/s")]
        vbr,
        [Description("Audio sampling rate in Hertz")]
        asr,
        [Description("Frame rate")]
        fps
    }
}