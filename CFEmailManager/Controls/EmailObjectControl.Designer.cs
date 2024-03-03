namespace CFEmailManager.Controls
{
    partial class EmailObjectControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.wbEmail = new System.Windows.Forms.WebBrowser();
            this.label1 = new System.Windows.Forms.Label();
            this.cbAttachment = new System.Windows.Forms.ComboBox();
            this.btnDownloadAttachment = new System.Windows.Forms.Button();
            this.btnDownloadAttachments = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // wbEmail
            // 
            this.wbEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.wbEmail.Location = new System.Drawing.Point(3, 27);
            this.wbEmail.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbEmail.Name = "wbEmail";
            this.wbEmail.Size = new System.Drawing.Size(935, 499);
            this.wbEmail.TabIndex = 1;
            this.wbEmail.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Attachments:";
            // 
            // cbAttachment
            // 
            this.cbAttachment.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAttachment.FormattingEnabled = true;
            this.cbAttachment.Location = new System.Drawing.Point(79, 0);
            this.cbAttachment.Name = "cbAttachment";
            this.cbAttachment.Size = new System.Drawing.Size(328, 21);
            this.cbAttachment.TabIndex = 3;
            // 
            // btnDownloadAttachment
            // 
            this.btnDownloadAttachment.Location = new System.Drawing.Point(413, -2);
            this.btnDownloadAttachment.Name = "btnDownloadAttachment";
            this.btnDownloadAttachment.Size = new System.Drawing.Size(83, 23);
            this.btnDownloadAttachment.TabIndex = 4;
            this.btnDownloadAttachment.Text = "Download";
            this.btnDownloadAttachment.UseVisualStyleBackColor = true;
            this.btnDownloadAttachment.Click += new System.EventHandler(this.btnDownloadAttachment_Click);
            // 
            // btnDownloadAttachments
            // 
            this.btnDownloadAttachments.Location = new System.Drawing.Point(502, -2);
            this.btnDownloadAttachments.Name = "btnDownloadAttachments";
            this.btnDownloadAttachments.Size = new System.Drawing.Size(83, 23);
            this.btnDownloadAttachments.TabIndex = 5;
            this.btnDownloadAttachments.Text = "Download All";
            this.btnDownloadAttachments.UseVisualStyleBackColor = true;
            this.btnDownloadAttachments.Click += new System.EventHandler(this.btnDownloadAttachments_Click);
            // 
            // EmailObjectControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnDownloadAttachments);
            this.Controls.Add(this.btnDownloadAttachment);
            this.Controls.Add(this.cbAttachment);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.wbEmail);
            this.Name = "EmailObjectControl";
            this.Size = new System.Drawing.Size(941, 529);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.WebBrowser wbEmail;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbAttachment;
        private System.Windows.Forms.Button btnDownloadAttachment;
        private System.Windows.Forms.Button btnDownloadAttachments;
    }
}
