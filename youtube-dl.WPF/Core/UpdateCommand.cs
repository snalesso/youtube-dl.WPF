using System.Collections.Generic;

namespace youtube_dl.WPF.Core
{
    public class UpdateCommand : ValueObject<UpdateCommand>, IYouTubeDLCommand
    {
        public YouTubeDLCommandType Type => YouTubeDLCommandType.Update;

        protected override IEnumerable<object> GetValueIngredients()
        {
            yield return this.Type;
        }

        public string Serialize() => "-U";

        public static UpdateCommand Default { get => new UpdateCommand(); }

        public IYouTubeDLCommandOptions Options => throw new System.NotSupportedException();
    }
}