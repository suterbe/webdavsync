using System;
using WebDavSync.Types;
namespace WebDavSync.Core {

    public interface IWebDavService {
        
        event EventHandler<LogEventArgs> LogData;
        event EventHandler<SyncStatusEventArgs> SyncProfileEnd;

        void DownloadWebDavContent();
    }
}
