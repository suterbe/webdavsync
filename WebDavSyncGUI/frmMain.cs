using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WebDavSync;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Threading;
using System.Diagnostics;
using WebDavSyncGUI.Logic;


namespace WebDavSyncGUI
{
    public partial class frmMain : Form
    {

        public frmMain()
        {
            InitializeComponent();
            this.Text = "WebDavSync Client BETA (" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + ")";
            _profileHelper = new ProfileHelper(new System.IO.DirectoryInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).Parent.FullName + "\\WebDavSync.xml");
            treeViewProfiles.ImageList = imageListMain32;

            loadTree();
        }


        private ProfileHelper _profileHelper;
        private Thread _thWebDavDownload = null;


        private const string WEBDAVSERVER = "https://elearning.hslu.ch";
        private bool _syncStarted = false;

        public delegate void ProfileLogSetText(string data);
        public delegate void ProfileFinished(WebDavClientHelper.SyncStatus success);

        private Queue<WebDavClientProfileNode> _SyncNodeList = new Queue<WebDavClientProfileNode>();
        private WebDavClientProfileNode _currentSyncNode = null;



        //TODO: TrayIcon




        #region ButtonEvents

        private void btnSavePrifole_Click(object sender, EventArgs e)
        {
            WebDAVClientProfile currentprofile = getCurrentProfile();
            List<WebDAVClientProfile> profileList = getProfileListTree();
            bool entryFound = false;
            for (int i = 0; i < profileList.Count; i++)
            {
                if (profileList[i].ProfileName.Equals(currentprofile.ProfileName))
                {
                    profileList[i] = currentprofile;
                    entryFound = true;
                }
            }
            if (!entryFound)
            {
                profileList.Add(currentprofile);
            }


            _profileHelper.saveProfiles(profileList);

            loadTree();
        }

        private void btnSyncStart_Click(object sender, EventArgs e)
        {
            txtProfileLog.Clear();
            try
            {
                _SyncNodeList.Enqueue((WebDavClientProfileNode)treeViewProfiles.SelectedNode);
                syncProfile();
            }
            catch (Exception ex)
            {
                ProfileLogSetData("ERROR: Bitte Profil auswählen " + Environment.NewLine + ex.Message);
            }

        }

        private void btnSyncStop_Click(object sender, EventArgs e)
        {

            //btnSyncStart.Enabled = true;
            
            if (_thWebDavDownload != null)
            {
                _thWebDavDownload.Abort();
                while (true)
                {
                    if (_thWebDavDownload.ThreadState == System.Threading.ThreadState.Stopped)
                    {
                        _syncStarted = false;
                        break;
                    }
                    Application.DoEvents();
                }
            }
            try
            {
                ProfileStateSetData(WebDavClientHelper.SyncStatus.SyncStopped);
            }
            catch (Exception ex)
            {
            }


        }
        #endregion


        #region Sync / Thread Logik

        private void syncProfile()
        {

            if (_SyncNodeList.Count > 0)
            {
                _currentSyncNode = _SyncNodeList.Dequeue();

                if (_syncStarted == false)
                {
                    ProfileStateSetData(WebDavClientHelper.SyncStatus.SyncStarted);
                    ParameterizedThreadStart thsWebDavDownload = new ParameterizedThreadStart(downloadContent);
                    _thWebDavDownload = new Thread(thsWebDavDownload);
                    _thWebDavDownload.Start(_currentSyncNode.WebDavClientProfileItem);
                    _syncStarted = true;
                    //btnSyncStart.Enabled = false;
                }
                else
                {
                    ProfileLogSetData("ERROR: Es ist bereits ein Download gestartet!");
                }



            }

            Application.DoEvents();
        }


        /// <summary>
        /// Synchronisiert alle Profile
        /// </summary>
        private void syncAllProfiles()
        {

            foreach (TreeNode tn in treeViewProfiles.TopNode.Nodes)
            {
                _SyncNodeList.Enqueue((WebDavClientProfileNode)tn);
            }
            syncProfile();
        }

        /// <summary>
        /// Lädt anhand von Profildaten den gesammten WebdavContent herunger
        /// Methode welche in neuem Thread aufgerufen wird
        /// </summary>
        /// <param name="profile">WebDavProfile</param>
        private void downloadContent(object profile)
        {
            WebDavClientHelper davHelper = new WebDavClientHelper((WebDAVClientProfile)profile);
            davHelper.LogData += new LogDataHandler(davHelper_LogData);
            davHelper.SyncProfileEnd += new SyncEnd(davHelper_SyncProfileEnd);
            try
            {
                davHelper.downloadWebDavContent();
            }
            catch (Exception ex)
            {
                ProfileLogSetData("ERROR: " + ex.Message);
                davHelper_SyncProfileEnd(this, new SyncStatusEventArgs(WebDavSync.WebDavClientHelper.SyncStatus.SyncFinished));
            }

        }

