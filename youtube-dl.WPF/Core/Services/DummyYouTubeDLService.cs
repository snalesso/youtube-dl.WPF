using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using youtube_dl.WPF.Core.Models;

namespace youtube_dl.WPF.Core.Services
{
    internal class DummyYouTubeDLService : IYouTubeDLService
    {
        private DummyYouTubeDLService() { }

        private static DummyYouTubeDLService _instance = new DummyYouTubeDLService();
        public static DummyYouTubeDLService Instance => DummyYouTubeDLService._instance;

        public string ExeFilePath => throw new NotImplementedException();
        public string DownloadsFolderPath => throw new NotImplementedException();
        public string RepositoryWebSite => throw new NotImplementedException();

        public bool IsBusy => throw new NotImplementedException();
        public IObservable<bool> WhenIsBusyChanged => throw new NotImplementedException();
        
        public IReadOnlyReactiveList<DownloadQueueEntry> DownloadingBatchQueue => throw new NotImplementedException();

        public string OfficialWebsite => throw new NotImplementedException();


        public Task<DownloadHistoryEntry> DownloadAsync(DownloadQueueEntry item)
        {
            throw new NotImplementedException();
        }
        public Task<IReadOnlyList<DownloadHistoryEntry>> DownloadAsync(IReadOnlyList<DownloadQueueEntry> items)
        {
            throw new NotImplementedException();
        }
        public Task<bool> UpdateAsync()
        {
            throw new NotImplementedException();
        }
    }
}