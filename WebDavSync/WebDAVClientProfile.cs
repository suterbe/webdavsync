using System;
using System.Collections.Generic;
using System.Text;

namespace WebDavSync
{
    public class WebDAVClientProfile : WebDAVClient
    {

        public WebDAVClientProfile()
        {
        }

        private string _davServer;

        public string DavServerPath
        {
            get { return _davServer; }
            set { _davServer = value; }
        }

        private string _davUrl;

        public string DavServer
        {
            get { return _davUrl; }
            set { _davUrl = value; }
        }
        private string _davUser;

        public string DavUser
        {
            get { return _davUser; }
            set { _davUser = value; }
        }
        private string _davPass;

        public string DavPass
        {
            get { return _davPass; }
            set { _davPass = value; }
        }
        private string _localPath;

        public string LocalPath
        {
            get { return _localPath; }
            set { _localPath = value; }
        }
        private bool _overrideItems;

        public bool OverrideItems
        {
            get { return _overrideItems; }
            set { _overrideItems = value; }
        }

        private string _profileName;

        public string ProfileName
        {
            get { return _profileName; }
            set { _profileName = value; }
        }


    }
}
