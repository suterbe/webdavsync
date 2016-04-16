namespace WebDavSyncGUI
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.panelMain = new System.Windows.Forms.Panel();
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.treeViewProfiles = new System.Windows.Forms.TreeView();
            this.tableLayoutPanelProfileMain = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanelProfileDetails = new System.Windows.Forms.TableLayoutPanel();
            this.txtProfileDavServer = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtProfileName = new System.Windows.Forms.TextBox();
            this.txtProfileDavPath = new System.Windows.Forms.TextBox();
            this.txtProfileLocalPath = new System.Windows.Forms.TextBox();
            this.txtProfileDavUser = new System.Windows.Forms.TextBox();
            this.txtProfileDavPassword = new System.Windows.Forms.TextBox();
            this.tableLayoutPanelProfileActions = new System.Windows.Forms.TableLayoutPanel();
            this.btnSyncStop = new System.Windows.Forms.Button();
            this.btnSyncStart = new System.Windows.Forms.Button();
            this.btnSavePrifole = new System.Windows.Forms.Button();
            this.cmdSyncAll = new System.Windows.Forms.Button();
            this.txtProfileLog = new System.Windows.Forms.TextBox();
            this.menuStripDatei = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItemDatei = new System.Windows.Forms.ToolStripMenuItem();
            this.beendenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripProfile = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ToolStripMenuItemTreeProfilAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemTreeProfilDel = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemTreeProfilSync = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemTreeProfilSyncAll = new System.Windows.Forms.ToolStripMenuItem();
            this.imageListMain32 = new System.Windows.Forms.ImageList(this.components);
            this.panelMain.SuspendLayout();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            this.tableLayoutPanelProfileMain.SuspendLayout();
            this.tableLayoutPanelProfileDetails.SuspendLayout();
            this.tableLayoutPanelProfileActions.SuspendLayout();
            this.menuStripDatei.SuspendLayout();
            this.contextMenuStripProfile.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.splitContainerMain);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 24);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(810, 486);
            this.panelMain.TabIndex = 0;
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.Location = new System.Drawing.Point(0, 0);
            this.splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.treeViewProfiles);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.tableLayoutPanelProfileMain);
            this.splitContainerMain.Size = new System.Drawing.Size(810, 486);
            this.splitContainerMain.SplitterDistance = 270;
            this.splitContainerMain.TabIndex = 0;
            // 
            // treeViewProfiles
            // 
            this.treeViewProfiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewProfiles.Location = new System.Drawing.Point(0, 0);
            this.treeViewProfiles.Name = "treeViewProfiles";
            this.treeViewProfiles.Size = new System.Drawing.Size(270, 486);
            this.treeViewProfiles.TabIndex = 0;
            this.treeViewProfiles.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewProfiles_AfterSelect);
            this.treeViewProfiles.Click += new System.EventHandler(this.treeViewProfiles_Click);
            // 
            // tableLayoutPanelProfileMain
            // 
            this.tableLayoutPanelProfileMain.ColumnCount = 1;
            this.tableLayoutPanelProfileMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelProfileMain.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanelProfileMain.Controls.Add(this.tableLayoutPanelProfileDetails, 0, 1);
            this.tableLayoutPanelProfileMain.Controls.Add(this.tableLayoutPanelProfileActions, 0, 3);
            this.tableLayoutPanelProfileMain.Controls.Add(this.txtProfileLog, 0, 2);
            this.tableLayoutPanelProfileMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelProfileMain.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelProfileMain.Name = "tableLayoutPanelProfileMain";
            this.tableLayoutPanelProfileMain.RowCount = 4;
            this.tableLayoutPanelProfileMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanelProfileMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanelProfileMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelProfileMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanelProfileMain.Size = new System.Drawing.Size(536, 486);
            this.tableLayoutPanelProfileMain.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(530, 50);
            this.label1.TabIndex = 0;
            this.label1.Text = "HSLU - ILIAS Downloader";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tableLayoutPanelProfileDetails
            // 
            this.tableLayoutPanelProfileDetails.ColumnCount = 2;
            this.tableLayoutPanelProfileDetails.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanelProfileDetails.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelProfileDetails.Controls.Add(this.txtProfileDavServer, 1, 1);
            this.tableLayoutPanelProfileDetails.Controls.Add(this.label7, 0, 1);
            this.tableLayoutPanelProfileDetails.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanelProfileDetails.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanelProfileDetails.Controls.Add(this.label4, 0, 3);
            this.tableLayoutPanelProfileDetails.Controls.Add(this.label5, 0, 4);
            this.tableLayoutPanelProfileDetails.Controls.Add(this.label6, 0, 5);
            this.tableLayoutPanelProfileDetails.Controls.Add(this.txtProfileName, 1, 0);
            this.tableLayoutPanelProfileDetails.Controls.Add(this.txtProfileDavPath, 1, 2);
            this.tableLayoutPanelProfileDetails.Controls.Add(this.txtProfileLocalPath, 1, 3);
            this.tableLayoutPanelProfileDetails.Controls.Add(this.txtProfileDavUser, 1, 4);
            this.tableLayoutPanelProfileDetails.Controls.Add(this.txtProfileDavPassword, 1, 5);
            this.tableLayoutPanelProfileDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelProfileDetails.Location = new System.Drawing.Point(3, 53);
            this.tableLayoutPanelProfileDetails.Name = "tableLayoutPanelProfileDetails";
            this.tableLayoutPanelProfileDetails.RowCount = 7;
            this.tableLayoutPanelProfileDetails.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanelProfileDetails.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanelProfileDetails.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanelProfileDetails.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanelProfileDetails.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanelProfileDetails.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanelProfileDetails.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanelProfileDetails.Size = new System.Drawing.Size(530, 194);
            this.tableLayoutPanelProfileDetails.TabIndex = 1;
            // 
            // txtProfileDavServer
            // 
            this.txtProfileDavServer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtProfileDavServer.Location = new System.Drawing.Point(203, 33);
            this.txtProfileDavServer.Name = "txtProfileDavServer";
            this.txtProfileDavServer.Size = new System.Drawing.Size(324, 20);
            this.txtProfileDavServer.TabIndex = 2;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label7.Location = new System.Drawing.Point(3, 30);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(194, 30);
            this.label7.TabIndex = 10;
            this.label7.Text = "WebDavServer";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(194, 30);
            this.label2.TabIndex = 0;
            this.label2.Text = "Profilname";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(3, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(194, 30);
            this.label3.TabIndex = 1;
            this.label3.Text = "WebDavServerPfad";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Location = new System.Drawing.Point(3, 90);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(194, 30);
            this.label4.TabIndex = 2;
            this.label4.Text = "Lokaler Pfad";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Location = new System.Drawing.Point(3, 120);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(194, 30);
            this.label5.TabIndex = 3;
            this.label5.Text = "WebDavBenutzer";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label6.Location = new System.Drawing.Point(3, 150);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(194, 30);
            this.label6.TabIndex = 4;
            this.label6.Text = "WebDavPasswort";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtProfileName
            // 
            this.txtProfileName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtProfileName.Location = new System.Drawing.Point(203, 3);
            this.txtProfileName.Name = "txtProfileName";
            this.txtProfileName.Size = new System.Drawing.Size(324, 20);
            this.txtProfileName.TabIndex = 1;
            // 
            // txtProfileDavPath
            // 
            this.txtProfileDavPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtProfileDavPath.Location = new System.Drawing.Point(203, 63);
            this.txtProfileDavPath.Name = "txtProfileDavPath";
            this.txtProfileDavPath.Size = new System.Drawing.Size(324, 20);
            this.txtProfileDavPath.TabIndex = 3;
            // 
            // txtProfileLocalPath
            // 
            this.txtProfileLocalPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtProfileLocalPath.Location = new System.Drawing.Point(203, 93);
            this.txtProfileLocalPath.Name = "txtProfileLocalPath";
            this.txtProfileLocalPath.ReadOnly = true;
            this.txtProfileLocalPath.Size = new System.Drawing.Size(324, 20);
            this.txtProfileLocalPath.TabIndex = 4;
            this.txtProfileLocalPath.Click += new System.EventHandler(this.txtProfileLocalPath_Click);
            this.txtProfileLocalPath.Enter += new System.EventHandler(this.txtProfileLocalPath_Enter);
            // 
            // txtProfileDavUser
            // 
            this.txtProfileDavUser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtProfileDavUser.Location = new System.Drawing.Point(203, 123);
            this.txtProfileDavUser.Name = "txtProfileDavUser";
            this.txtProfileDavUser.Size = new System.Drawing.Size(324, 20);
            this.txtProfileDavUser.TabIndex = 5;
            // 
            // txtProfileDavPassword
            // 
            this.txtProfileDavPassword.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtProfileDavPassword.Location = new System.Drawing.Point(203, 153);
            this.txtProfileDavPassword.Name = "txtProfileDavPassword";
            this.txtProfileDavPassword.PasswordChar = '*';
            this.txtProfileDavPassword.Size = new System.Drawing.Size(324, 20);
            this.txtProfileDavPassword.TabIndex = 6;
            // 
            // tableLayoutPanelProfileActions
            // 
            this.tableLayoutPanelProfileActions.ColumnCount = 4;
            this.tableLayoutPanelProfileActions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanelProfileActions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanelProfileActions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanelProfileActions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanelProfileActions.Controls.Add(this.btnSyncStop, 2, 0);
            this.tableLayoutPanelProfileActions.Controls.Add(this.btnSyncStart, 1, 0);
            this.tableLayoutPanelProfileActions.Controls.Add(this.btnSavePrifole, 3, 0);
            this.tableLayoutPanelProfileActions.Controls.Add(this.cmdSyncAll, 0, 0);
            this.tableLayoutPanelProfileActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelProfileActions.Location = new System.Drawing.Point(3, 439);
            this.tableLayoutPanelProfileActions.Name = "tableLayoutPanelProfileActions";
            this.tableLayoutPanelProfileActions.RowCount = 1;
            this.tableLayoutPanelProfileActions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelProfileActions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            this.tableLayoutPanelProfileActions.Size = new System.Drawing.Size(530, 44);
            this.tableLayoutPanelProfileActions.TabIndex = 2;
            // 
            // btnSyncStop
            // 
            this.btnSyncStop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSyncStop.Location = new System.Drawing.Point(267, 3);
            this.btnSyncStop.Name = "btnSyncStop";
            this.btnSyncStop.Size = new System.Drawing.Size(126, 38);
            this.btnSyncStop.TabIndex = 1;
            this.btnSyncStop.Text = "Sync Stoppen";
            this.btnSyncStop.UseVisualStyleBackColor = true;
            this.btnSyncStop.Click += new System.EventHandler(this.btnSyncStop_Click);
            // 
            // btnSyncStart
            // 
            this.btnSyncStart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSyncStart.Location = new System.Drawing.Point(135, 3);
            this.btnSyncStart.Name = "btnSyncStart";
            this.btnSyncStart.Size = new System.Drawing.Size(126, 38);
            this.btnSyncStart.TabIndex = 0;
            this.btnSyncStart.Text = "Sync Starten";
            this.btnSyncStart.UseVisualStyleBackColor = true;
            this.btnSyncStart.Click += new System.EventHandler(this.btnSyncStart_Click);
            // 
            // btnSavePrifole
            // 
            this.btnSavePrifole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSavePrifole.Location = new System.Drawing.Point(399, 3);
            this.btnSavePrifole.Name = "btnSavePrifole";
            this.btnSavePrifole.Size = new System.Drawing.Size(128, 38);
            this.btnSavePrifole.TabIndex = 1;
            this.btnSavePrifole.Text = "Profil speichern";
            this.btnSavePrifole.UseVisualStyleBackColor = true;
            this.btnSavePrifole.Click += new System.EventHandler(this.btnSavePrifole_Click);
            // 
            // cmdSyncAll
            // 
            this.cmdSyncAll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmdSyncAll.Location = new System.Drawing.Point(3, 3);
            this.cmdSyncAll.Name = "cmdSyncAll";
            this.cmdSyncAll.Size = new System.Drawing.Size(126, 38);
            this.cmdSyncAll.TabIndex = 2;
            this.cmdSyncAll.Text = "Sync All";
            this.cmdSyncAll.UseVisualStyleBackColor = true;
            this.cmdSyncAll.Click += new System.EventHandler(this.cmdSyncAll_Click);
            // 
            // txtProfileLog
            // 
            this.txtProfileLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtProfileLog.Location = new System.Drawing.Point(3, 253);
            this.txtProfileLog.Multiline = true;
            this.txtProfileLog.Name = "txtProfileLog";
            this.txtProfileLog.ReadOnly = true;
            this.txtProfileLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtProfileLog.Size = new System.Drawing.Size(530, 180);
            this.txtProfileLog.TabIndex = 3;
            this.txtProfileLog.WordWrap = false;
            // 
            // menuStripDatei
            // 
            this.menuStripDatei.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemDatei,
            this.helpToolStripMenuItem});
            this.menuStripDatei.Location = new System.Drawing.Point(0, 0);
            this.menuStripDatei.Name = "menuStripDatei";
            this.menuStripDatei.Size = new System.Drawing.Size(810, 24);
            this.menuStripDatei.TabIndex = 1;
            this.menuStripDatei.Text = "menuStrip1";
            // 
            // toolStripMenuItemDatei
            // 
            this.toolStripMenuItemDatei.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.beendenToolStripMenuItem});
            this.toolStripMenuItemDatei.Name = "toolStripMenuItemDatei";
            this.toolStripMenuItemDatei.Size = new System.Drawing.Size(44, 20);
            this.toolStripMenuItemDatei.Text = "Datei";
            // 
            // beendenToolStripMenuItem
            // 
            this.beendenToolStripMenuItem.Name = "beendenToolStripMenuItem";
            this.beendenToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.beendenToolStripMenuItem.Text = "Beenden";
            this.beendenToolStripMenuItem.Click += new System.EventHandler(this.beendenToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator5,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(123, 6);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
            this.aboutToolStripMenuItem.Text = "&About...";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // contextMenuStripProfile
            // 
            this.contextMenuStripProfile.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItemTreeProfilAdd,
            this.ToolStripMenuItemTreeProfilDel,
            this.ToolStripMenuItemTreeProfilSync,
            this.ToolStripMenuItemTreeProfilSyncAll});
            this.contextMenuStripProfile.Name = "contextMenuStripProfile";
            this.contextMenuStripProfile.Size = new System.Drawing.Size(214, 92);
            // 
            // ToolStripMenuItemTreeProfilAdd
            // 
            this.ToolStripMenuItemTreeProfilAdd.Name = "ToolStripMenuItemTreeProfilAdd";
            this.ToolStripMenuItemTreeProfilAdd.Size = new System.Drawing.Size(213, 22);
            this.ToolStripMenuItemTreeProfilAdd.Text = "Profil hinzufügen";
            this.ToolStripMenuItemTreeProfilAdd.Click += new System.EventHandler(this.ToolStripMenuItemTreeProfilAdd_Click);
            // 
            // ToolStripMenuItemTreeProfilDel
            // 
            this.ToolStripMenuItemTreeProfilDel.Name = "ToolStripMenuItemTreeProfilDel";
            this.ToolStripMenuItemTreeProfilDel.Size = new System.Drawing.Size(213, 22);
            this.ToolStripMenuItemTreeProfilDel.Text = "Profil löschen";
            this.ToolStripMenuItemTreeProfilDel.Click += new System.EventHandler(this.ToolStripMenuItemTreeProfilDel_Click);
            // 
            // ToolStripMenuItemTreeProfilSync
            // 
            this.ToolStripMenuItemTreeProfilSync.Name = "ToolStripMenuItemTreeProfilSync";
            this.ToolStripMenuItemTreeProfilSync.Size = new System.Drawing.Size(213, 22);
            this.ToolStripMenuItemTreeProfilSync.Text = "Profil synchronisieren";
            this.ToolStripMenuItemTreeProfilSync.Click += new System.EventHandler(this.ToolStripMenuItemTreeProfilSync_Click);
            // 
            // ToolStripMenuItemTreeProfilSyncAll
            // 
            this.ToolStripMenuItemTreeProfilSyncAll.Name = "ToolStripMenuItemTreeProfilSyncAll";
            this.ToolStripMenuItemTreeProfilSyncAll.Size = new System.Drawing.Size(213, 22);
            this.ToolStripMenuItemTreeProfilSyncAll.Text = "Alle Profile synchronisieren";
            this.ToolStripMenuItemTreeProfilSyncAll.Click += new System.EventHandler(this.ToolStripMenuItemTreeProfilSyncAll_Click);
            // 
            // imageListMain32
            // 
            this.imageListMain32.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListMain32.ImageStream")));
            this.imageListMain32.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListMain32.Images.SetKeyName(0, "sync_stop.ico");
            this.imageListMain32.Images.SetKeyName(1, "folder.ico");
            this.imageListMain32.Images.SetKeyName(2, "folder_down.ico");
            this.imageListMain32.Images.SetKeyName(3, "sync_start.ico");
            this.imageListMain32.Images.SetKeyName(4, "home.ico");
            this.imageListMain32.Images.SetKeyName(5, "Download.ico");
            this.imageListMain32.Images.SetKeyName(6, "folder_finish.ico");
            this.imageListMain32.Images.SetKeyName(7, "folder_start.ico");
            this.imageListMain32.Images.SetKeyName(8, "folder_sync.ico");
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(810, 510);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.menuStripDatei);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStripDatei;
            this.Name = "frmMain";
            this.panelMain.ResumeLayout(false);
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            this.splitContainerMain.ResumeLayout(false);
            this.tableLayoutPanelProfileMain.ResumeLayout(false);
            this.tableLayoutPanelProfileMain.PerformLayout();
            this.tableLayoutPanelProfileDetails.ResumeLayout(false);
            this.tableLayoutPanelProfileDetails.PerformLayout();
            this.tableLayoutPanelProfileActions.ResumeLayout(false);
            this.menuStripDatei.ResumeLayout(false);
            this.menuStripDatei.PerformLayout();
            this.contextMenuStripProfile.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.TreeView treeViewProfiles;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelProfileMain;
        private System.Windows.Forms.MenuStrip menuStripDatei;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemDatei;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelProfileDetails;
        private System.Windows.Forms.ToolStripMenuItem beendenToolStripMenuItem;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtProfileName;
        private System.Windows.Forms.TextBox txtProfileDavPath;
        private System.Windows.Forms.TextBox txtProfileLocalPath;
        private System.Windows.Forms.TextBox txtProfileDavUser;
        private System.Windows.Forms.TextBox txtProfileDavPassword;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelProfileActions;
        private System.Windows.Forms.Button btnSyncStart;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripProfile;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemTreeProfilAdd;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemTreeProfilDel;
        private System.Windows.Forms.Button btnSavePrifole;
        private System.Windows.Forms.TextBox txtProfileDavServer;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ImageList imageListMain32;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemTreeProfilSync;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemTreeProfilSyncAll;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.TextBox txtProfileLog;
        private System.Windows.Forms.Button btnSyncStop;
        private System.Windows.Forms.Button cmdSyncAll;
    }
}

