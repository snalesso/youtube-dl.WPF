using System;

namespace youtube_dl.WPF.Core
{
    public class YouTubeDLInstanceOutputData
    {
        public YouTubeDLInstanceOutputData(string outputString)
        {
            this.OriginalString = outputString;

            var x = outputString.Split('\t');
        }

        public string OriginalString { get; set; }

        public float DownloadedPercent { get; set; }
        public string OperationName { get; set; }
        public uint FileSize_Bytes { get; set; }
        public uint DownloadSpeed_Bps { get; set; }
        public TimeSpan ETA { get; set; }
    }
}