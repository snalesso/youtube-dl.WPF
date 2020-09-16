using System;
using System.Collections.Generic;
using System.Linq;

namespace youtube_dl.WPF.Core
{
    public static class QualitySelectorMixins
    {
        public static string Serialize(this NumericField numericField)
        {
            return numericField.ToString().ToLowerInvariant();
        }

        public static string Serialize(this LiteralField literalField)
        {
            return literalField.ToString().ToLowerInvariant();
        }

        public static string SerializeName(this Enum @enum)
        {
            return @enum.ToString().ToLowerInvariant();
        }

        public static string SerializeName<T>(this Nullable<T> x)
            where T : struct
        {
            var k = x.ToString().ToLowerInvariant();
            return k;
        }

        public static string SerializeValue(this YouTubeDLQuality quality)
        {
            return quality.ToString();
        }

        //public static string Serialize<TField, TComparison>(this Filter<TField, TComparison> filter)
        //    where TField : Enum
        //    where TComparison : Enum
        //{
        //    if (filter == null)
        //        throw new ArgumentNullException(nameof(filter));

        //    //string filterString =
        //    return "["
        //        + filter.Field.Serialize()
        //        + (filter.IsNegated ? "!" : string.Empty)
        //        + filter.Comparison.Serialize()
        //        + filter.Value
        //        + "]";

        //    //return $"[{filterString}]";
        //}

        public static string Serialize(this NumericFilter filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            //string filterString =
            return "["
                + filter.Field.Serialize()
                + (filter.IsNegated ? "!" : string.Empty)
                + filter.Comparison.Serialize()
                + filter.Value
                + "]";

            //return $"[{filterString}]";
        }

        public static string Serialize(this LiteralFilter filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            //string filterString =
            return "["
                + filter.Field.Serialize()
                + (filter.IsNegated ? "!" : string.Empty)
                + filter.Comparison.Serialize()
                + filter.Value
                + "]";

            //return $"[{filterString}]";
        }

        public static string Serialize<TField, TComparison>(this IEnumerable<Filter<TField, TComparison>> filters)
            where TField : Enum
            where TComparison : Enum
        {
            if (filters == null)
                throw new ArgumentNullException(nameof(filters));

            return string.Join(string.Empty, filters.Select(f =>
            {
                switch (f)
                {
                    case NumericFilter nf:
                        return nf.Serialize();
                    case LiteralFilter lf:
                        return lf.Serialize();
                    default:
                        throw new NotSupportedException();
                }
            }));
        }

        public static string Serialize(this LiteralComparison literalComparison)
        {
            switch (literalComparison)
            {
                case LiteralComparison.EqualsTo:
                    return "=";

                case LiteralComparison.Contains:
                    return "*=";

                case LiteralComparison.StartsWith:
                    return "^=";

                case LiteralComparison.EndsWith:
                    return "$=";

                default:
                    throw new NotSupportedException($"{nameof(LiteralComparison)}.{literalComparison} is not supported by {nameof(QualitySelectorMixins)}.{nameof(QualitySelectorMixins.Serialize)}");
            }
        }

        public static string Serialize(this NumericComparisons numericComparison)
        {
            switch (numericComparison)
            {
                case NumericComparisons.EqualsTo:
                    return "=";

                case NumericComparisons.LessThan:
                    return "<";

                case NumericComparisons.GreaterThan:
                    return ">";

                default:
                    throw new NotSupportedException($"{nameof(NumericComparisons)}.{numericComparison} is not supported by {nameof(QualitySelectorMixins)}.{nameof(QualitySelectorMixins.Serialize)}");
            }
        }

        public static string Serialize(this IYouTubeDLQualitySelector selector, DownloadMode? downloadMode = null)
        {
            if (selector == null)
                return null;

            switch (selector)
            {
                case VideoAudioQualitySelector avqs:

                    string videoFilter = null;
                    string audioFilter = null;

                    if (!downloadMode.HasValue || downloadMode.Value.HasFlag(DownloadMode.VideoOnly))
                    {
                        var vq = avqs.VideoQuality.HasValue ? avqs.VideoQuality.Value.SerializeName() : null;
                        var vlf = avqs.VideoFilters_Literal?.Values.Serialize();
                        var vnf = avqs.VideoFilters_Numeric?.Values.Serialize();

                        videoFilter = $"{vq}{(vq != null ? "video" : string.Empty)}{vnf}{vlf}";
                    }

                    if (!downloadMode.HasValue || downloadMode.Value.HasFlag(DownloadMode.AudioOnly))
                    {
                        var aq = avqs.AudioQuality.SerializeName();
                        var alf = avqs.AudioFilters_Literal?.Values.Serialize();
                        var anf = avqs.AudioFilters_Numeric?.Values.Serialize();

                        audioFilter = $"{aq}{(aq != null ? "audio" : string.Empty)}{anf}{alf}";
                    }

                    if (!string.IsNullOrWhiteSpace(videoFilter) && !string.IsNullOrWhiteSpace(audioFilter))
                        return videoFilter + "+" + audioFilter;

                    else if (!string.IsNullOrWhiteSpace(videoFilter))
                        return videoFilter;

                    else if (!string.IsNullOrWhiteSpace(audioFilter))
                        return audioFilter;

                    else
                        return null;

                case GenericQualitySelector gqs:

                    var q = gqs.Quality.SerializeName();
                    var nf = gqs.Filters_Numeric?.Values.Serialize();
                    var lf = gqs.Filters_Literal?.Values.Serialize();
                    return $"{q}{nf}{lf}";

                default:
                    throw new NotSupportedException();
            }
        }

        public static string Serialize(this IEnumerable<IYouTubeDLQualitySelector> selectors, DownloadMode? downloadMode = null)
        {
            if (selectors == null || !selectors.Any())
                return null;

            return string.Join("/", selectors.Select(s => s.Serialize(downloadMode)).RemoveNullOrWhitespaces());
        }
    }
}