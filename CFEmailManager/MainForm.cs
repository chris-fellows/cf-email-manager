﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CFUtilities;
using CFEmailManager.Model;

namespace CFEmailManager
{
    public partial class MainForm : Form
    {        
        private ApplicationData _applicationData;

        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            //_applicationData = GetApplicationData();

            //DownloadEmails();
            DisplayStatus("Initializing");
            //DisplayEmailFolders();

            InitializeScreen();

            DisplayStatus("Ready");

            // Select email account
            tscbAccount.SelectedIndex = 0;
        }

        private void InitializeScreen()
        {
            int count = 0;
            List<EmailAccount> emailAccounts = new List<EmailAccount>();
            do
            {
                count++;
                if (System.Configuration.ConfigurationSettings.AppSettings.Get(string.Format("EmailAccount.{0}.EmailAddress", count)) != null)
                { 
                    string emailAddress = System.Configuration.ConfigurationSettings.AppSettings.Get(string.Format("EmailAccount.{0}.EmailAddress", count)).ToString();                
                    EmailAccount emailAccount = new EmailAccount()
                    {
                        EmailAddress = emailAddress,
                        Password = System.Configuration.ConfigurationSettings.AppSettings.Get(string.Format("EmailAccount.{0}.Password", count)).ToString(),
                        LocalFolder = System.Configuration.ConfigurationSettings.AppSettings.Get(string.Format("EmailAccount.{0}.LocalEmailFolder", count)).ToString()
                    };
                    emailAccounts.Add(emailAccount);
                }
                else
                {
                    count = -1;
                }
            } while (count > 0);

            tscbAccount.ComboBox.ValueMember = nameof(EmailAccount.EmailAddress);
            tscbAccount.ComboBox.DisplayMember = nameof(EmailAccount.EmailAddress);
            tscbAccount.ComboBox.DataSource = emailAccounts;
        }
        
    
        private ApplicationData GetApplicationData(EmailAccount emailAccount)
        {
            //string localEmailFolder = @"C:\Data\Dev\C#\CFEmailManager\bin\Debug\Emails";
           // string localEmailFolder = System.Configuration.ConfigurationSettings.AppSettings.Get("LocalEmailFolder").ToString();

            var applicationData = new ApplicationData();
            applicationData.EmailRepository = new FileEmailRepository(emailAccount.LocalFolder);
            return applicationData;
        }

        private void RunSearch(EmailSearch emailSearch)
        {
            var emails = _applicationData.EmailRepository.Search(emailSearch);


            DisplayEmails(emails);           
        }

        private void DisplayEmailFolders(EmailAccount emailAccount)
        {
            tvwEmails.Nodes.Clear();
            dgvEmail.Rows.Clear();
            dgvEmail.Columns.Clear();
            wbEmail.Visible = false;

            _applicationData = GetApplicationData(emailAccount);

            List<EmailFolder> emailFolders = _applicationData.EmailRepository.GetAllFolders().ToList();
            List<EmailFolder> parentEmailFolders = emailFolders.Where(f => f.ParentFolderID == Guid.Empty).ToList();
           
            foreach (var emailFolder in parentEmailFolders.OrderBy(f => f.Name))
            {
                DisplayFolderNode(emailFolder, null);               
            }
        }

