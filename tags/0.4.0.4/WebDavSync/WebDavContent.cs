using System;
using System.Collections.Generic;
using System.Text;

namespace WebDavSync
{
    public class WebDavContent
    {

        private string _filePath;

        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; }
        }
        private string _lastmodified;

        public string Lastmodified
        {
            get { return _lastmodified; }
            set { _lastmodified = value; }
        }
        private string _contenttype;

        public string Contenttype
        {
            get { return _contenttype; }
            set { _contenttype = value; }
        }
        private string _displayname;
        private string _creationdate;

        public string Creationdate
        {
            get { return _creationdate; }
            set { _creationdate = value; }
        }
        private int _contentlength;

        public int Contentlength
        {
            get { return _contentlength; }
            set { _contentlength = value; }
        }


    }
}
