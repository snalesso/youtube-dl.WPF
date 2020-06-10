using System.Collections.Generic;

namespace youtube_dl.WPF.Core.Models
{
    public class DownloadCommandOptionsPreset : DownloadCommandOptions
    {
        public DownloadCommandOptionsPreset(DownloadMode downloadMode) : base(downloadMode)
        {
        }

        public string Name { get; }

        protected override IEnumerable<object> GetValueIngredients()
        {
            yield return this.Name;

            foreach (var i in base.GetValueIngredients())
            {
                yield return i;
            }
        }
    }
}
