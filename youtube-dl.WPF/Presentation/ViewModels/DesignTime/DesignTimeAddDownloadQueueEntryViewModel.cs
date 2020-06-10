using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using youtube_dl.WPF.Core.Services;

namespace youtube_dl.WPF.Presentation.ViewModels.DesignTime
{
    internal class DesignTimeAddDownloadQueueEntryViewModel : AddDownloadQueueEntryViewModel
    {
        public DesignTimeAddDownloadQueueEntryViewModel() : base(DummyDownloadCommandsQueue.Instance)
        {
            this.Url = "https://www.youtube.com/watch?v=tj9ACpY3lW4";
        }
    }
}
