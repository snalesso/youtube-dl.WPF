using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace youtube_dl.WPF.Presentation.ViewModels.DesignTime
{
    internal class DesignTimePostProcessingViewModel : PostProcessingViewModel
    {
        public DesignTimePostProcessingViewModel() : base(Observable.Return(Core.DownloadMode.AudioVideo))
        {
        }
    }
}
