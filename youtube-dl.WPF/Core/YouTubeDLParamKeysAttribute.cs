using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace youtube_dl.WPF.Core
{
    //public class YouTubeDLCommand : ValueObject<YouTubeDLCommand>, IYouTubeDLCommand
    //{
    //    public YouTubeDLCommand(YouTubeDLCommandType type)
    //    {
    //        this.Type = type ;
    //    }

    //    public YouTubeDLCommandType Type { get; }

    //    protected override IEnumerable<object> GetValueIngredients()
    //    {
    //        yield return this.Type;
    //    }
    //}

    public class YouTubeDLParamKeysAttribute: Attribute
    {
        public YouTubeDLParamKeysAttribute(IEnumerable<string> paramKeys)
        {
            if (paramKeys == null)
                throw new ArgumentNullException(nameof(paramKeys));

            paramKeys = paramKeys.RemoveNullOrWhitespaces();

            this.ParamKeys = paramKeys.Any() ? paramKeys.ToImmutableArray() : throw new ArgumentException($"{nameof(paramKeys)} is empty");
        }
        public YouTubeDLParamKeysAttribute(params string[] paramKeys) : this(paramKeys.AsEnumerable()) { }

        public IReadOnlyList< string> ParamKeys { get; }
    }
}