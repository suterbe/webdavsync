using System;
using System.Collections.Generic;

namespace WebDavSync.Client {

    public delegate void ListCompleteDel(IList<WebDavContent> contentList, int statusCode);
    public delegate void DownloadCompleteDel(string localPath, int statusCode);

    public delegate void UploadCompleteDel(int statusCode, object state);
    public delegate void CreateDirCompleteDel(int statusCode);
    public delegate void DeleteCompleteDel(int statusCode);

    public interface IWebDavClient {

        event DownloadCompleteDel DownloadComplete;
        event ListCompleteDel ListComplete;
        
        string WebDavServerPath { get; }

        void List();
        void List(string path);
        void List(string remoteFilePath, int? depth);

        void Download(string remoteFilePath, string localFilePath);
    }
}
