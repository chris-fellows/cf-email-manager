using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CFEmailManager.Interfaces;
using CFEmailManager.Model;
using CFEmailManager.Controls;
using CFEmailManager.Utilities;

namespace CFEmailManager.Forms
{
    /// <summary>
    /// Main form. Displays email folders. User can select folder and browse emails within folder    
    /// </summary>
    public partial class MainForm : Form
    {      
        private readonly IEmailAccountService _emailAccountRepository;
        private readonly IEmailDownloaderService _emailDownloader;
        private readonly IEnumerable<IEmailConnection> _emailConnections;
        private readonly IEnumerable<IEmailStorageService> _emailStorageServices;
        private bool _isTaskTray = false;
        private bool _syncing = false;
        private System.Timers.Timer _timer = null;
        private DateTimeOffset _timeLastSync = DateTimeOffset.MinValue;

        private Task<EmailDownloadStatistics> _downloadEmailsTask;

        public MainForm(IEmailAccountService emailAccountRepository,
                        IEnumerable<IEmailConnection> emailConnections,
                        IEmailDownloaderService emailDownloader,
                        IEnumerable<IEmailStorageService> emailStorageServices)
        {            
            _emailAccountRepository = emailAccountRepository;
            _emailConnections = emailConnections;
            _emailDownloader = emailDownloader;
            _emailStorageServices = emailStorageServices;
            
            InitializeComponent();
        }
   
        private EmailAccount SelectedEmailAccount
        {
            get { return (EmailAccount)tscbAccount.SelectedItem; }
        }


        private IEmailStorageService SelectedEmailStorageService
        {
            get { return _emailStorageServices.First(er => er.EmailAddress == SelectedEmailAccount.EmailAddress); }
        }

        /// <summary>
        /// Email summary list control if loaded
        /// </summary>
        private EmailSummaryListControl EmailSummaryListControl
        {
            get
            {
                if (splitContainer2.Panel1.Controls[0] is EmailSummaryListControl)
                {
                    return (EmailSummaryListControl)splitContainer2.Panel1.Controls[0];
                }
                return null;                
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {            
            DisplayStatus("Initializing");
            
            InitializeScreen("");                    

            if (Environment.GetCommandLineArgs().Contains("/Tray"))
            {
                RunInTray();
            }

            DisplayStatus("Ready");
        }

        private void RunInTray()
        {
            _isTaskTray = true;
            niNotify.Icon = this.Icon;      // SystemIcons.Application doesn't work
            niNotify.Text = "Email Manager - Idle";

            _timer = new System.Timers.Timer();
            _timer.Elapsed += _timer_Elapsed;
            _timer.Interval = 10000 * 1;    // Run soon after launch
            _timer.Enabled = true;
        }

        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                _timer.Enabled = false;

                if (!_syncing && _timeLastSync.AddHours(3) <= DateTimeOffset.UtcNow)
                {
                    _timeLastSync = DateTimeOffset.UtcNow;

                    // Download emails for each account                    
                    foreach(var emailAccount in _emailAccountRepository.GetAll())
                    {
                        try
                        {
                            var emailStorageService = _emailStorageServices.First(er => er.EmailAddress == emailAccount.EmailAddress);
                            _downloadEmailsTask = DownloadEmailsAsync(emailAccount, true, true, emailStorageService);
                            _downloadEmailsTask.Wait();                                                        
                        }
                        catch(Exception exception)
                        {

                        }
                    }
                }
            }
            finally
            {
                _timer.Interval = 30000;   // Occasional check
                _timer.Enabled = true;                
            }
        }

        private void InitializeScreen(string selectedEmailAccountId)
        {            
            // Default
            DisplayEmailSummaryListControl();   
            
            // Display email accounts
            var emailAccounts = _emailAccountRepository.GetAll();
            tscbAccount.ComboBox.ValueMember = nameof(EmailAccount.EmailAddress);
            tscbAccount.ComboBox.DisplayMember = nameof(EmailAccount.EmailAddress);
            tscbAccount.ComboBox.DataSource = emailAccounts;

            // Select email account
            //if (emailAccounts.Any()) tscbAccount.SelectedIndex = 0;
            if (!String.IsNullOrEmpty(selectedEmailAccountId))
            {
                var emailAccount = emailAccounts.First(a => a.ID == selectedEmailAccountId);
                tscbAccount.SelectedIndex = emailAccounts.IndexOf(emailAccount);
            }
            else if (emailAccounts.Any())
            {
                tscbAccount.SelectedIndex = 0;
            }


            // Set UI based on whether any accounts
            downloadEmailsToolStripMenuItem.Enabled = emailAccounts.Any();
            downloadEmailsAllAccountsToolStripMenuItem.Enabled = emailAccounts.Any();
            deleteAccountToolStripMenuItem.Enabled = emailAccounts.Any();
            editAccountToolStripMenuItem.Enabled = emailAccounts.Any();
        }

