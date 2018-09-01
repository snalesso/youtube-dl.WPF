using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace youtube_dl.WPF.Core.Services
{
    public interface IFileSystemService
    {
        bool OpenFolder(string folderPath, bool createIfNotExists = false);
        bool ShowFileInFolder(string selectedFileName);

        Task<bool> UncompressArchiveAsync(string archiveFilePath, string extractDirectoryPath, Action<long, long, int> progressHandler = null);

        void NavigateUrl(string url);

        Task<bool> DownloadFileAsync(string fileUrl, string saveFilePath);
        Task<bool> DownloadFileAsync(string fileUrl, string saveFilePath, Action<long, long, int> progressHandler);
    }
}