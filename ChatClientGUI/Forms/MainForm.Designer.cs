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
			this.lstMessages = new System.Windows.Forms.ListBox();
			this.txtMessage = new System.Windows.Forms.TextBox();
			this.btnSend = new System.Windows.Forms.Button();
			this.btnFile = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.lstUsers = new System.Windows.Forms.ListBox();
			this.btnExit = new System.Windows.Forms.Button();
			this.pnlBottom = new System.Windows.Forms.Panel();
			this.pnlRight = new System.Windows.Forms.Panel();
			this.pnlCenter = new System.Windows.Forms.Panel();
			this.pnlBottom.SuspendLayout();
			this.pnlRight.SuspendLayout();
			this.pnlCenter.SuspendLayout();
			this.SuspendLayout();
			// 
			// lstMessages
			// 
			this.lstMessages.BackColor = System.Drawing.Color.White;
			this.lstMessages.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.lstMessages.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lstMessages.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
			this.lstMessages.Font = new System.Drawing.Font("Segoe UI", 10F);
			this.lstMessages.FormattingEnabled = true;
			this.lstMessages.IntegralHeight = false;
			this.lstMessages.ItemHeight = 35;
			this.lstMessages.Location = new System.Drawing.Point(10, 10);
			this.lstMessages.Name = "lstMessages";
			this.lstMessages.Size = new System.Drawing.Size(560, 396);
			this.lstMessages.TabIndex = 0;
			// 
			// txtMessage
			// 
			this.txtMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
			| System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right)));
			this.txtMessage.Font = new System.Drawing.Font("Segoe UI", 10F);
			this.txtMessage.Location = new System.Drawing.Point(10, 10);
			this.txtMessage.Multiline = true;
			this.txtMessage.Name = "txtMessage";
			this.txtMessage.Size = new System.Drawing.Size(465, 30);
			this.txtMessage.TabIndex = 1;
			// 
			// btnSend
			// 
			this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSend.BackColor = System.Drawing.Color.DodgerBlue;
			this.btnSend.Cursor = System.Windows.Forms.Cursors.Hand;
			this.btnSend.FlatAppearance.BorderSize = 0;
			this.btnSend.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnSend.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			this.btnSend.ForeColor = System.Drawing.Color.White;
			this.btnSend.Location = new System.Drawing.Point(481, 10);
			this.btnSend.Name = "btnSend";
			this.btnSend.Size = new System.Drawing.Size(80, 30);
			this.btnSend.TabIndex = 2;
			this.btnSend.Text = "SEND";
			this.btnSend.UseVisualStyleBackColor = false;
			// 
			// btnFile
			// 
			this.btnFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnFile.BackColor = System.Drawing.Color.Gray;
			this.btnFile.Cursor = System.Windows.Forms.Cursors.Hand;
			this.btnFile.FlatAppearance.BorderSize = 0;
			this.btnFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnFile.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			this.btnFile.ForeColor = System.Drawing.Color.White;
			this.btnFile.Location = new System.Drawing.Point(567, 10);
			this.btnFile.Name = "btnFile";
			this.btnFile.Size = new System.Drawing.Size(50, 30);
			this.btnFile.TabIndex = 3;
			this.btnFile.Text = "FILE";
			this.btnFile.UseVisualStyleBackColor = false;
			// 
			// label1
			// 
			this.label1.Dock = System.Windows.Forms.DockStyle.Top;
			this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			this.label1.ForeColor = System.Drawing.Color.DimGray;
			this.label1.Location = new System.Drawing.Point(0, 0);
			this.label1.Name = "label1";
			this.label1.Padding = new System.Windows.Forms.Padding(5, 5, 0, 0);
			this.label1.Size = new System.Drawing.Size(160, 30);
			this.label1.TabIndex = 4;
			this.label1.Text = "ONLINE USERS";
			// 
			// lstUsers
			// 
			this.lstUsers.BackColor = System.Drawing.Color.WhiteSmoke;
			this.lstUsers.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.lstUsers.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lstUsers.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.lstUsers.FormattingEnabled = true;
			this.lstUsers.IntegralHeight = false;
			this.lstUsers.ItemHeight = 15;
			this.lstUsers.Location = new System.Drawing.Point(0, 30);
			this.lstUsers.Name = "lstUsers";
			this.lstUsers.Size = new System.Drawing.Size(160, 386);
			this.lstUsers.TabIndex = 5;
			// 
			// btnExit
			// 
			this.btnExit.BackColor = System.Drawing.Color.IndianRed;
			this.btnExit.Cursor = System.Windows.Forms.Cursors.Hand;
			this.btnExit.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.btnExit.FlatAppearance.BorderSize = 0;
			this.btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnExit.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			this.btnExit.ForeColor = System.Drawing.Color.White;
			this.btnExit.Location = new System.Drawing.Point(0, 416);
			this.btnExit.Name = "btnExit";
			this.btnExit.Size = new System.Drawing.Size(160, 30);
			this.btnExit.TabIndex = 6;
			this.btnExit.Text = "EXIT";
			this.btnExit.UseVisualStyleBackColor = false;
			// 
			// pnlBottom
			// 
			this.pnlBottom.BackColor = System.Drawing.Color.Gainsboro;
			this.pnlBottom.Controls.Add(this.txtMessage);
			this.pnlBottom.Controls.Add(this.btnSend);
			this.pnlBottom.Controls.Add(this.btnFile);
			this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlBottom.Location = new System.Drawing.Point(0, 416);
			this.pnlBottom.Name = "pnlBottom";
			this.pnlBottom.Size = new System.Drawing.Size(640, 50);
			this.pnlBottom.TabIndex = 7;
			// 
			// pnlRight
			// 
			this.pnlRight.BackColor = System.Drawing.Color.WhiteSmoke;
			this.pnlRight.Controls.Add(this.lstUsers);
			this.pnlRight.Controls.Add(this.label1);
			this.pnlRight.Controls.Add(this.btnExit);
			this.pnlRight.Dock = System.Windows.Forms.DockStyle.Right;
			this.pnlRight.Location = new System.Drawing.Point(640, 0);
			this.pnlRight.Name = "pnlRight";
			this.pnlRight.Size = new System.Drawing.Size(160, 466);
			this.pnlRight.TabIndex = 8;
			// 
			// pnlCenter
			// 
			this.pnlCenter.BackColor = System.Drawing.Color.White;
			this.pnlCenter.Controls.Add(this.lstMessages);
			this.pnlCenter.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlCenter.Location = new System.Drawing.Point(0, 0);
			this.pnlCenter.Name = "pnlCenter";
			this.pnlCenter.Padding = new System.Windows.Forms.Padding(10);
			this.pnlCenter.Size = new System.Drawing.Size(640, 416);
			this.pnlCenter.TabIndex = 9;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 466);
			this.Controls.Add(this.pnlCenter);
			this.Controls.Add(this.pnlBottom);
			this.Controls.Add(this.pnlRight);
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "ChatApp";
			this.pnlBottom.ResumeLayout(false);
			this.pnlBottom.PerformLayout();
			this.pnlRight.ResumeLayout(false);
			this.pnlCenter.ResumeLayout(false);
			this.ResumeLayout(false);
		}

		#endregion

		// DEKLARÁCIÓK - csak itt lehetnek!
		private System.Windows.Forms.ListBox lstMessages;
		private System.Windows.Forms.TextBox txtMessage;
		private System.Windows.Forms.Button btnSend;
		private System.Windows.Forms.Button btnFile;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ListBox lstUsers;
		private System.Windows.Forms.Button btnExit;
		private System.Windows.Forms.Panel pnlBottom;
		private System.Windows.Forms.Panel pnlRight;
		private System.Windows.Forms.Panel pnlCenter;
	}
}