        private void DisplayEmailSummaryListControl()
        {            
            var control = new EmailSummaryListControl();
            control.Dock = DockStyle.Fill;
            control.OnEmailSelected += EmailSummaryListControl_OnEmailSelected;
            control.OnNoEmailSelected += EmailSummaryListControl_OnNoEmailSelected;
            splitContainer2.Panel1.Controls.Clear();
            splitContainer2.Panel1.Controls.Add(control);
        }

        private void EmailSummaryListControl_OnNoEmailSelected()
        {
            DisplayNoEmail();   
        }

        private void EmailSummaryListControl_OnEmailSelected(EmailFolder emailFolder, EmailObject emailObject)
        {
            DisplayEmail(emailFolder, emailObject);
        }
      
        /// <summary>
        /// Displays email folders for email account
        /// </summary>
        /// <param name="emailAccount"></param>
        private void DisplayEmailFolders(EmailAccount emailAccount)
        {
            tvwEmailFolders.Nodes.Clear();
          
            var emailStorageService = _emailStorageServices.First(er => er.EmailAddress == emailAccount.EmailAddress);

            List<EmailFolder> emailFolders = emailStorageService.GetAllFolders().ToList();
            List<EmailFolder> parentEmailFolders = emailFolders.Where(f => f.ParentFolderID == Guid.Empty).ToList();
           
            foreach (var emailFolder in parentEmailFolders.OrderBy(f => f.Name))
            {
                DisplayFolderNode(emailFolder, emailStorageService, null);               
            }
        }

        private void DisplayFolderNode(EmailFolder emailFolder, IEmailStorageService emailRepository, TreeNode parentNode)
        {
            TreeNode nodeFolder = null;
            if (parentNode == null)
            {
                if (emailFolder.ExistsOnServer)
                {
                    nodeFolder = tvwEmailFolders.Nodes.Add(string.Format("Folder.{0}", emailFolder.ID), emailFolder.Name);
                }
                else
                {
                    nodeFolder = tvwEmailFolders.Nodes.Add(string.Format("Folder.{0}", emailFolder.ID), string.Format("{0} [DELETED]", emailFolder.Name));
                }
            }
            else
            {
                if (emailFolder.ExistsOnServer)
                {
                    nodeFolder = parentNode.Nodes.Add(string.Format("Folder.{0}", emailFolder.ID), emailFolder.Name);
                }
                else
                {
                    nodeFolder = parentNode.Nodes.Add(string.Format("Folder.{0}", emailFolder.ID), string.Format("{0} [DELETED]", emailFolder.Name));
                }
            }
            nodeFolder.Tag = emailFolder;

            var emailSubFolders = emailRepository.GetChildFolders(emailFolder);
            foreach(var emailSubFolder in emailSubFolders)
            {
                DisplayFolderNode(emailSubFolder, emailRepository, nodeFolder);
            }           
        }
      
