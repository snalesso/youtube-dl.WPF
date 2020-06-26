using System.ComponentModel;

namespace youtube_dl.WPF.Core
{
    public enum LiteralField
    {
        [Description("File extension")]
        ext,
        [Description("Name of the audio codec in use")]
        acodec,
        [Description("Name of the video codec in use")]
        vcodec,
        [Description("Name of the container format")]
        container,
        [Description("The protocol that will be used for the actual download, lower-case (http, https, rtsp, rtmp, rtmpe, mms, f4m, ism, http_dash_segments, m3u8, or m3u8_native)")]
        protocol,
        [Description("A short description of the format")]
        format_id
    }
}