using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace youtube_dl.WPF.Presentation.ViewModels.DesignTime
{
    internal class DesignTimeShellViewModel : ShellViewModel
    {
        public DesignTimeShellViewModel() : base(null,new DesignTimeAddDownloadQueueEntryViewModel(), new DesignTimeDownloadQueueViewModel(), new DesignTimeDownloadViewModel(), null)
        {
        }
    }
}