        /// <summary>
        /// Downloads emails asynchronously
        /// </summary>
        /// <param name="emailAccount"></param>
        /// <returns></returns>
        private Task<EmailDownloadStatistics> DownloadEmailsAsync(EmailAccount emailAccount, bool downloadAttachments, bool displayEmailFolders,
                                                                IEmailStorageService emailStorageService)
        {                  
            var task = _emailDownloader.DownloadEmailsAsync(emailAccount,
                            emailStorageService,
                            downloadAttachments,
                            (folder) =>     // Action when starting folder download
                            {
                                this.Invoke((Action)delegate
                                {
                                    DisplayStatus($"Downloading {folder}");
                                });
                            },
                              (folder) =>    // Action when completed folder download
                              {
                                  this.Invoke((Action)delegate
                                  {
                                      DisplayStatus($"Downloaded {folder}");
                                  });
                              },
                            () =>    // Action when download started
                            {
                                // Indicate started
                                this.Invoke((Action)delegate
                                {
                                    tscbAccount.Enabled = false;    // Prevent account change
                                    downloadEmailsToolStripMenuItem.Visible = false;
                                    downloadEmailsAllAccountsToolStripMenuItem.Visible = false;
                                    cancelDownloadToolStripMenuItem.Visible = true;     // Allow cancel
                                    cancelDownloadToolStripMenuItem.Text = "Cancel download";   // Sanity check                    
                                    niNotify.Text = $"Email Manager - Downloading {emailAccount.EmailAddress}";
                                    DisplayStatus("Downloading emails");
                                });
                            },
                            (emailDownloadStatistics) =>    // Action when download completed
                            {
                                // Indicate complete
                                this.Invoke((Action)delegate
                                {
                                    tscbAccount.Enabled = true;    // Allow account change
                                    downloadEmailsToolStripMenuItem.Visible = true;
                                    downloadEmailsAllAccountsToolStripMenuItem.Visible = true;
                                    cancelDownloadToolStripMenuItem.Visible = false;    // Disable cancel
                                    cancelDownloadToolStripMenuItem.Text = "Cancel download";
                                    niNotify.Text = $"Email Manager - Idle";

                                    // Update last download time
                                    emailAccount.TimeLastDownload = DateTimeOffset.UtcNow;
                                    _emailAccountRepository.Update(emailAccount);

                                    DisplayStatus($"Downloaded {emailDownloadStatistics.CountEmailsDownloaded} emails");                                    
                                
                                    if (displayEmailFolders)
                                    {
                                        DisplayEmailFolders(emailAccount);
                                    }
                                });
                            });

            return task;
        }

        /// <summary>
        /// Displays email
        /// </summary>
        /// <param name="emailFolder"></param>
        /// <param name="email"></param>
        private void DisplayEmail(EmailFolder emailFolder, EmailObject email)
        {
            splitContainer2.Panel2.Controls.Clear();

            var control = new EmailObjectControl();
            control.SetParameters(emailFolder, email, SelectedEmailStorageService);
            control.Dock = DockStyle.Fill;
            control.Refresh();
            splitContainer2.Panel2.Controls.Add(control);
        }

        /// <summary>
        /// Displays no email
        /// </summary>
        private void DisplayNoEmail()
        {
            splitContainer2.Panel2.Controls.Clear();
        }

        /// <summary>
        /// Creates the nodes for the folder path
        /// </summary>
        /// <param name="treeView"></param>
        /// <param name="folderPath"></param>
        private TreeNode CreateFolderNodes(TreeView treeView, string folderPath)
        {
            string[] folderNames = folderPath.Split('/');

            TreeNode currentNode = tvwEmailFolders.Nodes[0];

            for(int folderIndex =0; folderIndex < folderNames.Length; folderIndex++)
            {
                TreeNode folderNode = null;
                foreach(TreeNode childNode in currentNode.Nodes)
                {
                    if (childNode.Name == folderNames[folderIndex])
                    {
                        folderNode = childNode;
                        break;
                    }
                }

                if (folderNode == null)
                {
                    folderNode = currentNode.Nodes.Add(folderNames[folderIndex], folderNames[folderIndex]);
                }

                // Set node as current node
                currentNode = folderNode;   
            }
            return currentNode;
        }

        private void tvwEmails_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Get emails to display, most recent first
            EmailFolder emailFolder = (EmailFolder)e.Node.Tag;
            var emails = SelectedEmailStorageService.GetEmails(emailFolder).OrderByDescending(m => m.ReceivedDate).ToList();

            //DisplayEmails(emails);
            var emailSummaryListControl = this.EmailSummaryListControl;
            if (emailSummaryListControl != null)
            {                
                emailSummaryListControl.ModelToView(emails);
            }
        }
        

