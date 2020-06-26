using System;
using System.Collections.Generic;

namespace youtube_dl.WPF.Core
{
    [Obsolete("This class groups temporary solutions")]
    public static class Workarounds
    {
        public static IEnumerable<IYouTubeDLQualitySelector> DefaultYouTubeDLQualitySelectors =>
            new IYouTubeDLQualitySelector[] {
                new AudioVideoQualitySelector(
                    YouTubeDLQuality.Best,
                    YouTubeDLQuality.Best,
                    videoFilters_Numeric: new Dictionary<NumericField, NumericFilter>
                    {
                        { NumericField.height, new NumericFilter(NumericField.height, NumericComparisons.EqualsTo, ((int)VideoResolution.FHD.Height).ToString(), areUnknownIncluded: true) },
                    }),
                new GenericQualitySelector(
                    YouTubeDLQuality.Best,
                    filters_Numeric: new Dictionary<NumericField, NumericFilter>
                    {
                        { NumericField.height, new NumericFilter(NumericField.height, NumericComparisons.EqualsTo, ((int)VideoResolution.FHD.Height).ToString(), areUnknownIncluded: true) },
                    }),
                new GenericQualitySelector(YouTubeDLQuality.Best)
            };
    }
}
