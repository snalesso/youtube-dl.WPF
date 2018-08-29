using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using youtube_dl.WPF.Core.Services;

namespace youtube_dl.WPF.Presentation.ViewModels.DesignTime
{
    internal class DesignTimeUtilsViewModel : UtilsViewModel
    {
        public DesignTimeUtilsViewModel() : base(DummyYouTubeDLService.Instance)
        {
        }
    }
}