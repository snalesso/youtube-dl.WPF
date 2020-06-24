using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace youtube_dl.WPF.Core
{
    public partial class YouTubeDL
    {
        private class BusyManager
        {
            private readonly ReaderWriterLock rw = new ReaderWriterLock();
            private readonly AsyncLock asyncLock = new AsyncLock();
            private readonly SemaphoreSlim _isBusySemaphore = new SemaphoreSlim(1, 1);
            //private readonly SemaphoreSlim _isBusySemaphore = new SemaphoreSlim(1, 1);

            public BusyManager()
            {
                //this._isBusySemaphore.
            }

            //public async Task WaitForDownloadAsync()
            //{
            //    //await this.asyncLock.Wait();
            //}
        }
    }
}