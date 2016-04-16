using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace WebDavSync.Client {
    
    internal static class ExtendedRequestMethods {
        public const string Propfind = "PROPFIND";
        public const string Delete = "DELETE";
    }

    public class WebDavClient : IWebDavClient {

        public event ListCompleteDel ListComplete;
        public event UploadCompleteDel UploadComplete;
        public event DownloadCompleteDel DownloadComplete;
        public event CreateDirCompleteDel CreateDirComplete;
        public event DeleteCompleteDel DeleteComplete;

        public WebDavClient(WebDAVClientProfile clientProfile) {
            if (clientProfile == null) {
                throw new ArgumentNullException("clientProfile");
            }

            this.WebDavServerUrl = clientProfile.DavServer;
            this.WebDavServerPath = clientProfile.DavServerPath;            
            this.User = clientProfile.DavUser;
            this.Password = clientProfile.DavPass;
        }

        //=========================================================================================
        #region WebDAV connection parameters

        private string _server;
        /// <summary>
        /// Specify the WebDAV hostname (required).
        /// </summary>
        private string WebDavServerUrl {
            get { return this._server; }
            set {
                this._server = value.TrimEnd('/');
            }
        }

        private string _basePath = "/";
        /// <summary>
        /// Specify the path of a WebDAV directory (default: /)
        /// </summary>
        public string WebDavServerPath {
            get { return this._basePath; }
            private set {
                value = value.Trim('/');
                this._basePath = "/" + value + "/";
            }
        }

        /// <summary>
        /// Specify a username (optional)
        /// </summary>
        private string User { get; set; }

        /// <summary>
        /// Specify a password (optional)
        /// </summary>
        private string Password { get; set; }

        private string Domain { get; set; }

        #endregion
        //=========================================================================================

        private Uri GetServerUrl(string path) {
            var uriBuilder = new UriBuilder(this.WebDavServerUrl);
            uriBuilder.Path = path;
            return uriBuilder.Uri;
        }

        //=========================================================================================
        #region WebDAV operations

        private const string PropfindTemplate =@"<?xml version=""1.0"" encoding=""utf-8"" ?>
<propfind xmlns=""DAV:"">
    <allprop/>
</propfind>";

        /// <summary>
        /// List files in the root directory
        /// </summary>
        public void List() {
            // Set default depth to 1. This would prevent recursion (default is infinity).
            this.List("/", 1);
        }

        /// <summary>
        /// List files in the given directory
        /// </summary>
        /// <param name="path"></param>
        public void List(String path) {
            // Set default depth to 1. This would prevent recursion.
            this.List(path, 1);
        }

        /// <summary>
        /// List all files present on the server.
        /// </summary>
        /// <param name="remoteFilePath">List only files in this path</param>
        /// <param name="depth">Recursion depth</param>
        /// <returns>A list of files (entries without a trailing slash) and directories (entries with a trailing slash)</returns>
        public void List(String remoteFilePath, int? depth) {
            // Uri should end with a trailing slash
            var listUri = GetServerUrl(remoteFilePath);//, true);

            // see: http://webdav.org/specs/rfc4918.html#METHOD_PROPFIND
            var propfind = PropfindTemplate;

            // see: http://webdav.org/specs/rfc4918.html#rfc.section.9.1.4
            var headers = new Dictionary<string, string>();
            if (depth != null) {
                headers.Add("Depth", depth.ToString());
            }

            this.BeginHttpRequest<string>(
                uri: listUri, 
                requestMethod: ExtendedRequestMethods.Propfind, 
                callback: new AsyncCallback(FinishList),
                headers: headers, 
                content: Encoding.UTF8.GetBytes(propfind.ToString()),
                userState: remoteFilePath);
        }

        /// <summary>
        /// Upload a file to the server
        /// </summary>
        /// <param name="localFilePath">Local path and filename of the file to upload</param>
        /// <param name="remoteFilePath">Destination path and filename of the file on the server</param>
        public void Upload(String localFilePath, String remoteFilePath) {
            this.Upload(localFilePath, remoteFilePath, null);
        }

        /// <summary>
        /// Upload a file to the server
        /// </summary>
        /// <param name="localFilePath">Local path and filename of the file to upload</param>
        /// <param name="remoteFilePath">Destination path and filename of the file on the server</param>
        /// <param name="state">Object to pass along with the callback</param>
        public void Upload(String localFilePath, String remoteFilePath, object state) {
            var fileInfo = new FileInfo(localFilePath);
            var fileSize = fileInfo.Length;
            
            //TODO:true path
            var uploadUri = this.GetServerUrl(remoteFilePath);//, false);

            this.BeginHttpRequest(
                uri: uploadUri, 
                requestMethod: WebRequestMethods.Http.Put, 
                callback: new AsyncCallback(FinishUpload), 
                uploadFilePath: localFilePath, 
                userState: state);
        }

        /// <summary>
        /// Download a file from the server
        /// </summary>
        /// <param name="remoteFilePath">Source path and filename of the file on the server</param>
        /// <param name="localFilePath">Destination path and filename of the file to download on the local filesystem</param>
        public void Download(String remoteFilePath, String localFilePath) {
            // Should not have a trailing slash.
            var downloadUri = this.GetServerUrl(remoteFilePath);//, false);

            this.BeginHttpRequest<string>(
                uri: downloadUri, 
                requestMethod: WebRequestMethods.Http.Get, 
                callback: new AsyncCallback(FinishDownload), 
                userState: localFilePath);
        }

        /// <summary>
        /// Create a directory on the server
        /// </summary>
        /// <param name="remotePath">Destination path of the directory on the server</param>
        public void CreateDir(string remotePath) {
            // Should not have a trailing slash.
            var dirUri = this.GetServerUrl(remotePath);//, false);

            this.BeginHttpRequest<object>(
                uri: dirUri, 
                requestMethod: WebRequestMethods.Http.MkCol,
                callback: new AsyncCallback(FinishCreateDir));
        }

        /// <summary>
        /// Delete a file on the server
        /// </summary>
        /// <param name="remoteFilePath"></param>
        public void Delete(string remoteFilePath) {
            var delUri = this.GetServerUrl(remoteFilePath);//, remoteFilePath.EndsWith("/"));

            this.BeginHttpRequest<object>(
                uri: delUri, 
                requestMethod: ExtendedRequestMethods.Delete, 
                callback: new AsyncCallback(FinishDelete));
        }

        #endregion
        //=========================================================================================

        //=========================================================================================
        #region Async Result Handlers

        private class ManagedState<TUserState> where TUserState : class {
            public TUserState UserState { get; set; }
            public HttpWebRequest HttpWebRequest { get; set; }
        }

        private void FinishList(IAsyncResult result) {
            try {
                var statusCode = 0;
                var webDavContentList = new List<WebDavContent>();

                var managedState = (ManagedState<string>)result.AsyncState;
                var remoteFilePath = managedState.UserState;

                using (var response = (HttpWebResponse)managedState.HttpWebRequest.EndGetResponse(result)) {
                    statusCode = (int)response.StatusCode;
                    using (var stream = response.GetResponseStream()) {
                        XmlDocument xml = new XmlDocument();
                        xml.Load(stream);

                        XmlNamespaceManager xmlNsManager = new XmlNamespaceManager(xml.NameTable);
                        xmlNsManager.AddNamespace("d", "DAV:");

                        foreach (XmlNode node in xml.DocumentElement.ChildNodes) {
                            WebDavContent currentContent = new WebDavContent();
                            XmlNode xmlNode = null;
                            XmlNode xmlNodeProp = null;
                            XmlNode xmlNodePropStat = null;

                            xmlNode = node.SelectSingleNode("d:href", xmlNsManager);
                            currentContent.FilePath = Uri.UnescapeDataString(xmlNode.InnerXml);

                            xmlNode = node.SelectSingleNode("d:propstat", xmlNsManager);
                            xmlNodeProp = xmlNode.SelectSingleNode("d:prop", xmlNsManager);
                            //Childnode
                            xmlNodePropStat = xmlNodeProp.SelectSingleNode("d:getlastmodified", xmlNsManager);
                            currentContent.Lastmodified = Uri.UnescapeDataString(xmlNodePropStat.InnerXml);

                            xmlNodePropStat = xmlNodeProp.SelectSingleNode("d:getcontenttype", xmlNsManager);
                            currentContent.Contenttype = Uri.UnescapeDataString(xmlNodePropStat.InnerXml);

                            xmlNodePropStat = xmlNodeProp.SelectSingleNode("d:getcontentlength", xmlNsManager);
                            currentContent.Contentlength = Convert.ToInt32(Uri.UnescapeDataString(xmlNodePropStat.InnerXml));


                            xmlNodePropStat = xmlNodeProp.SelectSingleNode("d:creationdate", xmlNsManager);
                            currentContent.Creationdate = Uri.UnescapeDataString(xmlNodePropStat.InnerXml);

                            // Want to see directory contents, not the directory itself.
                            if (currentContent.FilePath == remoteFilePath) { continue; }
                            webDavContentList.Add(currentContent);

                        }
                    }
                }

                if (ListComplete != null) {
                    ListComplete(webDavContentList, statusCode);
                }
            } catch (WebException ex) {
                if (ex.Message.Equals("Der Remoteserver hat einen Fehler zurückgegeben: (401) Nicht autorisiert.")) {
                    Debug.WriteLine("Benutzername oder Kennwort ist falsch");
                }
            }
        }

        private void FinishUpload(IAsyncResult result) {
            var statusCode = 0;
            var managedState = (ManagedState<object>)result.AsyncState;

            using (var response = (HttpWebResponse)managedState.HttpWebRequest.EndGetResponse(result)) {
                statusCode = (int)response.StatusCode;
            }

            if (UploadComplete != null) {
                UploadComplete(statusCode, result.AsyncState);
            }
        }

        private void FinishDownload(IAsyncResult result) {
            var statusCode = 0;

            var managedState = (ManagedState<string>)result.AsyncState;
            var localFilePath = managedState.UserState;

            try {
                using (var response = (HttpWebResponse)managedState.HttpWebRequest.EndGetResponse(result)) {
                    statusCode = (int)response.StatusCode;
                    int contentLength = int.Parse(response.GetResponseHeader("Content-Length"));
                    using (Stream s = response.GetResponseStream()) {
                        using (FileStream fs = new FileStream(localFilePath, FileMode.Create, FileAccess.Write)) {
                            byte[] content = new byte[4096];
                            int bytesRead = 0;
                            do {
                                bytesRead = s.Read(content, 0, content.Length);
                                fs.Write(content, 0, bytesRead);
                            } while (bytesRead > 0);
                        }
                    }
                }

                if (DownloadComplete != null) {
                    DownloadComplete(localFilePath, statusCode);
                }
            } catch (WebException) {
                //Exception bei Zugriff verweigert (Briefkasten) --> also ok markieren
                DownloadComplete(localFilePath, 200);
            }
        }

        private void FinishCreateDir(IAsyncResult result) {
            var statusCode = 0;
            var managedState = (ManagedState<string>)result.AsyncState;
            
            using (var response = (HttpWebResponse)managedState.HttpWebRequest.EndGetResponse(result)) {
                statusCode = (int)response.StatusCode;
            }

            if (CreateDirComplete != null) {
                CreateDirComplete(statusCode);
            }
        }

        private void FinishDelete(IAsyncResult result) {
            var statusCode = 0;
            var managedState = (ManagedState<string>)result.AsyncState;

            using (var response = (HttpWebResponse)managedState.HttpWebRequest.EndGetResponse(result)) {
                statusCode = (int)response.StatusCode;
            }

            if (DeleteComplete != null) {
                DeleteComplete(statusCode);
            }
        }

        #endregion
        //=========================================================================================
        
        //=========================================================================================
        #region Server communication

        /// <summary>
        /// This class stores the request state of the request.
        /// </summary>
        private class RequestState {

            public WebRequest Request { get; set; }

            // The request either contains actual content...
            public byte[] Content { get; set; }

            // ...or a reference to the file to be added as content.
            public string UploadFilePath { get; set; }

            // Callback and state to use after handling the request.
            public AsyncCallback UserCallback { get; set; }

            public object UserState { get; set; }
        }

        /// <summary>
        /// Perform the WebDAV call and fire the callback when finished.
        /// </summary>
        private void BeginHttpRequest<TUserState>(
            Uri uri, 
            string requestMethod, 
            AsyncCallback callback,
            IDictionary<string, string> headers = null, 
            byte[] content = null, 
            string uploadFilePath = null, 
            TUserState userState = null) where TUserState : class {

            var httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(uri);

            var managedState = new ManagedState<TUserState> {
                UserState = userState,
                HttpWebRequest = httpWebRequest
            };

            // The server may use authentication
            this.SetCredentials(httpWebRequest);

            httpWebRequest.Method = requestMethod;
            httpWebRequest.ContentType = "text/xml";

            // Need to send along headers?
            if (headers != null) {
                foreach (string key in headers.Keys) {
                    httpWebRequest.Headers.Set(key, headers[key]);
                }
            }

            // Need to send along content?
            if (content != null || uploadFilePath != null) {
                var asyncState = new RequestState {
                    Request = httpWebRequest,
                    UserCallback = callback,
                    UserState = managedState
                };

                if (content != null) {
                    // The request either contains actual content...
                    httpWebRequest.ContentLength = content.Length;
                    asyncState.Content = content;
                } else {
                    // ...or a reference to the file to be added as content.
                    httpWebRequest.ContentLength = new FileInfo(uploadFilePath).Length;
                    asyncState.UploadFilePath = uploadFilePath;
                }

                // Perform asynchronous request.
                IAsyncResult r = (IAsyncResult)asyncState.Request.BeginGetRequestStream(new AsyncCallback(ReadCallback), asyncState);
            } else {
                // Begin async communications
                httpWebRequest.BeginGetResponse(callback, managedState);
            }
        }

        /// <summary>
        /// Submit data asynchronously
        /// </summary>
        /// <param name="result"></param>
        private void ReadCallback(IAsyncResult result) {
            var state = (RequestState)result.AsyncState;
            var request = state.Request;

            // End the Asynchronus request.
            using (Stream streamResponse = request.EndGetRequestStream(result)) {
                // Submit content
                if (state.Content != null) {
                    streamResponse.Write(state.Content, 0, state.Content.Length);
                } else {
                    using (FileStream fs = new FileStream(state.UploadFilePath, FileMode.Open, FileAccess.Read)) {
                        byte[] content = new byte[4096];
                        int bytesRead = 0;
                        do {
                            bytesRead = fs.Read(content, 0, content.Length);
                            streamResponse.Write(content, 0, bytesRead);
                        } while (bytesRead > 0);

                        //XXX: perform upload status callback
                    }
                }
            }

            // Done, invoke user callback
            request.BeginGetResponse(state.UserCallback, state.UserState);
        }

        private void SetCredentials(HttpWebRequest httpWebRequest) {
            if (this.User != null && this.Password != null) {
                NetworkCredential networkCredential;
                if (this.Domain != null) {
                    networkCredential = new NetworkCredential(this.User, this.Password, this.Domain);
                } else {
                    networkCredential = new NetworkCredential(this.User, this.Password);
                }
                httpWebRequest.Credentials = networkCredential;
                // Send authentication along with first request.
                httpWebRequest.PreAuthenticate = true;
            }
        }

        #endregion
        //=========================================================================================
    }
}