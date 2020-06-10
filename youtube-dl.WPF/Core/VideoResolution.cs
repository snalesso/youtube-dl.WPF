namespace youtube_dl.WPF.Core.Models
{
    public struct VideoResolution
    {
        #region standard resolutions

        public static VideoResolution HD => new VideoResolution(VideoResolutionWidth._1280p, VideoResolutionHeight._720p);
        public static VideoResolution FHD => new VideoResolution(VideoResolutionWidth._1920p, VideoResolutionHeight._1080p);
        public static VideoResolution QHD => new VideoResolution(VideoResolutionWidth._2560p, VideoResolutionHeight._1440p);
        public static VideoResolution _4K => new VideoResolution(VideoResolutionWidth._3840p, VideoResolutionHeight._2160p);
        public static VideoResolution _8K => new VideoResolution(VideoResolutionWidth._7680p, VideoResolutionHeight._4320p);

        #endregion

        public VideoResolution(
            VideoResolutionWidth width,
            VideoResolutionHeight height)
        {
            this.Width = width;
            this.Height = height;
        }

        public VideoResolutionWidth Width{ get; }
        public VideoResolutionHeight Height { get; }
    }
}