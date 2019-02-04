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
        
        public bool OpenFolder(string directoryPath, bool createIfNotExists = false)
        {
            throw new NotImplementedException();
        }
        public bool ShowFileInFolder(string selectedFileName)
        {
            throw new NotImplementedException();
        }
        public Task<bool> UncompressArchiveAsync(string archiveFilePath, string extractDirectoryPath, Action<long, long, int> progressHandler = null)
        {
            throw new NotImplementedException();
        }

        public void NavigateUrl(string link)
        {
            throw new NotImplementedException();
        }

        //public Task<bool> DownloadFileAsync(string link, string saveFilePath, bool createDirectoryTree = false)
        //{
        //    throw new NotImplementedException();
        //}
        public Task<bool> DownloadFileAsync(string link, string saveFilePath, bool createDirectoryTree = false,  Action<long, long, int> progressHandler=null)
        {
            throw new NotImplementedException();
        }

        public bool UncompressArchive(string archiveFilePath, string extractDirectoryPath, Action<long, long, int> progressHandler = null)
        {
            throw new NotImplementedException();
        }
    }
}