        private void davHelper_SyncProfileEnd(object sender, SyncStatusEventArgs e)
        {

            ProfileStateSetData(e.Status);
            if (_SyncNodeList.Count > 0)
                syncProfile();
        }

        /// <summary>
        /// Methode welche von Thread aufgerufen wird um Einträge ins Log zu schreiben
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void davHelper_LogData(object sender, WebDavClientEventArgs e)
        {
            ProfileLogSetData(e.Log);
        }


        #endregion


        #region Interne Klassen
        internal class WebDavClientProfileNode : TreeNode
        {
            private WebDAVClientProfile _profile;

            public WebDavClientProfileNode(WebDAVClientProfile profile)
            {
                _profile = profile;
                setText();
            }

            public WebDAVClientProfile WebDavClientProfileItem
            {
                get { return _profile; }
                set { }
            }
            private void setText()
            {
                this.Text = _profile.ProfileName;
                this.ImageKey = "folder_start.ico";
                this.SelectedImageKey = "folder_start.ico";

            }

        }

        #endregion

        #region Tree Operationen

        private void clearTreeDetailInforamtion()
        {
            txtProfileName.Text = "";
            txtProfileDavServer.Text = "";
            txtProfileDavPath.Text = "";
            txtProfileDavUser.Text = "";
            txtProfileDavPassword.Text = "";
            txtProfileLocalPath.Text = "";
        }

        private void loadTree()
        {
            treeViewProfiles.BeginUpdate();

            treeViewProfiles.Nodes.Clear();
            TreeNode tnRoot = new TreeNode();
            tnRoot.Text = "WebDavSync";
            tnRoot.ImageKey = "download.ico";
            tnRoot.SelectedImageKey = "download.ico";
            treeViewProfiles.Nodes.Add(tnRoot);
            foreach (WebDAVClientProfile profile in getProfileListXML())
            {
                tnRoot.Nodes.Add(new WebDavClientProfileNode(profile));
            }
            treeViewProfiles.ExpandAll();
            treeViewProfiles.EndUpdate();
        }

        #endregion

        #region Profile Operationen
        private List<WebDAVClientProfile> getProfileListXML()
        {

            return _profileHelper.getProfiles();
        }

        private List<WebDAVClientProfile> getProfileListTree()
        {
            List<WebDAVClientProfile> profileList = new List<WebDAVClientProfile>();
            foreach (TreeNode tn in treeViewProfiles.TopNode.Nodes)
            {
                try
                {
                    WebDAVClientProfile profile = ((WebDavClientProfileNode)tn).WebDavClientProfileItem;
                    profileList.Add(profile);
                }
                catch
                {
                }
            }
            return profileList;
        }

        private void loadProfileData(WebDAVClientProfile profile)
        {
            txtProfileName.Text = profile.ProfileName;
            txtProfileDavServer.Text = profile.DavServer;
            txtProfileDavPath.Text = profile.DavServerPath;
            txtProfileDavUser.Text = profile.DavUser;
            txtProfileDavPassword.Text = profile.DavPass;
            txtProfileLocalPath.Text = profile.LocalPath;
        }

        private WebDAVClientProfile getCurrentProfile()
        {
            WebDAVClientProfile profile = new WebDAVClientProfile();
            profile.ProfileName = txtProfileName.Text;
            profile.DavServerPath = txtProfileDavPath.Text;
            profile.DavServer = txtProfileDavServer.Text;
            profile.DavUser = txtProfileDavUser.Text;
            profile.DavPass = txtProfileDavPassword.Text;
            profile.LocalPath = txtProfileLocalPath.Text;
            return profile;
        }

        #endregion





