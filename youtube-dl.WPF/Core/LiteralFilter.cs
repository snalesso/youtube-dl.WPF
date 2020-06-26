namespace youtube_dl.WPF.Core
{
    public class LiteralFilter : Filter<LiteralField, LiteralComparison>
    {
        public LiteralFilter(LiteralField field, LiteralComparison comparison, string value, bool isNegated = false, bool areUnknownIncluded = false)
            : base(field, comparison, value, isNegated, areUnknownIncluded)
        {
        }
    }
}