        private void tscbAccount_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tscbAccount.SelectedIndex > -1)
            {
                EmailAccount emailAccount = (EmailAccount)tscbAccount.SelectedItem;

                // Set email storage for
                var emailSummaryListControl = this.EmailSummaryListControl;
                if (emailSummaryListControl != null)
                {                    
                    emailSummaryListControl.EmailStorageService = this.SelectedEmailStorageService;
                }

                DisplayEmailFolders(emailAccount);
            }
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void downloadEmailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var emailStorageService = _emailStorageServices.First(ess => ess.EmailAddress == SelectedEmailAccount.EmailAddress);
            _downloadEmailsTask = DownloadEmailsAsync(SelectedEmailAccount, true, true, emailStorageService);
        }

        private void DisplayStatus(string status)
        {            
            tsslStatus.Text = string.Format(" {0}", status);
            statusStrip1.Update();
        }

        private void cancelDownloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _emailDownloader.Cancel();
        }

        private void niNotify_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (_isTaskTray)
            {
                Show();
                WindowState = FormWindowState.Normal;
            }
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (_isTaskTray && FormWindowState.Minimized == WindowState)
            {
                Hide();
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // If closing form with task tray then prompt user
            if (e.CloseReason == CloseReason.UserClosing && _isTaskTray)
            {
                if (MessageBox.Show("Close application?", "Close", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }

            niNotify.Dispose();
        }

        private void encryptSettingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InputValueForm form = new InputValueForm("Encrypt Setting", "Setting", "", null, null, true);
            if (form.ShowDialog() == DialogResult.OK)
            {
                // Encrypt
                var encrypted = InternalUtilities.EncryptSettingToString(form.EnteredValue);

                // Copy to clipboard
                Clipboard.SetText(encrypted);
                MessageBox.Show("The value has been copied to the clipboard", "Encrypt Setting");
            }
        }

        private void downloadEmailsAllAccountsToolStripMenuItem_Click(object sender, EventArgs e)
        {                           
            foreach (var emailAccount in _emailAccountRepository.GetAll())
            {
                try
                {
                    var emailStorageService = _emailStorageServices.First(er => er.EmailAddress == emailAccount.EmailAddress);

                    _downloadEmailsTask = DownloadEmailsAsync(emailAccount, true, true, emailStorageService);
                    _downloadEmailsTask.Wait();                    
                }
                catch (Exception exception)
                {

                }
            }
        }

        private void editEmailAccountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var emailAccount = SelectedEmailAccount;
            EmailAccountForm form = new EmailAccountForm(emailAccount);            
            if (form.ShowDialog() == DialogResult.OK)
            {
                var messages = form.ValidateBeforeApplyChanges();
                if (messages.Any())
                {
                    MessageBox.Show(messages[0], "Error");
                }
                else if(MessageBox.Show("Save changes?", "Edit Email Account", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    form.ApplyChanges();
                    _emailAccountRepository.Update(emailAccount);
                }
            }
        }

        private void editAccountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var emailAccount = SelectedEmailAccount;
            EmailAccountForm form = new EmailAccountForm(emailAccount);
            if (form.ShowDialog() == DialogResult.OK)
            {
                var messages = form.ValidateBeforeApplyChanges();
                if (messages.Any())
                {
                    MessageBox.Show(messages[0], "Error");
                }
                else if (MessageBox.Show("Save changes?", "Edit Account", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    form.ApplyChanges();
                    _emailAccountRepository.Update(emailAccount);
                }
            }
        }

        private void addAccountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var emailAccount = new EmailAccount()
            {
                ID = Guid.NewGuid().ToString(),
                ServerType = "IMAP"
            };
            EmailAccountForm form = new EmailAccountForm(emailAccount);
            if (form.ShowDialog() == DialogResult.OK)
            {
                var messages = form.ValidateBeforeApplyChanges();
                if (messages.Any())
                {
                    MessageBox.Show(messages[0], "Error");
                }
                else if (MessageBox.Show("Save new account?", "Add Account", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    form.ApplyChanges();
                    _emailAccountRepository.Insert(emailAccount);

                    // Refresh screen with new email account selected
                    InitializeScreen(emailAccount.ID);
                }
            }
        }

        private void deleteAccountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var emailAccount = SelectedEmailAccount;
            if (MessageBox.Show($"Delete account {emailAccount.EmailAddress}?", "Delete Account", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                _emailAccountRepository.Delete(emailAccount.ID);

                // Refresh screen with first email account (if any) selected                
                InitializeScreen("");
            }
        }
    }
}
