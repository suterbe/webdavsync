﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace WebDavSync
{
    public delegate void ListCompleteDel(List<WebDavContent> contentList, int statusCode);
    public delegate void UploadCompleteDel(int statusCode, object state);
    public delegate void DownloadCompleteDel(int statusCode);
    public delegate void CreateDirCompleteDel(int statusCode);
    public delegate void DeleteCompleteDel(int statusCode);


    public class WebDAVClient
    {
        public event ListCompleteDel ListComplete;
        public event UploadCompleteDel UploadComplete;
        public event DownloadCompleteDel DownloadComplete;
        public event CreateDirCompleteDel CreateDirComplete;
        public event DeleteCompleteDel DeleteComplete;

        //XXX: submit along with state object.
        HttpWebRequest httpWebRequest;

        #region WebDAV connection parameters

        private String _server;
        /// <summary>
        /// Specify the WebDAV hostname (required).
        /// </summary>
        public String WebDavServerUrl
        {
            get { return _server; }
            set
            {
                value = value.TrimEnd('/');
                _server = value;
            }
        }

        private String _basePath = "/";
        /// <summary>
        /// Specify the path of a WebDAV directory (default: /)
        /// </summary>
        public String WebDavServerPath
        {
            get { return _basePath; }
            set
            {
                value = value.Trim('/');
                _basePath = "/" + value + "/";
            }
        }


        private String _user;
        /// <summary>
        /// Specify a username (optional)
        /// </summary>
        public String WebDavUsername
        {
            get { return _user; }
            set { _user = value; }
        }

        private String _pass;
        /// <summary>
        /// Specify a password (optional)
        /// </summary>
        public String WebDavPassword
        {
            get { return _pass; }
            set { _pass = value; }
        }

        private String _domain = null;
        public String Domain
        {
            get { return _domain; }
            set { _domain = value; }
        }

        Uri getServerUrl(String path, Boolean appendTrailingSlash)
        {
            //String completePath = _basePath;
            //if (path != null)
            //{
            //    completePath += path.Trim('/');
            //}
            //if (appendTrailingSlash) { completePath += '/'; }

            //return new Uri(_server + completePath);
            var uriBuilder = new UriBuilder(_server);
            uriBuilder.Path = path;
            return uriBuilder.Uri;
        }
        #endregion

        #region WebDAV operations
        /// <summary>
        /// List files in the root directory
        /// </summary>
        public void List()
        {
            // Set default depth to 1. This would prevent recursion (default is infinity).
            List("/", 1);
        }

        /// <summary>
        /// List files in the given directory
        /// </summary>
        /// <param name="path"></param>
        public void List(String path)
        {
            // Set default depth to 1. This would prevent recursion.
            List(path, 1);
        }

        /// <summary>
        /// List all files present on the server.
        /// </summary>
        /// <param name="remoteFilePath">List only files in this path</param>
        /// <param name="depth">Recursion depth</param>
        /// <returns>A list of files (entries without a trailing slash) and directories (entries with a trailing slash)</returns>
        public void List(String remoteFilePath, int? depth)
        {
            // Uri should end with a trailing slash
            Uri listUri = getServerUrl(remoteFilePath, true);

            // http://webdav.org/specs/rfc4918.html#METHOD_PROPFIND
            StringBuilder propfind = new StringBuilder();
            propfind.Append("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            propfind.Append("<propfind xmlns=\"DAV:\">");
            propfind.Append("<allprop/>");
            //propfind.Append("");
            //propfind.Append("");

            //propfind.Append("  <propname/>");
            propfind.Append("</propfind>");

            // Depth header: http://webdav.org/specs/rfc4918.html#rfc.section.9.1.4
            IDictionary<string, string> headers = new Dictionary<string, string>();
            if (depth != null)
            {
                headers.Add("Depth", depth.ToString());
            }

            AsyncCallback callback = new AsyncCallback(FinishList);
            HTTPRequest(listUri, "PROPFIND", headers, Encoding.UTF8.GetBytes(propfind.ToString()), null, callback, remoteFilePath);
        }


        void FinishList(IAsyncResult result)
        {
            try
            {

                string remoteFilePath = (string)result.AsyncState;
                int statusCode = 0;
                List<WebDavContent> webDavContentList = new List<WebDavContent>();

                using (HttpWebResponse response = (HttpWebResponse)httpWebRequest.EndGetResponse(result))
                {
                    statusCode = (int)response.StatusCode;
                    using (Stream stream = response.GetResponseStream())
                    {
                        XmlDocument xml = new XmlDocument();
                        xml.Load(stream);

                        XmlNamespaceManager xmlNsManager = new XmlNamespaceManager(xml.NameTable);
                        xmlNsManager.AddNamespace("d", "DAV:");

                        foreach (XmlNode node in xml.DocumentElement.ChildNodes)
                        {
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

                if (ListComplete != null)
                {
                    ListComplete(webDavContentList, statusCode);
                }
            }
            catch (WebException ex)
            {
                if (ex.Message.Equals("Der Remoteserver hat einen Fehler zurückgegeben: (401) Nicht autorisiert."))
                {
                    Debug.WriteLine("Benutzername oder Kennwort ist falsch");
                }
            }
        }

        /// <summary>
        /// Upload a file to the server
        /// </summary>
        /// <param name="localFilePath">Local path and filename of the file to upload</param>
        /// <param name="remoteFilePath">Destination path and filename of the file on the server</param>
        public void Upload(String localFilePath, String remoteFilePath)
        {
            Upload(localFilePath, remoteFilePath, null);
        }

        /// <summary>
        /// Upload a file to the server
        /// </summary>
        /// <param name="localFilePath">Local path and filename of the file to upload</param>
        /// <param name="remoteFilePath">Destination path and filename of the file on the server</param>
        /// <param name="state">Object to pass along with the callback</param>
        public void Upload(String localFilePath, String remoteFilePath, object state)
        {
            FileInfo fileInfo = new FileInfo(localFilePath);
            long fileSize = fileInfo.Length;
            //TODO:true path
            Uri uploadUri = getServerUrl(remoteFilePath, false);
            string method = WebRequestMethods.Http.Put.ToString();

            AsyncCallback callback = new AsyncCallback(FinishUpload);
            HTTPRequest(uploadUri, method, null, null, localFilePath, callback, state);
        }


        void FinishUpload(IAsyncResult result)
        {
            int statusCode = 0;

            using (HttpWebResponse response = (HttpWebResponse)httpWebRequest.EndGetResponse(result))
            {
                statusCode = (int)response.StatusCode;
            }

            if (UploadComplete != null)
            {
                UploadComplete(statusCode, result.AsyncState);
            }
        }


        /// <summary>
        /// Download a file from the server
        /// </summary>
        /// <param name="remoteFilePath">Source path and filename of the file on the server</param>
        /// <param name="localFilePath">Destination path and filename of the file to download on the local filesystem</param>
        public void Download(String remoteFilePath, String localFilePath)
        {
            // Should not have a trailing slash.
            Uri downloadUri = getServerUrl(remoteFilePath, false);
            string method = WebRequestMethods.Http.Get.ToString();

            AsyncCallback callback = new AsyncCallback(FinishDownload);
            HTTPRequest(downloadUri, method, null, null, null, callback, localFilePath);
        }


        void FinishDownload(IAsyncResult result)
        {
            string localFilePath = (string)result.AsyncState;
            int statusCode = 0;
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)httpWebRequest.EndGetResponse(result))
                {
                    statusCode = (int)response.StatusCode;
                    int contentLength = int.Parse(response.GetResponseHeader("Content-Length"));
                    using (Stream s = response.GetResponseStream())
                    {
                        using (FileStream fs = new FileStream(localFilePath, FileMode.Create, FileAccess.Write))
                        {
                            byte[] content = new byte[4096];
                            int bytesRead = 0;
                            do
                            {
                                bytesRead = s.Read(content, 0, content.Length);
                                fs.Write(content, 0, bytesRead);
                            } while (bytesRead > 0);
                        }
                    }
                }


                if (DownloadComplete != null)
                {
                    DownloadComplete(statusCode);
                }
            }
            //Exception bei Zugriff verweigert (Briefkasten) --> also ok markieren
            catch (WebException ex)
            {
                DownloadComplete(200);
            }

        }


        /// <summary>
        /// Create a directory on the server
        /// </summary>
        /// <param name="remotePath">Destination path of the directory on the server</param>
        public void CreateDir(string remotePath)
        {
            // Should not have a trailing slash.
            Uri dirUri = getServerUrl(remotePath, false);
            string method = WebRequestMethods.Http.MkCol.ToString();

            AsyncCallback callback = new AsyncCallback(FinishCreateDir);
            HTTPRequest(dirUri, method, null, null, null, callback, null);
        }


        void FinishCreateDir(IAsyncResult result)
        {
            int statusCode = 0;

            using (HttpWebResponse response = (HttpWebResponse)httpWebRequest.EndGetResponse(result))
            {
                statusCode = (int)response.StatusCode;
            }

            if (CreateDirComplete != null)
            {
                CreateDirComplete(statusCode);
            }
        }


        /// <summary>
        /// Delete a file on the server
        /// </summary>
        /// <param name="remoteFilePath"></param>
        public void Delete(string remoteFilePath)
        {
            Uri delUri = getServerUrl(remoteFilePath, remoteFilePath.EndsWith("/"));

            AsyncCallback callback = new AsyncCallback(FinishDelete);
            HTTPRequest(delUri, "DELETE", null, null, null, callback, null);
        }


        void FinishDelete(IAsyncResult result)
        {
            int statusCode = 0;

            using (HttpWebResponse response = (HttpWebResponse)httpWebRequest.EndGetResponse(result))
            {
                statusCode = (int)response.StatusCode;
            }

            if (DeleteComplete != null)
            {
                DeleteComplete(statusCode);
            }
        }
        #endregion

        #region Server communication

        /// <summary>
        /// This class stores the request state of the request.
        /// </summary>
        public class RequestState
        {
            public WebRequest request;
            // The request either contains actual content...
            public byte[] content;
            // ...or a reference to the file to be added as content.
            public string uploadFilePath;
            // Callback and state to use after handling the request.
            public AsyncCallback userCallback;
            public object userState;
        }

        /// <summary>
        /// Perform the WebDAV call and fire the callback when finished.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="requestMethod"></param>
        /// <param name="headers"></param>
        /// <param name="content"></param>
        /// <param name="uploadFilePath"></param>
        /// <param name="callback"></param>
        /// <param name="state"></param>
        void HTTPRequest(Uri uri, string requestMethod, IDictionary<string, string> headers, byte[] content, string uploadFilePath, AsyncCallback callback, object state)
        {
            httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(uri);

            // The server may use authentication
            if (_user != null && _pass != null)
            {
                NetworkCredential networkCredential;
                if (_domain != null)
                {
                    networkCredential = new NetworkCredential(_user, _pass, _domain);
                }
                else
                {
                    networkCredential = new NetworkCredential(_user, _pass);
                }
                httpWebRequest.Credentials = networkCredential;
                // Send authentication along with first request.
                httpWebRequest.PreAuthenticate = true;
            }
            httpWebRequest.Method = requestMethod;
            httpWebRequest.ContentType = "text/xml";

            // Need to send along headers?
            if (headers != null)
            {
                foreach (string key in headers.Keys)
                {
                    httpWebRequest.Headers.Set(key, headers[key]);
                }
            }

            // Need to send along content?
            if (content != null || uploadFilePath != null)
            {
                RequestState asyncState = new RequestState();
                asyncState.request = httpWebRequest;
                asyncState.userCallback = callback;
                asyncState.userState = state;

                if (content != null)
                {
                    // The request either contains actual content...
                    httpWebRequest.ContentLength = content.Length;
                    asyncState.content = content;
                }
                else
                {
                    // ...or a reference to the file to be added as content.
                    httpWebRequest.ContentLength = new FileInfo(uploadFilePath).Length;
                    asyncState.uploadFilePath = uploadFilePath;
                }

                // Perform asynchronous request.
                IAsyncResult r = (IAsyncResult)asyncState.request.BeginGetRequestStream(new AsyncCallback(ReadCallback), asyncState);
            }
            else
            {

                // Begin async communications
                httpWebRequest.BeginGetResponse(callback, state);
            }
        }

        /// <summary>
        /// Submit data asynchronously
        /// </summary>
        /// <param name="result"></param>
        private void ReadCallback(IAsyncResult result)
        {
            RequestState state = (RequestState)result.AsyncState;
            WebRequest request = state.request;

            // End the Asynchronus request.
            using (Stream streamResponse = request.EndGetRequestStream(result))
            {
                // Submit content
                if (state.content != null)
                {
                    streamResponse.Write(state.content, 0, state.content.Length);
                }
                else
                {
                    using (FileStream fs = new FileStream(state.uploadFilePath, FileMode.Open, FileAccess.Read))
                    {
                        byte[] content = new byte[4096];
                        int bytesRead = 0;
                        do
                        {
                            bytesRead = fs.Read(content, 0, content.Length);
                            streamResponse.Write(content, 0, bytesRead);
                        } while (bytesRead > 0);

                        //XXX: perform upload status callback
                    }
                }
            }

            // Done, invoke user callback
            request.BeginGetResponse(state.userCallback, state.userState);
        }
        #endregion
    }
}

