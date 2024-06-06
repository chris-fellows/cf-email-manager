using CFEmailManager.Model;
using CFEmailManager.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CFEmailManager.Forms
{
    /// <summary>
    /// Form to edit email account
    /// </summary>
    public partial class EmailAccountForm : Form
    {
        private EmailAccount _emailAccount;

        public EmailAccountForm()
        {
            InitializeComponent();
        }

        public EmailAccountForm(EmailAccount emailAccount)
        {
            InitializeComponent();

            _emailAccount = emailAccount;
            ModelToView(_emailAccount);
        }

        private void ModelToView(EmailAccount emailAccount)
        {                        
            txtServer.Text = emailAccount.Server;
            txtEmailAddress.Text = emailAccount.EmailAddress;
            txtPassword.Text = InternalUtilities.DecryptSettingToString(emailAccount.Password);
            txtLocalFolder.Text = emailAccount.LocalFolder;

            var items = new[] { "POP", "IMAP" };
            items.ToList().ForEach(item => cbServerType.Items.Add(item));
            cbServerType.SelectedIndex = Array.IndexOf(items, emailAccount.ServerType);            
        }

        private void ViewToModel(EmailAccount emailAccount)
        {
            emailAccount.Server = txtServer.Text;
            emailAccount.EmailAddress = txtEmailAddress.Text;
            emailAccount.Password = InternalUtilities.EncryptSettingToString(txtPassword.Text);
            emailAccount.LocalFolder = txtLocalFolder.Text;
            emailAccount.ServerType = cbServerType.SelectedItem.ToString();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        public void ApplyChanges()
        {
            ViewToModel(_emailAccount);
        }

        public List<string> ValidateBeforeApplyChanges()
        {
            List<string> messages = new List<string>();
            if (String.IsNullOrEmpty(txtServer.Text)) messages.Add("Server is invalid");
            if (String.IsNullOrEmpty(txtEmailAddress.Text)) messages.Add("Email is invalid");
            if (String.IsNullOrEmpty(txtPassword.Text)) messages.Add("Password is invalid");
            if (String.IsNullOrEmpty(txtLocalFolder.Text)) messages.Add("Local Folder is invalid");

            return messages;
        }
    }
}
