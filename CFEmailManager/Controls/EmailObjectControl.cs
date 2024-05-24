using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using CFEmailManager.Interfaces;
using CFEmailManager.Model;
using CFUtilities;

namespace CFEmailManager.Controls
{
    public partial class EmailObjectControl : UserControl
    {
        private EmailFolder _emailFolder;
        private EmailObject _emailObject;
        private IEmailRepository _emailRepository;

        public EmailObjectControl()
        {
            InitializeComponent();
        }

        public void SetParameters(EmailFolder emailFolder, EmailObject emailObject, IEmailRepository emailRepository)
        {
            _emailFolder = emailFolder;
            _emailObject = emailObject;
            _emailRepository = emailRepository;

            ModelToView(emailFolder, emailObject);
        }

        private void ModelToView(EmailFolder emailFolder, EmailObject emailObject)
        {
            var items = new List<NameValuePair<Guid>>();

            // Display attachments
            items.AddRange(emailObject.Attachments.Select(a => new NameValuePair<Guid>(a.Name, a.ID)));
            items.Insert(0, new NameValuePair<Guid>("<None>", Guid.Empty));
            cbAttachment.DisplayMember = nameof(NameValuePair<EmailAttachment>.Name);
            cbAttachment.ValueMember = nameof(NameValuePair<EmailAttachment>.Value);
            cbAttachment.DataSource = items;            
            
            // Display email body            
            string tmpFile = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "CFEmailManager.email.html");            
            var emailData = _emailRepository.GetEmailContent(emailObject);
            System.IO.File.WriteAllBytes(tmpFile, emailData);

            //if (System.IO.File.Exists(emailBodyFile))
            if (emailData != null && emailData.Length > 0)
            {                
                wbEmail.Visible = true;
                wbEmail.Navigate("file://" + tmpFile.Replace(@"\\", @"//"));
            }
            else
            {
                MessageBox.Show("Email file does not exist");
            }
        }

        private void btnDownloadAttachment_Click(object sender, EventArgs e)
        {
            var emailAttachmentId = (Guid)cbAttachment.SelectedValue;
            if (emailAttachmentId != Guid.Empty)
            {
                // Download attachment
                var localFile = DownloadAttachment(_emailFolder, _emailObject, _emailObject.Attachments.First(a => a.ID == emailAttachmentId));

                // Open Explorer folder
                IOUtilities.OpenDirectoryWithExplorer(System.IO.Path.GetDirectoryName(localFile));
            }
        }

        private string DownloadAttachment(EmailFolder emailFolder, EmailObject emailObject, EmailAttachment emailAttachment)                            
        {
            //string emailBodyFile = System.IO.Path.Combine(_emailFolder.LocalFolder, string.Format("Attachment.{0}.{1}", emailObject.ID, emailAttachment.ID));
            string localFile = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), emailAttachment.Name);
            //System.IO.File.Copy(emailBodyFile, localFile, true);            
            var attachmentContent = _emailRepository.GetEmailAttachmentContent(emailObject, emailObject.Attachments.IndexOf(emailAttachment));
            System.IO.File.WriteAllBytes(localFile, attachmentContent);
            return localFile;
        }

        private void btnDownloadAttachments_Click(object sender, EventArgs e)
        {
            if (_emailObject.Attachments != null && _emailObject.Attachments.Any())
            {
                // Download attachments
                var localFile = "";
                foreach (var emailAttachment in _emailObject.Attachments)
                {
                    localFile = DownloadAttachment(_emailFolder, _emailObject, emailAttachment);
                }

                // Open Explorer folder
                IOUtilities.OpenDirectoryWithExplorer(System.IO.Path.GetDirectoryName(localFile));
            }
        }
    }
}