        private void treeViewProfiles_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                loadProfileData(((WebDavClientProfileNode)((TreeView)sender).SelectedNode).WebDavClientProfileItem);
            }
            catch
            {
                clearTreeDetailInforamtion();
            }

        }

        private void treeViewProfiles_Click(object sender, EventArgs e)
        {
            try
            {
                System.Windows.Forms.MouseEventArgs userEvent = (System.Windows.Forms.MouseEventArgs)e;
                if (userEvent.Button == MouseButtons.Right)
                {
                    TreeNode n = treeViewProfiles.GetNodeAt(userEvent.X, userEvent.Y);
                    if (n.FullPath.StartsWith("WebDavSync\\"))
                    {
                        if (n != null && n.GetType() == typeof(WebDavClientProfileNode))
                        {
                            treeViewProfiles.SelectedNode = n;

                            ToolStripMenuItemTreeProfilAdd.Enabled = true;
                            ToolStripMenuItemTreeProfilDel.Enabled = true;
                            ToolStripMenuItemTreeProfilSync.Enabled = true;
                        }
                        else
                        {
                            ToolStripMenuItemTreeProfilAdd.Enabled = true;
                            ToolStripMenuItemTreeProfilDel.Enabled = false;
                            ToolStripMenuItemTreeProfilSync.Enabled = false;
                            ToolStripMenuItemTreeProfilSyncAll.Enabled = true;
                        }
                    }
                    else
                    {
                        ToolStripMenuItemTreeProfilAdd.Enabled = true;
                        ToolStripMenuItemTreeProfilDel.Enabled = false;
                        ToolStripMenuItemTreeProfilSync.Enabled = false;
                        ToolStripMenuItemTreeProfilSyncAll.Enabled = true;
                    }

                    contextMenuStripProfile.Show(treeViewProfiles, userEvent.X, userEvent.Y);
                    //con.Show(treeViewGruppenCustomScan, 

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(this.GetType().ToString() + " - " + System.Reflection.MethodBase.GetCurrentMethod() + "Error : " + ex.Message);
            }

        }

        private void ToolStripMenuItemTreeProfilAdd_Click(object sender, EventArgs e)
        {
            clearTreeDetailInforamtion();
            txtProfileDavServer.Text = WEBDAVSERVER;
        }

        private void ToolStripMenuItemTreeProfilDel_Click(object sender, EventArgs e)
        {
            treeViewProfiles.SelectedNode.Remove();
            _profileHelper.saveProfiles(getProfileListTree());

        }

        private void ToolStripMenuItemTreeProfilSyncAll_Click(object sender, EventArgs e)
        {
            syncAllProfiles();
        }

        private void cmdSyncAll_Click(object sender, EventArgs e)
        {
            syncAllProfiles();
        }

        private void ToolStripMenuItemTreeProfilSync_Click(object sender, EventArgs e)
        {
            syncProfile();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutDialog().Show();
        }

        private void txtProfileLocalPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dglFolderOpen = new FolderBrowserDialog();
            dglFolderOpen.RootFolder = Environment.SpecialFolder.MyComputer;
            dglFolderOpen.Description = "Bitte wählen Sie einen Pfad aus";
            dglFolderOpen.ShowDialog();
            if (dglFolderOpen.SelectedPath != string.Empty)
            {

                txtProfileLocalPath.Text = dglFolderOpen.SelectedPath;
            }

        }

        private void txtProfileLocalPath_Enter(object sender, EventArgs e)
        {
            txtProfileLocalPath_Click(sender, e);
        }

        /// <summary>
        /// Beendet Programm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void beendenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        #region Threading SetLogText

        private void ProfileLogSetData(string data)
        {
            if (txtProfileLog.InvokeRequired)
                txtProfileLog.Invoke(new ProfileLogSetText(ProfileLogSetCallback),
                new object[] { data });
            else
                ProfileLogSetCallback(data); //call directly
        }

        private void ProfileLogSetCallback(string data)
        {
            txtProfileLog.Text += data + Environment.NewLine;
            txtProfileLog.SelectAll();
            txtProfileLog.ScrollToCaret();
        }


        private void ProfileStateSetData(WebDavSync.WebDavClientHelper.SyncStatus status)
        {
            if (treeViewProfiles.InvokeRequired)
                treeViewProfiles.Invoke(new ProfileFinished(ProfileStateSetCallback), new object[] { status });
            else
                ProfileStateSetCallback(status);
        }

        /// <summary>
        /// Wird beim Downloadende ausgelöst
        /// </summary>
        /// <param name="status">Status ob erfolgreich, True ist ok</param>
        private void ProfileStateSetCallback(WebDavClientHelper.SyncStatus status)
        {
            _syncStarted = false;
            //btnSyncStart.Enabled = true;
            switch (status)
            {
                case WebDavSync.WebDavClientHelper.SyncStatus.SyncFinished:
                    if (_currentSyncNode != null)
                    {
                        _currentSyncNode.SelectedImageKey = "folder_finish.ico";
                        _currentSyncNode.ImageKey = "folder_finish.ico";
                    }
                    break;

                case WebDavSync.WebDavClientHelper.SyncStatus.SyncStopped:
                    if (_currentSyncNode != null)
                    {
                        _currentSyncNode.SelectedImageKey = "folder_start.ico";
                        _currentSyncNode.ImageKey = "folder_start.ico";
                    }
                    break;

                case WebDavSync.WebDavClientHelper.SyncStatus.SyncStarted:
                    if (_currentSyncNode != null)
                    {
                        _currentSyncNode.SelectedImageKey = "folder_sync.ico";
                        _currentSyncNode.ImageKey = "folder_sync.ico";
                    }
                    break;
                //case 0:


            }
        }

        #endregion

    }
}
