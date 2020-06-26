using System;

namespace youtube_dl.WPF.Core
{
    [Flags]
    public enum NumericComparisons
    {
        EqualsTo = 1,
        LessThan = 2,
        GreaterThan = 4
    }
}