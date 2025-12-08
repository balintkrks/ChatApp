namespace ChatClientGUI.Forms
{
	partial class MainForm
	{
		private System.ComponentModel.IContainer components = null;

		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		private void InitializeComponent()
		{
			lstMessages = new ListBox();
			txtMessage = new TextBox();
			btnSend = new Button();
			btnFile = new Button();
			label1 = new Label();
			lstUsers = new ListBox();
			btnExit = new Button();
			pnlBottom = new Panel();
			pnlRight = new Panel();
			pnlCenter = new Panel();
			pnlBottom.SuspendLayout();
			pnlRight.SuspendLayout();
			pnlCenter.SuspendLayout();
			SuspendLayout();
			// 
			// lstMessages
			// 
			lstMessages.BackColor = Color.White;
			lstMessages.BorderStyle = BorderStyle.None;
			lstMessages.Dock = DockStyle.Fill;
			lstMessages.Font = new Font("Segoe UI", 10F);
			lstMessages.FormattingEnabled = true;
			lstMessages.IntegralHeight = false;
			lstMessages.ItemHeight = 17;
			lstMessages.Location = new Point(10, 10);
			lstMessages.Name = "lstMessages";
			lstMessages.Size = new Size(560, 396);
			lstMessages.TabIndex = 0;
			// 
			// txtMessage
			// 
			txtMessage.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			txtMessage.Font = new Font("Segoe UI", 10F);
			txtMessage.Location = new Point(10, 10);
			txtMessage.Multiline = true;
			txtMessage.Name = "txtMessage";
			txtMessage.Size = new Size(465, 30);
			txtMessage.TabIndex = 1;
			// 
			// btnSend
			// 
			btnSend.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			btnSend.BackColor = Color.DodgerBlue;
			btnSend.Cursor = Cursors.Hand;
			btnSend.FlatAppearance.BorderSize = 0;
			btnSend.FlatStyle = FlatStyle.Flat;
			btnSend.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
			btnSend.ForeColor = Color.White;
			btnSend.Location = new Point(481, 10);
			btnSend.Name = "btnSend";
			btnSend.Size = new Size(80, 30);
			btnSend.TabIndex = 2;
			btnSend.Text = "SEND";
			btnSend.UseVisualStyleBackColor = false;
			// 
			// btnFile
			// 
			btnFile.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			btnFile.BackColor = Color.Gray;
			btnFile.Cursor = Cursors.Hand;
			btnFile.FlatAppearance.BorderSize = 0;
			btnFile.FlatStyle = FlatStyle.Flat;
			btnFile.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
			btnFile.ForeColor = Color.White;
			btnFile.Location = new Point(567, 10);
			btnFile.Name = "btnFile";
			btnFile.Size = new Size(50, 30);
			btnFile.TabIndex = 3;
			btnFile.Text = "FILE";
			btnFile.UseVisualStyleBackColor = false;
			// 
			// label1
			// 
			label1.Dock = DockStyle.Top;
			label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
			label1.ForeColor = Color.DimGray;
			label1.Location = new Point(0, 0);
			label1.Name = "label1";
			label1.Padding = new Padding(5, 5, 0, 0);
			label1.Size = new Size(160, 30);
			label1.TabIndex = 4;
			label1.Text = "ONLINE USERS";
			// 
			// lstUsers
			// 
			lstUsers.BackColor = Color.WhiteSmoke;
			lstUsers.BorderStyle = BorderStyle.None;
			lstUsers.Dock = DockStyle.Fill;
			lstUsers.Font = new Font("Segoe UI", 9F);
			lstUsers.FormattingEnabled = true;
			lstUsers.IntegralHeight = false;
			lstUsers.ItemHeight = 15;
			lstUsers.Location = new Point(0, 30);
			lstUsers.Name = "lstUsers";
			lstUsers.Size = new Size(160, 386);
			lstUsers.TabIndex = 5;
			// 
			// btnExit
			// 
			btnExit.BackColor = Color.IndianRed;
			btnExit.Cursor = Cursors.Hand;
			btnExit.Dock = DockStyle.Bottom;
			btnExit.FlatAppearance.BorderSize = 0;
			btnExit.FlatStyle = FlatStyle.Flat;
			btnExit.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
			btnExit.ForeColor = Color.White;
			btnExit.Location = new Point(0, 416);
			btnExit.Name = "btnExit";
			btnExit.Size = new Size(160, 30);
			btnExit.TabIndex = 6;
			btnExit.Text = "EXIT";
			btnExit.UseVisualStyleBackColor = false;
			// 
			// pnlBottom
			// 
			pnlBottom.BackColor = Color.Gainsboro;
			pnlBottom.Controls.Add(txtMessage);
			pnlBottom.Controls.Add(btnSend);
			pnlBottom.Controls.Add(btnFile);
			pnlBottom.Dock = DockStyle.Bottom;
			pnlBottom.Location = new Point(0, 416);
			pnlBottom.Name = "pnlBottom";
			pnlBottom.Size = new Size(640, 50);
			pnlBottom.TabIndex = 7;
			// 
			// pnlRight
			// 
			pnlRight.BackColor = Color.WhiteSmoke;
			pnlRight.Controls.Add(lstUsers);
			pnlRight.Controls.Add(label1);
			pnlRight.Controls.Add(btnExit);
			pnlRight.Dock = DockStyle.Right;
			pnlRight.Location = new Point(640, 0);
			pnlRight.Name = "pnlRight";
			pnlRight.Size = new Size(160, 466);
			pnlRight.TabIndex = 8;
			// 
			// pnlCenter
			// 
			pnlCenter.BackColor = Color.White;
			pnlCenter.Controls.Add(lstMessages);
			pnlCenter.Dock = DockStyle.Fill;
			pnlCenter.Location = new Point(0, 0);
			pnlCenter.Name = "pnlCenter";
			pnlCenter.Padding = new Padding(10);
			pnlCenter.Size = new Size(640, 416);
			pnlCenter.TabIndex = 9;
			// 
			// MainForm
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(800, 466);
			Controls.Add(pnlCenter);
			Controls.Add(pnlBottom);
			Controls.Add(pnlRight);
			Name = "MainForm";
			StartPosition = FormStartPosition.CenterScreen;
			Text = "ChatApp";
			pnlBottom.ResumeLayout(false);
			pnlBottom.PerformLayout();
			pnlRight.ResumeLayout(false);
			pnlCenter.ResumeLayout(false);
			ResumeLayout(false);
		}

		#endregion

		private ListBox lstMessages;
		private TextBox txtMessage;
		private Button btnSend;
		private Button btnFile;
		private Label label1;
		private ListBox lstUsers;
		private Button btnExit;
		private Panel pnlBottom;
		private Panel pnlRight;
		private Panel pnlCenter;
	}
}