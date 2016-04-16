using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace WebDavSync
{


    /// <summary>
    /// Delegate um Log ins GUI zu schreiben
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void LogDataHandler(object sender, WebDavClientEventArgs e);
    
    public delegate void SyncEnd(object sender, SyncStatusEventArgs e);

    public class WebDavClientHelper
    {
        private WebDAVClient webDavClient = new WebDAVClient();
        private string localPath;
        private string profileName;

        /// <summary>
        /// Event um Log ins GUI zu Schreiben
        /// </summary>
        public event LogDataHandler LogData;

        public event SyncEnd SyncProfileEnd;

        public enum SyncStatus
        {
            SyncStopped,
            SyncStarted,
            SyncFinished
        }

        public WebDavClientHelper(WebDAVClientProfile profile)
        {
            webDavClient.WebDavPassword = profile.DavPass;
            webDavClient.WebDavServerPath = profile.DavServerPath;
            webDavClient.WebDavServerUrl = profile.DavServer;
            webDavClient.WebDavUsername = profile.DavUser;
            localPath = profile.LocalPath;
            profileName = profile.ProfileName;

        }


        public void downloadWebDavContent()
        {
            Debug.WriteLine(this.GetType().ToString() + " - " + System.Reflection.MethodBase.GetCurrentMethod() + ": Beginn " + DateTime.Now.ToString());
            LogData(this, new WebDavClientEventArgs("*********************************************"));
            LogData(this, new WebDavClientEventArgs("INFO: Starte Download für Profil " + profileName));
            downloadWebDavContent(webDavClient.WebDavServerPath, localPath);
            //TODO: Status setzen, Log Schreiben
            LogData(this, new WebDavClientEventArgs("INFO: Download für Profil " + profileName + " beendet"));
            LogData(this, new WebDavClientEventArgs("*********************************************" + Environment.NewLine));
            SyncProfileEnd(this, new SyncStatusEventArgs(SyncStatus.SyncFinished));
            Debug.WriteLine(this.GetType().ToString() + " - " + System.Reflection.MethodBase.GetCurrentMethod() + ": End " + DateTime.Now.ToString());
        }

        private void downloadWebDavContent(string currentWebDavPath, string currentLocalPath)
        {

            Debug.WriteLine(this.GetType().ToString() + " - " + System.Reflection.MethodBase.GetCurrentMethod() + ": Beginn " + DateTime.Now.ToString());
            _webDavContentList.Clear();
            if (!Directory.Exists(currentLocalPath))
            {
                try { Directory.CreateDirectory(currentLocalPath); }
                catch (Exception ex)
                {
                    throw new Exception("Fehler beim erstellen des Ordners " + currentLocalPath + " " + ex.Message);
                }
            }
            autoResetEvent = new AutoResetEvent(false);
            webDavClient.ListComplete += new ListCompleteDel(c_ListComplete);
            webDavClient.List(currentWebDavPath);
            autoResetEvent.WaitOne();
            List<WebDavContent> contentList = new List<WebDavContent>(_webDavContentList);
            foreach (WebDavContent item in contentList)
            {
                string fileName = item.FilePath;
                if (item.Contentlength == 0)
                {
                    if (!item.FilePath.ToLower().Contains("briefkasten"))
                    {
                        downloadWebDavContent(fileName, currentLocalPath + "\\" + fileName.Substring(fileName.TrimEnd(new char[] { '/' }).LastIndexOf('/')).Trim('/'));
                    }
                    else
                    {
                        LogData(this, new WebDavClientEventArgs("INFO: Kein Update, weil Briefkasten!!! " + item.FilePath));
                    }
                }
                else //File
                {
                    if (!fileName.Contains("\""))
                    {
                        string strlocalfile = currentLocalPath + "\\" + fileName.Substring(fileName.LastIndexOf('/')).TrimStart('/');

                        FileInfo objLocalFile = new FileInfo(strlocalfile);
                        if (objLocalFile.LastWriteTime > Convert.ToDateTime(item.Lastmodified))
                        {
                            //File ist lokal neuer
                            Debug.WriteLine("Lokale Datei ist neuer --> kein Download");
                            LogData(this, new WebDavClientEventArgs("INFO: Kein Update: " + strlocalfile));
                        }
                        else
                        {
                            Debug.WriteLine("Lokale Datei ist älter --> Download");
                            LogData(this, new WebDavClientEventArgs("INFO: Update:      " + strlocalfile));
                            autoResetEvent = new AutoResetEvent(false);
                            webDavClient.DownloadComplete += new DownloadCompleteDel(c_DownloadComplete);
                            webDavClient.Download(fileName, strlocalfile);
                            autoResetEvent.WaitOne();
                        }

                    }
                    else
                    {
                        //Wegen eines der Zeichen \ / : * ? " < > | ist dieses Objekt in Windows Webordnern nicht sichtbar
                        LogData(this, new WebDavClientEventArgs("ERROR: Datei " + fileName + " kann nicht heruntergeladen werden. Dateinamen wird nicht unterstützt"));
                        Debug.WriteLine("Datei kann nicht herungergeladen werden " + fileName);
                        break;
                    }


                }
            }

            Debug.WriteLine(this.GetType().ToString() + " - " + System.Reflection.MethodBase.GetCurrentMethod() + ": End " + DateTime.Now.ToString());
        }





        #region Statische Methoden
        static AutoResetEvent autoResetEvent;




        static void c_DeleteComplete(int statusCode)
        {
            Debug.Assert(statusCode == 204);
            autoResetEvent.Set();
        }

        static void c_UploadComplete(int statusCode, object state)
        {
            Debug.Assert(statusCode == 201);
            autoResetEvent.Set();
        }

        static void c_CreateDirComplete(int statusCode)
        {
            Debug.Assert(statusCode == 200 || statusCode == 201);
            autoResetEvent.Set();
        }

        static List<WebDavContent> _webDavContentList = new List<WebDavContent>();
        static void c_ListComplete(List<WebDavContent> list, int statusCode)
        {
            Debug.Assert(statusCode == 207);
            _webDavContentList = list;
            autoResetEvent.Set();
        }

        static void c_DownloadComplete(int code)
        {
            Debug.Assert(code == 200);
            autoResetEvent.Set();
        }
        #endregion
    }
}


/// <summary>
/// Klasse um EventParameter von Events zu verarbeiten
/// </summary>
public class WebDavClientEventArgs : EventArgs
{
    private string _log = "";
    /// <summary>
    /// Konstruktor für EventArgs
    /// </summary>
    /// <param name="logtext">Nimmt den LogText als Sring entgegen</param>
    public WebDavClientEventArgs(string logtext)
    {
        _log = logtext;
    }

    /// <summary>
    /// LogText als String
    /// </summary>
    public string Log
    {
        get { return _log; }
    }

}

/// <summary>
/// Klasse um EventParameter von Events zu verarbeiten
/// </summary>
public class SyncStatusEventArgs : EventArgs
{
    private WebDavSync.WebDavClientHelper.SyncStatus _status;
    /// <summary>
    /// Konstruktor für EventArgs
    /// </summary>
    /// <param name="logtext">Nimmt den LogText als Sring entgegen</param>
    public SyncStatusEventArgs(WebDavSync.WebDavClientHelper.SyncStatus status)
    {
        _status = status;
    }

    /// <summary>
    /// Status als boolean
    /// </summary>
    public WebDavSync.WebDavClientHelper.SyncStatus Status
    {
        get { return _status; }
    }

}