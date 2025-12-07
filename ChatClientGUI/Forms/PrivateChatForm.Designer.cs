namespace ChatClientGUI.Forms
{
    partial class PrivateChatForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lstPrivateMessages = new ListBox();
            txtPrivateInput = new TextBox();
            btnSendPrivate = new Button();
            SuspendLayout();
            // 
            // lstPrivateMessages
            // 
            lstPrivateMessages.FormattingEnabled = true;
            lstPrivateMessages.ItemHeight = 15;
            lstPrivateMessages.Location = new Point(101, 60);
            lstPrivateMessages.Name = "lstPrivateMessages";
            lstPrivateMessages.Size = new Size(211, 184);
            lstPrivateMessages.TabIndex = 0;
            // 
            // txtPrivateInput
            // 
            txtPrivateInput.Location = new Point(111, 298);
            txtPrivateInput.Name = "txtPrivateInput";
            txtPrivateInput.Size = new Size(100, 23);
            txtPrivateInput.TabIndex = 1;
            // 
            // btnSendPrivate
            // 
            btnSendPrivate.Location = new Point(278, 301);
            btnSendPrivate.Name = "btnSendPrivate";
            btnSendPrivate.Size = new Size(75, 23);
            btnSendPrivate.TabIndex = 2;
            btnSendPrivate.Text = "button1";
            btnSendPrivate.UseVisualStyleBackColor = true;
            // 
            // PrivateChatForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnSendPrivate);
            Controls.Add(txtPrivateInput);
            Controls.Add(lstPrivateMessages);
            Name = "PrivateChatForm";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox lstPrivateMessages;
        private TextBox txtPrivateInput;
        private Button btnSendPrivate;
    }
}