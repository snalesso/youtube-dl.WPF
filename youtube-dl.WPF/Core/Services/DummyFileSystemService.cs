using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace youtube_dl.WPF.Core.Services
{
    internal class DummyFileSystemService : IFileSystemService
    {
        private DummyFileSystemService() { }

        private static DummyFileSystemService _instance = new DummyFileSystemService();
        public static DummyFileSystemService Instance => DummyFileSystemService._instance;

        public void NavigateLink(string link)
        {
            throw new NotImplementedException();
        }

        public bool OpenFolder(string directoryPath)
        {
            throw new NotImplementedException();
        }

        public bool SHowFileInFolder(string selectedFileName)
        {
            throw new NotImplementedException();
        }
    }
}
