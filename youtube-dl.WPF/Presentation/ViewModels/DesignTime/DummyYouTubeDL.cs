using System;
using youtube_dl.WPF.Core;

namespace youtube_dl.WPF.Presentation.ViewModels.DesignTime
{
    internal class DummyYouTubeDL : YouTubeDL
    {
        private DummyYouTubeDL() : base(new Uri("."), DummyFileSystemService.Instance)        
        { }

        private static DummyYouTubeDL _instance = new DummyYouTubeDL();
        public static DummyYouTubeDL Instance => DummyYouTubeDL._instance;

        //public string ExeFilePath => throw new NotImplementedException();
        //public string DownloadsFolderPath => throw new NotImplementedException();
        //public string RepositoryWebSite => throw new NotImplementedException();

        //public bool IsBusy => throw new NotImplementedException();
        //public IObservable<bool> WhenIsBusyChanged => throw new NotImplementedException();
        
        //public IObservableList<DownloadCommand> Instances => throw new NotImplementedException();

        //public string OfficialWebsite => throw new NotImplementedException();

        //public Task DownloadAsync(DownloadCommand entry)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<bool> UpdateAsync()
        //{
        //    throw new NotImplementedException();
        //}
    }
}