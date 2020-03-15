using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace youtube_dl.WPF.Core.Services
{
    public class WindowsFileSystemService : IFileSystemService
    {
        public bool OpenFolder(string folderPath, bool createIfNotExists = false)
        {
            try
            {
                if (!Directory.Exists(folderPath))
                {
                    if (createIfNotExists)
                        Directory.CreateDirectory(folderPath);
                    else
                        return false;
                }

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

        public bool UncompressArchive(string archiveFilePath, string extractDirectoryPath, Action<long, long, int> progressHandler = null)
        {
            var zipFile = new FastZip();

            try
            {
                zipFile.ExtractZip(archiveFilePath, extractDirectoryPath, FastZip.Overwrite.Always, null, null, null, true);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Task<bool> UncompressArchiveAsync(string archiveFilePath, string extractDirectoryPath, Action<long, long, int> progressHandler = null)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DownloadFileAsync(string fileUrl, string saveFilePath, bool createDirectoryTree = false, Action<long, long, int> progressHandler = null)
        {
            using (var client = new WebClient())
            {
                if (createDirectoryTree)
                {
                    var saveFileDir = Path.GetDirectoryName(Path.GetFullPath(saveFilePath));
                    if (!Directory.Exists(saveFileDir))
                    {
                        Directory.CreateDirectory(saveFileDir);
                    }
                }

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