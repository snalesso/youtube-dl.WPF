using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace youtube_dl.WPF.Core.Services
{
    public class WindowsFileSystemService : IFileSystemService
    {
        public bool OpenFolder(string folderPath, bool createIfNotExists = false)
        {
            try
            {
                if (!Directory.Exists(folderPath) && createIfNotExists)
                    Directory.CreateDirectory(folderPath);

                Process.Start($"\"{folderPath}\"");

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool ShowFileInFolder(string selectedFilePath)
        {
            try
            {
                Process.Start("explorer.exe", $"/select, \"{selectedFilePath}\"");

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void NavigateUrl(string url)
        {
            Process.Start(url);
        }

        public Task<bool> UncompressArchiveAsync(string archiveFilePath, string extractDirectoryPath, Action<long, long, int> progressHandler = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DownloadFileAsync(string fileUrl, string saveFilePath)
        {
            return this.DownloadFileAsync(fileUrl, saveFilePath, null);
        }

        public async Task<bool> DownloadFileAsync(string fileUrl, string saveFilePath, Action<long, long, int> progressHandler)
        {
            using (var client = new WebClient())
            {
                //if (progressHandler != null)
                //    // TODO: ensure unsubscription is not needed to avoid memory leaks
                //    client.DownloadProgressChanged += (s, e) => progressHandler(e.TotalBytesToReceive, e.BytesReceived, e.ProgressPercentage);

                using (var receiveStream = await client.OpenReadTaskAsync(fileUrl))
                using (var fileStream = new FileStream(saveFilePath, FileMode.Create, FileAccess.Write))
                {
                    await receiveStream.CopyToAsync(fileStream);
                }
            }

            return true;
        }
    }
}
