using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using youtube_dl.WPF.Core;

namespace youtube_dl.WPF.Presentation.ViewModels.DesignTime
{
    internal class DesignTimeYouTubeDLInstanceHandlersViewModel : YouTubeDLInstanceHandlersViewModel
    {
        public DesignTimeYouTubeDLInstanceHandlersViewModel() : base(DummyYouTubeDL.Instance)
        {
        }
    }
}
