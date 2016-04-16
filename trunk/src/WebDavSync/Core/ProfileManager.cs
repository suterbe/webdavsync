using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using WebDavSync.Core;

namespace WebDavSync.Core {

    public class ProfileManager : IProfileManager {

        public ProfileManager(string profilePath)
            : this(profilePath, new CryptoService()) {
        }

        public ProfileManager(string profilePath, ICryptoService cryptoService) {
            this.ProfilePath = profilePath;
            this.CryptoService = cryptoService;
        }

        /// <summary>
        /// Pfad zur Profil-Datei.
        /// </summary>
        private string ProfilePath { get; set; }

        private ICryptoService CryptoService { get; set; }

        public List<WebDAVClientProfile> GetProfiles() {
            List<WebDAVClientProfile> listProfile = new List<WebDAVClientProfile>();

            try {
                XPathDocument doc = new XPathDocument(this.ProfilePath);
                XPathNavigator nav = doc.CreateNavigator();

                XPathNodeIterator xnodes = nav.Select("/WebDavClient");
                xnodes.Current.MoveToFirstChild();
                while (xnodes.MoveNext()) {
                    xnodes.Current.MoveToFirstChild();
                    do {
                        WebDAVClientProfile entry = new WebDAVClientProfile();
                        entry.ProfileName = xnodes.Current.GetAttribute("ProfileName", "").ToString();
                        entry.DavServer = xnodes.Current.GetAttribute("DavServer", "").ToString();
                        entry.DavServerPath = xnodes.Current.GetAttribute("DavServerPath", "").ToString();
                        entry.DavUser = xnodes.Current.GetAttribute("DavUser", "").ToString();
                        entry.DavPass = this.CryptoService.Decrypt(xnodes.Current.GetAttribute("DavPass", "").ToString());
                        entry.LocalPath = xnodes.Current.GetAttribute("LocalPath", "").ToString();
                        entry.OverrideItems = Convert.ToBoolean(xnodes.Current.GetAttribute("OverrideItems", "").ToString());
                        listProfile.Add(entry);

                    } while (xnodes.Current.MoveToNext());
                }
            } catch (Exception ex) {
                Debug.WriteLine("Profile  - " + System.Reflection.MethodBase.GetCurrentMethod() + ": Exeption " + ex.Message);
            }
            return listProfile;
        }

        public void SaveProfiles(List<WebDAVClientProfile> listProfiles) {

            if (listProfiles.Count > 0) {
                try {
                    //Erstellt den Pfad wenn dieser nicht vorhanden ist (Bei neuer Gruppe, wo die Daten nicht von der Standardgruppe kopiert werden)
                    if (!Directory.Exists(this.ProfilePath))
                        Directory.CreateDirectory(Path.GetDirectoryName(this.ProfilePath));
                    string newXmlPath = Path.GetDirectoryName(this.ProfilePath) + "\\" + Path.GetFileNameWithoutExtension(ProfilePath) + "_new.xml";
                    System.Text.UnicodeEncoding objXMLEncoder = new System.Text.UnicodeEncoding();
                    XmlTextWriter objXMLFile = new XmlTextWriter(newXmlPath, objXMLEncoder);

                    objXMLFile.Formatting = Formatting.Indented;
                    objXMLFile.Indentation = 4;
                    //Dokument erstellen
                    objXMLFile.WriteStartDocument();
                    //Generate XML Header
                    objXMLFile.WriteStartElement("WebDavClient");

                    foreach (WebDAVClientProfile entry in listProfiles) {

                        objXMLFile.WriteStartElement("WebDAVClientProfile");
                        objXMLFile.WriteAttributeString("ProfileName", entry.ProfileName);
                        objXMLFile.WriteAttributeString("DavServer", entry.DavServer);
                        objXMLFile.WriteAttributeString("DavServerPath", entry.DavServerPath);
                        objXMLFile.WriteAttributeString("DavUser", entry.DavUser);
                        objXMLFile.WriteAttributeString("DavPass", this.CryptoService.Encrypt(entry.DavPass));
                        objXMLFile.WriteAttributeString("LocalPath", entry.LocalPath);
                        objXMLFile.WriteAttributeString("OverrideItems", entry.OverrideItems.ToString());

                        objXMLFile.WriteEndElement();
                    }

                    // Endelement für WebDavClient
                    objXMLFile.WriteEndElement();
                    objXMLFile.Close();

                    File.Copy(newXmlPath, ProfilePath, true);
                    File.Delete(newXmlPath);
                } catch (Exception ex) {
                    Debug.WriteLine(this.GetType().ToString() + " - " + System.Reflection.MethodBase.GetCurrentMethod() + ": " + ex.Message);
                    throw new Exception("Fehler beim Speichern der XML Datei: " + ex.Message);
                }
            }
        }
    }
}
