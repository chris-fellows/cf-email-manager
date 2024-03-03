using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CFEmailManager.Model;
using CFUtilities;

namespace CFEmailManager.Controls
{
    public partial class EmailObjectControl : UserControl
    {
        private EmailFolder _emailFolder;
        private EmailObject _emailObject;

        public EmailObjectControl()
        {
            InitializeComponent();
        }

        public void SetParameters(EmailFolder emailFolder, EmailObject emailObject)
        {
            _emailFolder = emailFolder;
            _emailObject = emailObject;

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
            string emailBodyFile = System.IO.Path.Combine(_emailFolder.LocalFolder, string.Format("Attachment.{0}.{1}", emailObject.ID, emailAttachment.ID));
            string localFile = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), emailAttachment.Name);
            System.IO.File.Copy(emailBodyFile, localFile, true);            

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
