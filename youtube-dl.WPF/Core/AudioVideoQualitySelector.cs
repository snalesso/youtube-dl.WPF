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
    public class AudioVideoQualitySelector : IYouTubeDLQualitySelector
    {
        public AudioVideoQualitySelector(
            YouTubeDLQuality audioQuality,
            YouTubeDLQuality videoQuality,
            IReadOnlyDictionary<NumericField, NumericFilter> audioFilters_Numeric = null,
            IReadOnlyDictionary<LiteralField, LiteralFilter> audioFilters_Literal = null,
            IReadOnlyDictionary<NumericField, NumericFilter> videoFilters_Numeric = null,
            IReadOnlyDictionary<LiteralField, LiteralFilter> videoFilters_Literal = null)
        {
            this.AudioQuality = audioQuality;
            this.AudioFilters_Numeric = audioFilters_Numeric?.ToImmutableDictionary();
            this.AudioFilters_Literal = audioFilters_Literal?.ToImmutableDictionary();
            this.VideoQuality = videoQuality;
            this.VideoFilters_Numeric = videoFilters_Numeric?.ToImmutableDictionary();
            this.VideoFilters_Literal = videoFilters_Literal?.ToImmutableDictionary();
        }

        public YouTubeDLQuality AudioQuality { get; }
        public IReadOnlyDictionary<NumericField, NumericFilter> AudioFilters_Numeric { get; }
        public IReadOnlyDictionary<LiteralField, LiteralFilter> AudioFilters_Literal { get; }

        public YouTubeDLQuality VideoQuality { get; }
        public IReadOnlyDictionary<NumericField, NumericFilter> VideoFilters_Numeric { get; }
        public IReadOnlyDictionary<LiteralField, LiteralFilter> VideoFilters_Literal { get; }
    }
}