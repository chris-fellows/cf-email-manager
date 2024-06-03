using CFEmailManager.Interfaces;
using CFEmailManager.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CFEmailManager.Controls
{
    /// <summary>
    /// Display email summary list (Basic email details)
    /// </summary>
    public partial class EmailSummaryListControl : UserControl
    {
        public delegate void EmailSelected(EmailFolder emailFolder, EmailObject emailObject);
        public event EmailSelected OnEmailSelected;

        public delegate void NoEmailSelected();
        public event NoEmailSelected OnNoEmailSelected;

        private IEmailStorageService _emailStorageService;

        public EmailSummaryListControl()
        {
            InitializeComponent();
        }

        public IEmailStorageService EmailStorageService
        {
            get { return _emailStorageService; }
            set 
            {
                if (_emailStorageService != value)
                {
                    _emailStorageService = value;
                    ModelToView(new List<EmailObject>());   // Clear email list
                }
            }
        }

        public void ModelToView(List<EmailObject> emails)
        {
            dgvEmail.Rows.Clear();
            dgvEmail.Columns.Clear();
            int columnIndex = -1;
            columnIndex = dgvEmail.Columns.Add("EmailFolder", "EmailFolder");
            dgvEmail.Columns[columnIndex].Visible = false;
            columnIndex = dgvEmail.Columns.Add("Received", "Received");
            dgvEmail.Columns[columnIndex].DefaultCellStyle.Format = "dd-MM-yyyy HH:mm";
            dgvEmail.Columns[columnIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            columnIndex = dgvEmail.Columns.Add("From", "From");
            columnIndex = dgvEmail.Columns.Add("To", "To");
            columnIndex = dgvEmail.Columns.Add("Subject", "Subject");
            columnIndex = dgvEmail.Columns.Add("Attachments", "Attachments");
            columnIndex = dgvEmail.Columns.Add("On Server", "On Server");
            dgvEmail.Columns[columnIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            if (emails.Any())
            {
                List<EmailFolder> emailFolders = _emailStorageService.GetAllFolders();

                foreach (var email in emails)
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
                            cell.Value = email.From.Address;
                            row.Cells.Add(cell);
                        }

                        using (DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell())
                        {
                            cell.Value = string.Join("; ", email.To.Select(to => to.Address));
                            row.Cells.Add(cell);
                        }

                        using (DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell())
                        {
                            cell.Value = email.Subject;
                            row.Cells.Add(cell);
                        }

                        using (DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell())
                        {
                            cell.Value = string.Join("; ", email.Attachments.Select(a => a.Name));
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
            }

            dgvEmail.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

            // Flag no email selected
            if (OnNoEmailSelected != null)
            {
                OnNoEmailSelected();
            }
        }

        private void dgvEmail_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                EmailFolder emailFolder = (EmailFolder)dgvEmail.Rows[e.RowIndex].Cells["EmailFolder"].Tag;
                EmailObject email = (EmailObject)dgvEmail.Rows[e.RowIndex].Cells["Received"].Tag;

                if (OnEmailSelected != null)
                {
                    OnEmailSelected(emailFolder, email);
                }
            }
            else 
            {
                if (OnNoEmailSelected != null)
                {
                    OnNoEmailSelected();
                }
            }

            //DisplayEmail(emailFolder, email);
        }
    }
}
