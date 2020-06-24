using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MaterialDesignThemes.Wpf;

namespace youtube_dl.WPF.Core
{
    public interface IYouTubeDLQualitySelector
    {
        string Serialize();
    }

    [Flags]
    public enum NumericComparisons
    {
        EqualsTo = 1,
        LessThan = 2,
        GreaterThan = 4
    }

    public enum LiteralComparison
    {
        EqualsTo,
        StartsWith,
        EndsWith,
        Contains,
    }

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

    public interface IFilter<TField, TComparison>
        where TField : Enum
        where TComparison : Enum
    {
        TField Field { get; }
        TComparison Comparison { get; }
        string Value { get; }
        bool IsNegated { get; }
    }

    /* TODO: consider redesign to separate audio/video fields in dedicated enums (with some fields in common repeated in each enum) 
     * and strucutre each filter-able property as a class property, each one with its dedicated type, 
     * so we can pass precise filter values */
    public class NumericFilter : IFilter<NumericField, NumericComparisons>
    {
        public NumericFilter(NumericField field, NumericComparisons comparison, string value, bool isNegated = false, bool areUnknownIncluded = false)
        {
            this.Field = field;
            this.Comparison = comparison;
            this.Value = value ?? throw new ArgumentNullException(nameof(value));
            this.IsNegated = isNegated;
            this.AreUnknownIncluded = areUnknownIncluded;
        }

        public NumericField Field { get; }
        public NumericComparisons Comparison { get; }
        public string Value { get; }
        public bool IsNegated { get; }
        public bool AreUnknownIncluded { get; }
    }

    public class LiteralFilter : IFilter<LiteralField, LiteralComparison>
    {
        public LiteralFilter(LiteralField field, LiteralComparison comparison, string value, bool isNegated = false)
        {
            this.Field = field;
            this.Comparison = comparison;
            this.Value = value ?? throw new ArgumentNullException(nameof(value));
            this.IsNegated = isNegated;
        }

        public LiteralField Field { get; }
        public LiteralComparison Comparison { get; }
        public string Value { get; }
        public bool IsNegated { get; } = false;
        //public bool IsNegated { get; } = false;
    }

    public class Filter<TType>
    {
        public TType Value { get; }
    }

    public class VideoFilters
    {

    }

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

        public string Serialize()
        {
            throw new NotImplementedException();
        }
    }

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

    public static class QualitySelectorMixins
    {
        public static string Serialize(this NumericField numericField)
        {
            return numericField.ToString().ToLowerInvariant();
        }

        public static string SerializeName(this Enum @enum)
        {
            return @enum.ToString().ToLowerInvariant();
        }

        public static string Serialize(this LiteralField literalField)
        {
            return literalField.ToString().ToLowerInvariant();
        }

        public static string Serialize(this YouTubeDLQuality quality)
        {
            return quality.ToString().ToLowerInvariant();
        }

        public static string Serialize<TField, TComparison>(this IFilter<TField, TComparison> filter)
            where TField : Enum
            where TComparison : Enum
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            //string filterString =
            return "["
                + filter.Field.SerializeName()
                + (filter.IsNegated ? "!" : string.Empty)
                + filter.Comparison.SerializeName()
                + filter.Value
                + "]";

            //return $"[{filterString}]";
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

        public static string Serialize(this IYouTubeDLQualitySelector selector)
        {
            if (selector == null)
                return null;

            switch (selector)
            {
                case AudioVideoQualitySelector avqs:
                    var vq = avqs.VideoQuality.Serialize();
                    var vlf = avqs.VideoFilters_Literal.Select(f => f.Value.Serialize());
                    var vnf = avqs.VideoFilters_Numeric.Select(f => f.Value.Serialize());

                    var aq = avqs.AudioQuality.Serialize();
                    var alf = avqs.AudioFilters_Literal.Select(f => f.Value.Serialize());
                    var anf = avqs.AudioFilters_Numeric.Select(f => f.Value.Serialize());

                    return
                        $"{vq}{string.Join(string.Empty, vlf)}" +
                        $"+" +
                        $"{aq}{string.Join(string.Empty, alf)}";


                case GenericQualitySelector gqs:

                    return "";

                default:
                    return null;
            }
        }

        public static string Serialize(this IEnumerable<IYouTubeDLQualitySelector> selectors)
        {
            if (selectors == null || !selectors.Any())
                return null;

            return string.Join("/", selectors.Select(s => s.Serialize()));
        }
    }
}