using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace youtube_dl.WPF.Core
{
    //public class NumericAudioFilter
    //{
    //    public NumericComparisons Comparison { get; }
    //    public int Value { get; }
    //    public bool IsNegated { get; } = false;
    //}

    // TODO: can be redesigned to group better and separate audio/video fields
    public class VideoAudioQualitySelector : IYouTubeDLQualitySelector
    {
        public VideoAudioQualitySelector(
            YouTubeDLQuality? videoQuality = null,
            YouTubeDLQuality? audioQuality = null,
            IReadOnlyDictionary<NumericField, NumericFilter> videoFilters_Numeric = null,
            IReadOnlyDictionary<LiteralField, LiteralFilter> videoFilters_Literal = null,
            IReadOnlyDictionary<NumericField, NumericFilter> audioFilters_Numeric = null,
            IReadOnlyDictionary<LiteralField, LiteralFilter> audioFilters_Literal = null)
        {
            if (videoQuality == null
                && audioQuality == null
                && (videoFilters_Numeric == null || videoFilters_Numeric.Count <= 0)
                && (videoFilters_Literal == null || videoFilters_Literal.Count <= 0)
                && (audioFilters_Numeric == null || audioFilters_Numeric.Count <= 0)
                && (audioFilters_Literal == null || audioFilters_Literal.Count <= 0))
                throw new Exception("At least one parameter must be != null");

            this.VideoQuality = videoQuality;
            this.VideoFilters_Numeric = videoFilters_Numeric?.ToImmutableDictionary();
            this.VideoFilters_Literal = videoFilters_Literal?.ToImmutableDictionary();
            this.AudioQuality = audioQuality;
            this.AudioFilters_Numeric = audioFilters_Numeric?.ToImmutableDictionary();
            this.AudioFilters_Literal = audioFilters_Literal?.ToImmutableDictionary();
        }

        public YouTubeDLQuality? VideoQuality { get; }
        public IReadOnlyDictionary<NumericField, NumericFilter> VideoFilters_Numeric { get; }
        public IReadOnlyDictionary<LiteralField, LiteralFilter> VideoFilters_Literal { get; }

        public YouTubeDLQuality? AudioQuality { get; }
        public IReadOnlyDictionary<NumericField, NumericFilter> AudioFilters_Numeric { get; }
        public IReadOnlyDictionary<LiteralField, LiteralFilter> AudioFilters_Literal { get; }
    }
}