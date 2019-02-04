using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using youtube_dl.WPF.Core.Services;

namespace youtube_dl.WPF.Presentation.ViewModels.DesignTime
{
    internal class DesignTimeDependenciesCheckerViewModel : DependenciesCheckerViewModel
    {
        public DesignTimeDependenciesCheckerViewModel() : base(
            new WindowManager(), DummyYouTubeDLService.Instance, DummyFileSystemService.Instance, () => new DesignTimeShellViewModel())
        {
        }
    }
}