        private void DisplayFolderNode(EmailFolder emailFolder, TreeNode parentNode)
        {
            TreeNode nodeFolder = null;
            if (parentNode == null)
            {
                if (emailFolder.ExistsOnServer)
                {
                    nodeFolder = tvwEmails.Nodes.Add(string.Format("Folder.{0}", emailFolder.ID), emailFolder.Name);
                }
                else
                {
                    nodeFolder = tvwEmails.Nodes.Add(string.Format("Folder.{0}", emailFolder.ID), string.Format("{0} [DELETED]", emailFolder.Name));
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

            var emailSubFolders = _applicationData.EmailRepository.GetChildFolders(emailFolder);
            foreach(var emailSubFolder in emailSubFolders)
            {
                DisplayFolderNode(emailSubFolder, nodeFolder);
            }           
        }

        private void DownloadEmails(EmailAccount emailAccount)
        {
            EmailConnection emailConnection = new EmailConnection();

            //string localEmailFolder = @"C:\Data\Dev\C#\CFEmailManager\bin\Debug\Emails";

            IEmailRepository emailRepository = new FileEmailRepository(emailAccount.LocalFolder);
            emailConnection.DownloadViaImap("imap-mail.outlook.com", emailAccount.EmailAddress, emailAccount.Password, emailAccount.LocalFolder, true, emailRepository);
        }

        private void DisplayEmail(EmailObject email)
        {
            splitContainer1.Panel2.Controls.Clear();

            CFEmailManager.Controls.EmailObjectControl control = new CFEmailManager.Controls.EmailObjectControl();
            control.SetParameters(email);
            control.Dock = DockStyle.Fill;
            control.Refresh();
            splitContainer1.Panel2.Controls.Add(control);
        }

        /// <summary>
        /// Creates the nodes for the folder path
        /// </summary>
        /// <param name="treeView"></param>
        /// <param name="folderPath"></param>
        private TreeNode CreateFolderNodes(TreeView treeView, string folderPath)
        {
            string[] folderNames = folderPath.Split('/');

            TreeNode currentNode = tvwEmails.Nodes[0];

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
            EmailFolder emailFolder = (EmailFolder)e.Node.Tag;
            var emails = _applicationData.EmailRepository.GetEmails(emailFolder).OrderBy(m => m.ReceivedDate).ToList();     
            DisplayEmails(emails);
        }

        private void DisplayEmails(List<EmailObject> emails)
        {
            dgvEmail.Rows.Clear();
            dgvEmail.Columns.Clear();
            int columnIndex = -1;
            columnIndex = dgvEmail.Columns.Add("EmailFolder", "EmailFolder");
            dgvEmail.Columns[columnIndex].Visible = false;
            columnIndex = dgvEmail.Columns.Add("Received", "Received");
            dgvEmail.Columns[columnIndex].DefaultCellStyle.Format = "dd-MM-yyyy HH:mm";
            dgvEmail.Columns[columnIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            columnIndex = dgvEmail.Columns.Add("Sender", "Sender");
            columnIndex = dgvEmail.Columns.Add("Subject", "Subject");
            columnIndex = dgvEmail.Columns.Add("Attachments", "Attachments");
            columnIndex = dgvEmail.Columns.Add("On Server", "On Server");
            dgvEmail.Columns[columnIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            wbEmail.Visible = false;

            List<EmailFolder> emailFolders = _applicationData.EmailRepository.GetAllFolders();

            foreach(var email in emails)
            {
                EmailFolder emailFolder = emailFolders.First(f => f.ID == email.FolderID);

                using (DataGridViewRow row = new DataGridViewRow())
                {
                    using (DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell())
                    {
                        cell.Value = emailFolder.ID;
                        cell.Tag = emailFolder;
                        row.Cells.Add(cell);
                    }

                    using (DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell())
                    {
                        cell.Value = email.ReceivedDate;
                        cell.Tag = email;
                        row.Cells.Add(cell);
                    }

                    using (DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell())
                    {
                        cell.Value = email.SenderAddress;
                        row.Cells.Add(cell);
                    }

                    using (DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell())
                    {
                        cell.Value = email.Subject;
                        row.Cells.Add(cell);
                    }

                    using (DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell())
                    {
                        StringBuilder attachmentList = new StringBuilder("");
                        if (email.Attachments !=  null && email.Attachments.Any())
                        {
                            foreach(var attachment in email.Attachments)
                            {
                                if (attachmentList.Length > 0)
                                {
                                    attachmentList.Append(", ");
                                }
                                attachmentList.Append(attachment.Name);
                            }
                        }
                        cell.Value = attachmentList.ToString();
                        row.Cells.Add(cell);
                    }

                    using (DataGridViewCheckBoxCell cell = new DataGridViewCheckBoxCell())
                    {
                        cell.Value = email.ExistsOnServer;
                        row.Cells.Add(cell);
                    }


                        dgvEmail.Rows.Add(row);
                }

            }

            dgvEmail.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

        }

        private void dgvEmail_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            EmailFolder emailFolder = (EmailFolder)dgvEmail.Rows[e.RowIndex].Cells["EmailFolder"].Tag;
            EmailObject email = (EmailObject)dgvEmail.Rows[e.RowIndex].Cells["Received"].Tag;

            DisplayEmail(emailFolder, email);
        }

        private void DisplayEmail(EmailFolder emailFolder, EmailObject emailObject)
        {
            string emailBodyFile = System.IO.Path.Combine(emailFolder.LocalFolder, string.Format("Body.{0}.eml", emailObject.ID));
            string tmpFile = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "CFEmailManager.email.html");
            System.IO.File.Copy(emailBodyFile, tmpFile, true);

            if (System.IO.File.Exists(emailBodyFile))
            {
                //wbEmail.Navigate("file://" + emailBodyFile.Replace(@"\\", @"//"));
                wbEmail.Visible = true;
                wbEmail.Navigate("file://" + tmpFile.Replace(@"\\", @"//"));
            }
            else
            {
                MessageBox.Show("Email file does not exist");
            }
        }

        private void tscbAccount_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tscbAccount.SelectedIndex > -1)
            {
                EmailAccount emailAccount = (EmailAccount)tscbAccount.SelectedItem;
                DisplayEmailFolders(emailAccount);
            }
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void downloadEmailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisplayStatus("Downloading emails");
            EmailAccount emailAccount = (EmailAccount)tscbAccount.SelectedItem;
            DownloadEmails(emailAccount);
            DisplayStatus("Downloaded emails");
        }

        private void DisplayStatus(string status)
        {            
            tsslStatus.Text = string.Format(" {0}", status);
            statusStrip1.Update();
        }
    }
}