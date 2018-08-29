using System;

namespace youtube_dl.WPF.Presentation.MarkupExtensions
{
    public partial class EnumExtension
    {
        public class EnumMember
        {
            public EnumMember(object value, string description = null)
            {
                this.Value = value ?? throw new ArgumentNullException(nameof(value));
                this.Description = description;
            }

            public object Value { get; }
            public string Description { get; }
        }
    }
}