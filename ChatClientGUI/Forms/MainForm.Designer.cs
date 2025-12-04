namespace ChatClientGUI.Forms
{
    partial class MainForm
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
            lstMessages = new ListBox();
            txtMessage = new TextBox();
            btnSend = new Button();
            btnFile = new Button();
            label1 = new Label();
            lstUsers = new ListBox();
            SuspendLayout();
            // 
            // lstMessages
            // 
            lstMessages.FormattingEnabled = true;
            lstMessages.ItemHeight = 15;
            lstMessages.Location = new Point(110, 47);
            lstMessages.Name = "lstMessages";
            lstMessages.Size = new Size(246, 139);
            lstMessages.TabIndex = 0;
            // 
            // txtMessage
            // 
            txtMessage.Location = new Point(110, 203);
            txtMessage.Name = "txtMessage";
            txtMessage.Size = new Size(100, 23);
            txtMessage.TabIndex = 1;
            // 
            // btnSend
            // 
            btnSend.Location = new Point(232, 202);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(75, 23);
            btnSend.TabIndex = 2;
            btnSend.Text = "Küldés";
            btnSend.UseVisualStyleBackColor = true;
            // 
            // btnFile
            // 
            btnFile.Location = new Point(325, 202);
            btnFile.Name = "btnFile";
            btnFile.Size = new Size(75, 23);
            btnFile.TabIndex = 3;
            btnFile.Text = "Fájl küldés";
            btnFile.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(460, 48);
            label1.Name = "label1";
            label1.Size = new Size(118, 15);
            label1.TabIndex = 4;
            label1.Text = "Elérhető felhasználók";
            // 
            // lstUsers
            // 
            lstUsers.FormattingEnabled = true;
            lstUsers.ItemHeight = 15;
            lstUsers.Location = new Point(460, 82);
            lstUsers.Name = "lstUsers";
            lstUsers.Size = new Size(120, 94);
            lstUsers.TabIndex = 5;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(lstUsers);
            Controls.Add(label1);
            Controls.Add(btnFile);
            Controls.Add(btnSend);
            Controls.Add(txtMessage);
            Controls.Add(lstMessages);
            Name = "MainForm";
            Text = "MainForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox lstMessages;
        private TextBox txtMessage;
        private Button btnSend;
        private Button btnFile;
        private Label label1;
        private ListBox lstUsers;
    }
}