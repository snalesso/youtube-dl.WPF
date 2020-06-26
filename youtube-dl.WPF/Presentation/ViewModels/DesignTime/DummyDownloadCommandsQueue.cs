using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using ReactiveUI;
using youtube_dl.WPF.Core.Queue;
using youtube_dl.WPF.Presentation.ViewModels.DesignTime;

namespace youtube_dl.WPF.Core.Services
{
    internal class DummyDownloadCommandsQueue : DownloadCommandsQueue
    {
        private DummyDownloadCommandsQueue() : base(DummyYouTubeDL.Instance)
        {
            var dummyEntries = new[]
            {
                new DownloadCommand("https://www.youtube.com/watch?v=WeHEhrj6vEc"
                , new DownloadCommandOptions()),
                new DownloadCommand(
                    //new []
                    //{
                        "https://www.youtube.com/watch?v=0ggg_iCrE6I"
                        //, "https://www.youtube.com/watch?v=HGlIImajcRI"
                        //, "https://www.youtube.com/watch?v=tsPxaAVg584"
                    //}
                    , new DownloadCommandOptions()),
            };

            this.Enqueue(dummyEntries);
            //var sl = new SourceList<DownloadCommand>();
            //sl.AddRange(dummyEntries);
            //this.QueueEntries = sl;
        }

        private static DummyDownloadCommandsQueue _instance = new DummyDownloadCommandsQueue();
        public static DummyDownloadCommandsQueue Instance => DummyDownloadCommandsQueue._instance;

        //public IObservableList<DownloadCommand> QueueEntries { get; }

        //public DownloadCommand Dequeue()
        //{
        //    throw new NotImplementedException();
        //}

        //public override void Enqueue(DownloadCommand item)
        //{
        //    throw new NotImplementedException();
        //}

        //public void Enqueue(IReadOnlyList<DownloadCommand> items)
        //{
        //    throw new NotImplementedException();
        //}

        //public DownloadCommand Extract(DownloadCommand item)
        //{
        //    throw new NotImplementedException();
        //}

        //public void EmptyQueue()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
