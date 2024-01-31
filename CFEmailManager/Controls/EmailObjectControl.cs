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

namespace CFEmailManager.Controls
{
    public partial class EmailObjectControl : UserControl
    {
        private EmailObject _emailObject;

        public EmailObjectControl()
        {
            InitializeComponent();
        }

        public void SetParameters(EmailObject emailObject)
        {
            _emailObject = emailObject;

            ModelToView(emailObject);
        }

        private void ModelToView(EmailObject emailObject)
        {

        }

      
    }
}
