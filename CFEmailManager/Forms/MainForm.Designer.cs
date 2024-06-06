namespace CFEmailManager.Forms
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.tscbAccount = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripTextBox1 = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.downloadEmailsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downloadEmailsAllAccountsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cancelDownloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.encryptSettingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tsslStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tvwEmailFolders = new System.Windows.Forms.TreeView();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.niNotify = new System.Windows.Forms.NotifyIcon(this.components);
            this.toolStripDropDownButton2 = new System.Windows.Forms.ToolStripDropDownButton();
            this.editAccountToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addAccountToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteAccountToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel2,
            this.tscbAccount,
            this.toolStripLabel1,
            this.toolStripTextBox1,
            this.toolStripDropDownButton1,
            this.toolStripDropDownButton2});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1204, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            this.toolStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.toolStrip1_ItemClicked);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(55, 22);
            this.toolStripLabel2.Text = "Account:";
            // 
            // tscbAccount
            // 
            this.tscbAccount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tscbAccount.Name = "tscbAccount";
            this.tscbAccount.Size = new System.Drawing.Size(220, 25);
            this.tscbAccount.SelectedIndexChanged += new System.EventHandler(this.tscbAccount_SelectedIndexChanged);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(45, 22);
            this.toolStripLabel1.Text = "Search:";
            // 
            // toolStripTextBox1
            // 
            this.toolStripTextBox1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.toolStripTextBox1.Name = "toolStripTextBox1";
            this.toolStripTextBox1.Size = new System.Drawing.Size(200, 25);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.downloadEmailsToolStripMenuItem,
            this.downloadEmailsAllAccountsToolStripMenuItem,
            this.cancelDownloadToolStripMenuItem,
            this.encryptSettingToolStripMenuItem});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(54, 22);
            this.toolStripDropDownButton1.Text = "File";
            // 
            // downloadEmailsToolStripMenuItem
            // 
            this.downloadEmailsToolStripMenuItem.Name = "downloadEmailsToolStripMenuItem";
            this.downloadEmailsToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.downloadEmailsToolStripMenuItem.Text = "Download emails";
            this.downloadEmailsToolStripMenuItem.Click += new System.EventHandler(this.downloadEmailsToolStripMenuItem_Click);
            // 
            // downloadEmailsAllAccountsToolStripMenuItem
            // 
            this.downloadEmailsAllAccountsToolStripMenuItem.Name = "downloadEmailsAllAccountsToolStripMenuItem";
            this.downloadEmailsAllAccountsToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.downloadEmailsAllAccountsToolStripMenuItem.Text = "Download emails (All accounts)";
            this.downloadEmailsAllAccountsToolStripMenuItem.Click += new System.EventHandler(this.downloadEmailsAllAccountsToolStripMenuItem_Click);
            // 
            // cancelDownloadToolStripMenuItem
            // 
            this.cancelDownloadToolStripMenuItem.Name = "cancelDownloadToolStripMenuItem";
            this.cancelDownloadToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.cancelDownloadToolStripMenuItem.Text = "Cancel download";
            this.cancelDownloadToolStripMenuItem.Visible = false;
            this.cancelDownloadToolStripMenuItem.Click += new System.EventHandler(this.cancelDownloadToolStripMenuItem_Click);
            // 
            // encryptSettingToolStripMenuItem
            // 
            this.encryptSettingToolStripMenuItem.Name = "encryptSettingToolStripMenuItem";
            this.encryptSettingToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.encryptSettingToolStripMenuItem.Text = "Encrypt setting";
            this.encryptSettingToolStripMenuItem.Click += new System.EventHandler(this.encryptSettingToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 631);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1204, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tsslStatus
            // 
            this.tsslStatus.Name = "tsslStatus";
            this.tsslStatus.Size = new System.Drawing.Size(118, 17);
            this.tsslStatus.Text = "toolStripStatusLabel1";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tvwEmailFolders);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(1204, 606);
            this.splitContainer1.SplitterDistance = 277;
            this.splitContainer1.TabIndex = 2;
            // 
            // tvwEmailFolders
            // 
            this.tvwEmailFolders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvwEmailFolders.Location = new System.Drawing.Point(0, 0);
            this.tvwEmailFolders.Name = "tvwEmailFolders";
            this.tvwEmailFolders.Size = new System.Drawing.Size(277, 606);
            this.tvwEmailFolders.TabIndex = 0;
            this.tvwEmailFolders.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvwEmails_AfterSelect);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.splitContainer2.Size = new System.Drawing.Size(923, 606);
            this.splitContainer2.SplitterDistance = 201;
            this.splitContainer2.TabIndex = 0;
            // 
            // niNotify
            // 
            this.niNotify.Text = "notifyIcon1";
            this.niNotify.Visible = true;
            this.niNotify.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.niNotify_MouseDoubleClick);
            // 
            // toolStripDropDownButton2
            // 
            this.toolStripDropDownButton2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addAccountToolStripMenuItem,
            this.editAccountToolStripMenuItem,
            this.deleteAccountToolStripMenuItem});
            this.toolStripDropDownButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton2.Image")));
            this.toolStripDropDownButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton2.Name = "toolStripDropDownButton2";
            this.toolStripDropDownButton2.Size = new System.Drawing.Size(86, 22);
            this.toolStripDropDownButton2.Text = "Accounts";
            // 
            // editAccountToolStripMenuItem
            // 
            this.editAccountToolStripMenuItem.Name = "editAccountToolStripMenuItem";
            this.editAccountToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.editAccountToolStripMenuItem.Text = "Edit account";
            this.editAccountToolStripMenuItem.Click += new System.EventHandler(this.editAccountToolStripMenuItem_Click);
            // 
            // addAccountToolStripMenuItem
            // 
            this.addAccountToolStripMenuItem.Name = "addAccountToolStripMenuItem";
            this.addAccountToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.addAccountToolStripMenuItem.Text = "Add account";
            this.addAccountToolStripMenuItem.Click += new System.EventHandler(this.addAccountToolStripMenuItem_Click);
            // 
            // deleteAccountToolStripMenuItem
            // 
            this.deleteAccountToolStripMenuItem.Name = "deleteAccountToolStripMenuItem";
            this.deleteAccountToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.deleteAccountToolStripMenuItem.Text = "Delete account";
            this.deleteAccountToolStripMenuItem.Click += new System.EventHandler(this.deleteAccountToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1204, 653);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Email Manager";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView tvwEmailFolders;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox1;
        private System.Windows.Forms.ToolStripComboBox tscbAccount;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem downloadEmailsToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel tsslStatus;
        private System.Windows.Forms.ToolStripMenuItem cancelDownloadToolStripMenuItem;
        private System.Windows.Forms.NotifyIcon niNotify;
        private System.Windows.Forms.ToolStripMenuItem encryptSettingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem downloadEmailsAllAccountsToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton2;
        private System.Windows.Forms.ToolStripMenuItem editAccountToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addAccountToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteAccountToolStripMenuItem;
    }
}

