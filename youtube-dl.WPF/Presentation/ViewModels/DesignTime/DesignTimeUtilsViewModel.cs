namespace youtube_dl.WPF.Presentation.ViewModels.DesignTime
{
    internal class DesignTimeUtilsViewModel : UtilsViewModel
    {
        public DesignTimeUtilsViewModel() : base(DummyYouTubeDL.Instance, DummyFileSystemService.Instance)
        {
        }
    }
}