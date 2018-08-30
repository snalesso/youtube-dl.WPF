using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace youtube_dl.WPF.Core.Services
{
    public interface IFileSystemService
    {
        bool OpenFolder(string directoryPath);
        bool SHowFileInFolder(string selectedFileName);

        void NavigateLink(string link);
    }
}