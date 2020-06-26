namespace youtube_dl.WPF.Core
{
    /* TODO: consider redesign to separate audio/video fields in dedicated enums (with some fields in common repeated in each enum) 
     * and strucutre each filter-able property as a class property, each one with its dedicated type, 
     * so we can pass precise filter values */
    public class NumericFilter : Filter<NumericField, NumericComparisons>
    {
        public NumericFilter(NumericField field, NumericComparisons comparison, string value, bool isNegated = false, bool areUnknownIncluded = false)
            : base(field, comparison, value, isNegated, areUnknownIncluded)
        {
        }
    }
}