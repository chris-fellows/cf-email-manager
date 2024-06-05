using System;
using System.Windows.Forms;

namespace CFEmailManager.Forms
{
    /// <summary>
    /// Form to allow entry of text string value
    /// </summary>
    public partial class InputValueForm : Form
    {
        public InputValueForm()
        {
            InitializeComponent();
        }

        public InputValueForm(string title, string label, string defaultValue, Char? passwordChar, int? maxLength, bool canCancel)
        {
            InitializeComponent();

            this.Text = title;            
            this.lblLabel.Text = label;
            this.txtValue.Text = defaultValue;
            if (passwordChar != null) this.txtValue.PasswordChar = passwordChar.Value;
            if (maxLength != null) this.txtValue.MaxLength = maxLength.Value;
            this.btnCancel.Enabled = canCancel;
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

        public string EnteredValue => txtValue.Text;        
    }
}
