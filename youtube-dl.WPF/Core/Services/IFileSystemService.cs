using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace youtube_dl.WPF.Core.Services
{
    // add IsBusy

    public interface IFileSystemService
    {
        bool OpenFolder(string folderPath, bool createIfNotExists = false);
        bool ShowFileInFolder(string selectedFileName);

        bool UncompressArchive(string archiveFilePath, string extractDirectoryPath, Action<long, long, int> progressHandler = null);
        Task<bool> UncompressArchiveAsync(string archiveFilePath, string extractDirectoryPath, Action<long, long, int> progressHandler = null);

        void NavigateUrl(string url);

        //Task<bool> DownloadFileAsync(string fileUrl, string saveFilePath, bool createDirectoryTree = false);
        Task<bool> DownloadFileAsync(string fileUrl, string saveFilePath, bool createDirectoryTree = false, Action<long, long, int> progressHandler = null);
    }
}