using System;
using System.Collections.Generic;
using System.Text;
using WebDavSync;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Xml.XPath;

namespace WebDavSyncGUI.Logic
{
    public class ProfileHelper
    {
        private string _strXmlPath;

        public ProfileHelper(string strProfilePath)
        {
            _strXmlPath = strProfilePath;
        }

        public List<WebDAVClientProfile> getProfiles()
        {
            List<WebDAVClientProfile> listProfile = new List<WebDAVClientProfile>();

            try
            {


                XPathDocument doc = new XPathDocument(_strXmlPath);
                XPathNavigator nav = doc.CreateNavigator();

                XPathNodeIterator xnodes = nav.Select("/WebDavClient");
                xnodes.Current.MoveToFirstChild();
                while (xnodes.MoveNext())
                {


                    xnodes.Current.MoveToFirstChild();
                    do
                    {
                        WebDAVClientProfile entry = new WebDAVClientProfile();
                        entry.ProfileName = xnodes.Current.GetAttribute("ProfileName", "").ToString();
                        entry.DavServer = xnodes.Current.GetAttribute("DavServer", "").ToString();
                        entry.DavServerPath = xnodes.Current.GetAttribute("DavServerPath", "").ToString();
                        entry.DavUser = xnodes.Current.GetAttribute("DavUser", "").ToString();
                        entry.DavPass =  new Encryption().decrypt(xnodes.Current.GetAttribute("DavPass", "").ToString());
                        entry.LocalPath = xnodes.Current.GetAttribute("LocalPath", "").ToString();
                        entry.OverrideItems = Convert.ToBoolean(xnodes.Current.GetAttribute("OverrideItems", "").ToString());
                        listProfile.Add(entry);

                    } while (xnodes.Current.MoveToNext());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Profile  - " + System.Reflection.MethodBase.GetCurrentMethod() + ": Exeption " + ex.Message);
            }
            return listProfile;
           

        }

        public void saveProfiles(List<WebDAVClientProfile> listProfiles)
        {
            Debug.WriteLine(this.GetType().ToString() + " - " + System.Reflection.MethodBase.GetCurrentMethod() + ": Beginn");
            if (listProfiles.Count == 0)
            { //throw new Exception("XML wurde nicht gespeichert, da die Liste der Einträge leer ist."); 
            }

            try
            {
                //Erstellt den Pfad wenn dieser nicht vorhanden ist (Bei neuer Gruppe, wo die Daten nicht von der Standardgruppe kopiert werden)
                if (!Directory.Exists(_strXmlPath))
                    Directory.CreateDirectory(Path.GetDirectoryName(_strXmlPath));
                string newXmlPath = Path.GetDirectoryName(_strXmlPath) + "\\" + Path.GetFileNameWithoutExtension(_strXmlPath) + "_new.xml";
                System.Text.UnicodeEncoding objXMLEncoder = new System.Text.UnicodeEncoding();
                XmlTextWriter objXMLFile = new XmlTextWriter(newXmlPath, objXMLEncoder);

                objXMLFile.Formatting = Formatting.Indented;
                objXMLFile.Indentation = 4;
                //Dokument erstellen
                objXMLFile.WriteStartDocument();
                //Generate XML Header
                objXMLFile.WriteStartElement("WebDavClient");

                foreach (WebDAVClientProfile entry in listProfiles)
                {

                    objXMLFile.WriteStartElement("WebDAVClientProfile");
                    objXMLFile.WriteAttributeString("ProfileName", entry.ProfileName);
                    objXMLFile.WriteAttributeString("DavServer", entry.DavServer);
                    objXMLFile.WriteAttributeString("DavServerPath", entry.DavServerPath);
                    objXMLFile.WriteAttributeString("DavUser", entry.DavUser);
                    objXMLFile.WriteAttributeString("DavPass",  new Encryption().encrypt(entry.DavPass));
                    objXMLFile.WriteAttributeString("LocalPath", entry.LocalPath);
                    objXMLFile.WriteAttributeString("OverrideItems", entry.OverrideItems.ToString());

                    objXMLFile.WriteEndElement();
                }


                // Endelement für WebDavClient
                objXMLFile.WriteEndElement();
                objXMLFile.Close();

                File.Copy(newXmlPath, _strXmlPath, true);
                File.Delete(newXmlPath);

                Debug.WriteLine(this.GetType().ToString() + " - " + System.Reflection.MethodBase.GetCurrentMethod() + ": End");

            }
            catch (Exception ex)
            {
                Debug.WriteLine(this.GetType().ToString() + " - " + System.Reflection.MethodBase.GetCurrentMethod() + ": " + ex.Message);
                throw new Exception("Fehler beim Speichern der XML Datei: " + ex.Message);
            }

        }
    }
}
