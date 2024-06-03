using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CFEmailManager.Interfaces;
using CFEmailManager.Model;
using CFEmailManager.Controls;

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
        
        private Task<EmailDownloadStatistics> _downloadEmailsTask;     // Active download task

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
            
            InitializeScreen();

            DisplayStatus("Ready");

            // Select email account
            tscbAccount.SelectedIndex = 0;
        }

        private void InitializeScreen()
        {            
            // Default
            DisplayEmailSummaryListControl();   
            
            // Display email accounts
            var emailAccounts = _emailAccountRepository.GetAll();
            tscbAccount.ComboBox.ValueMember = nameof(EmailAccount.EmailAddress);
            tscbAccount.ComboBox.DisplayMember = nameof(EmailAccount.EmailAddress);
            tscbAccount.ComboBox.DataSource = emailAccounts;         
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
        private Task<EmailDownloadStatistics> DownloadEmailsAsync(EmailAccount emailAccount, bool downloadAttachments, bool displayEmailFolders)
        {
            var emailStorageService = _emailStorageServices.First(er => er.EmailAddress == emailAccount.EmailAddress);

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
                                    cancelDownloadToolStripMenuItem.Visible = true;     // Allow cancel
                                    cancelDownloadToolStripMenuItem.Text = "Cancel download";   // Sanity check                    
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
                                    cancelDownloadToolStripMenuItem.Visible = false;    // Disable cancel
                                    cancelDownloadToolStripMenuItem.Text = "Cancel download";
                                    DisplayStatus($"Downloaded {emailDownloadStatistics.CountEmailsDownloaded} emails");                                    

                                    // Display email folders
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
            _downloadEmailsTask = DownloadEmailsAsync(SelectedEmailAccount, true, true);                                  
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
    }
}
