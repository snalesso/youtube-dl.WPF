using System;

namespace youtube_dl.WPF.Core
{
    public abstract class Filter<TField, TComparison>
        where TField : Enum
        where TComparison : Enum
    {
        public Filter(TField field, TComparison comparison, string value, bool isNegated = false, bool areUnknownIncluded = false)
        {
            this.Field = field;
            this.Comparison = comparison;
            this.Value = value ?? throw new ArgumentNullException(nameof(value));
            this.IsNegated = isNegated;
            this.AreUnknownIncluded = areUnknownIncluded;
        }

        public TField Field { get; }
        public TComparison Comparison { get; }
        public string Value { get; }
        public bool IsNegated { get; }
        public bool AreUnknownIncluded { get; }
    }
}