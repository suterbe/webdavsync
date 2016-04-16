using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using System.Diagnostics;
using WebDavSync.Client;
using WebDavSync.Types;

namespace WebDavSync.Core {

    public enum SyncStatus {
        SyncStopped,
        SyncStarted,
        SyncFinished
    }

    // Anm. jac: Diese Klasse ist noch nicht ThreadSafe!
    public class WebDavService : IWebDavService {

        /// <summary>
        /// Event um Log ins GUI zu Schreiben
        /// </summary>
        public event EventHandler<LogEventArgs> LogData;

        public event EventHandler<SyncStatusEventArgs> SyncProfileEnd;

        public WebDavService(WebDAVClientProfile profile) :
            this(new WebDavClient(profile), profile.LocalPath, profile.ProfileName) {
        }

        // Für tests
        // Todo,jac: Buildscirpt anpassen, dass im AssemblyInfo InternalsVisibleTo gesetzt wird.
        public WebDavService(IWebDavClient client, string localPath, string profileName) {
            this.WebDavClient = client;
            this.LocalPath = localPath;
            this.ProfileName = profileName;

            //client.DownloadComplete += new DownloadCompleteDel(client_DownloadComplete);
        }

        //private void client_DownloadComplete(string localPath, int statusCode) {
        //    if (statusCode != 200) {
        //        this.OnLogData(new LogEventArgs(
        //            @"Die Datei ""{0}"" konnte nicht heruntergeladen werden.",
        //            Path.GetFileName(localPath)));
        //    }
        //}

        private IWebDavClient WebDavClient { get; set; }

        private string LocalPath { get; set; }
        private string ProfileName { get; set; }

        public void DownloadWebDavContent() {
            this.OnLogData(new LogEventArgs("*********************************************"));
            this.OnLogData(new LogEventArgs("INFO: Starte Download für Profil " + ProfileName));
            
            this.DownloadWebDavContentRecursive(this.WebDavClient.WebDavServerPath, LocalPath);
            
            //TODO: Status setzen, Log Schreiben
            this.OnLogData(new LogEventArgs("INFO: Download für Profil " + ProfileName + " beendet"));
            this.OnLogData(new LogEventArgs("*********************************************" + Environment.NewLine));
            this.OnSyncProfileEnd(new SyncStatusEventArgs(SyncStatus.SyncFinished));
        }

        private void DownloadWebDavContentRecursive(string currentWebDavPath, string currentLocalPath) {
            _webDavContentList.Clear();

            this.EnsureLocalFolderExists(currentLocalPath);

            autoResetEvent = new AutoResetEvent(false);
            // gehört hier nicht hin..
            // Evtl. mit einem dict
            WebDavClient.ListComplete += new ListCompleteDel(c_ListComplete);



            //WebDavClient.ListComplete += (list, statusCode) => {
            //    Debug.Assert(statusCode == 207);
            //    _webDavContentList = list;
            //    autoResetEvent.Set();
            //};

            WebDavClient.List(currentWebDavPath);
            autoResetEvent.WaitOne();
            List<WebDavContent> contentList = new List<WebDavContent>(_webDavContentList);
            foreach (WebDavContent item in contentList) {
                string fileName = item.FilePath;

                if (this.IsFolder(item)) {
                    this.HandleFolder(currentLocalPath, item);
                } else {
                    this.HandleFile(currentLocalPath, item);
                }
            }
        }

        //=========================================================================================
        #region Private Helpers

        private bool IsFolder(WebDavContent item) {
            return item.Contentlength == 0;
        }

        private bool IsBriefkasten(WebDavContent item) {
            return item.FilePath.IndexOf("briefkasten", StringComparison.OrdinalIgnoreCase) > 0;
        }

        private void EnsureLocalFolderExists(string folderPath) {
            if (!Directory.Exists(folderPath)) {
                try {
                    Directory.CreateDirectory(folderPath);
                } catch (Exception ex) {
                    throw new Exception("Fehler beim erstellen des Ordners " + folderPath + " " + ex.Message);
                }
            }
        }

        private void HandleFolder(string currentLocalPath, WebDavContent item) {
            var fileName = item.FilePath;

            if (this.IsBriefkasten(item)) {
                this.OnLogData(new LogEventArgs("INFO: Kein Update, weil Briefkasten!!! " + item.FilePath));
            } else {
                this.DownloadWebDavContentRecursive(fileName, currentLocalPath + "\\" + fileName.Substring(fileName.TrimEnd(new char[] { '/' }).LastIndexOf('/')).Trim('/'));
            }
        }

        private void HandleFile(string currentLocalPath, WebDavContent item) {
            var fileName = item.FilePath;

            if (!fileName.Contains("\"")) {
                string strlocalfile = currentLocalPath + "\\" + fileName.Substring(fileName.LastIndexOf('/')).TrimStart('/');

                FileInfo objLocalFile = new FileInfo(strlocalfile);
                // Todo,jac: objLocalFile.Exists prüfen
                if (objLocalFile.LastWriteTime > Convert.ToDateTime(item.Lastmodified)) {
                    //File ist lokal neuer
                    //Debug.WriteLine("Lokale Datei ist neuer --> kein Download");
                    this.OnLogData(new LogEventArgs("INFO: Kein Update: " + strlocalfile));
                } else {
                    //Debug.WriteLine("Lokale Datei ist älter --> Download");
                    this.OnLogData(new LogEventArgs("INFO: Update:      " + strlocalfile));

                    // Todo,jac: wieso wird hier blockiert -> Queue?
                    autoResetEvent = new AutoResetEvent(false);
                    WebDavClient.DownloadComplete += new DownloadCompleteDel(c_DownloadComplete);
                    this.WebDavClient.Download(fileName, strlocalfile);
                    
                    autoResetEvent.WaitOne();
                }
            } else {
                //Wegen eines der Zeichen \ / : * ? " < > | ist dieses Objekt in Windows Webordnern nicht sichtbar
                LogData(this, new LogEventArgs("ERROR: Datei " + fileName + " kann nicht heruntergeladen werden. Dateinamen wird nicht unterstützt"));
                Debug.WriteLine("Datei kann nicht herungergeladen werden " + fileName);
            }
        }

        #endregion
        //=========================================================================================

        private void OnLogData(LogEventArgs e) {
            if (this.LogData != null) {
                this.LogData(this, e);
            }
        }

        private void OnSyncProfileEnd(SyncStatusEventArgs e) {
            if (this.SyncProfileEnd != null) {
                this.SyncProfileEnd(this, e);
            }
        }

        #region Statische Methoden
        // Todo,jac: !! refactor code below !!

        static AutoResetEvent autoResetEvent;

        //static void c_DeleteComplete(int statusCode) {
        //    Debug.Assert(statusCode == 204);
        //    autoResetEvent.Set();
        //}

        //static void c_UploadComplete(int statusCode, object state) {
        //    Debug.Assert(statusCode == 201);
        //    autoResetEvent.Set();
        //}

        //static void c_CreateDirComplete(int statusCode) {
        //    Debug.Assert(statusCode == 200 || statusCode == 201);
        //    autoResetEvent.Set();
        //}

        static IList<WebDavContent> _webDavContentList = new List<WebDavContent>();
        static void c_ListComplete(IList<WebDavContent> list, int statusCode) {
            Debug.Assert(statusCode == 207);
            _webDavContentList = list;
            autoResetEvent.Set();
        }

        static void c_DownloadComplete(string localPath, int code) {
            Debug.Assert(code == 200);
            autoResetEvent.Set();
        }

        #endregion
    }
}