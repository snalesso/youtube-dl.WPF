using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace youtube_dl.WPF.Core
{
    public class GenericQualitySelector : IYouTubeDLQualitySelector
    {
        public GenericQualitySelector(
            YouTubeDLQuality quality,
            IReadOnlyDictionary<NumericField, NumericFilter> filters_Numeric = null,
            IReadOnlyDictionary<LiteralField, LiteralFilter> filters_Literal = null)
        {
            this.Quality = quality;
            this.Filters_Numeric = filters_Numeric?.ToImmutableDictionary();
            this.Filters_Literal = filters_Literal?.ToImmutableDictionary();
        }

        public YouTubeDLQuality Quality { get; }
        public IReadOnlyDictionary<NumericField, NumericFilter> Filters_Numeric { get; }
        public IReadOnlyDictionary<LiteralField, LiteralFilter> Filters_Literal { get; }

        public string Serialize()
        {
            throw new NotImplementedException();
        }
    }
}