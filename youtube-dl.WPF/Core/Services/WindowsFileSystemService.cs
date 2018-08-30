using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace youtube_dl.WPF.Core.Services
{
    public class WindowsFileSystemService : IFileSystemService
    {
        public bool OpenFolder(string folderPath)
        {
            try
            {
                Process.Start($"\"{folderPath}\"");

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public bool SHowFileInFolder(string selectedFilePath)
        {
            try
            {
                Process.Start("explorer.exe", $"/select, \"{selectedFilePath}\"");

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void NavigateLink(string link)
        {
            Process.Start(link);
        }
    }
}
