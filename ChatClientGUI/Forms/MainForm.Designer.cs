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
			this.pnlBottom = new System.Windows.Forms.Panel();
			this.pnlRight = new System.Windows.Forms.Panel();
			this.pnlCenter = new System.Windows.Forms.Panel();
			this.pnlHeader = new System.Windows.Forms.Panel();
			this.lblTitle = new System.Windows.Forms.Label();
			this.btnCloseApp = new System.Windows.Forms.Button();
			this.btnMinimize = new System.Windows.Forms.Button();
			this.pnlBottom.SuspendLayout();
			this.pnlRight.SuspendLayout();
			this.pnlCenter.SuspendLayout();
			this.pnlHeader.SuspendLayout();
			this.SuspendLayout();
			

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
			this.lstMessages.SelectionMode = System.Windows.Forms.SelectionMode.None;
			this.lstMessages.Size = new System.Drawing.Size(560, 396);
			this.lstMessages.TabIndex = 0;
			

			this.txtMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
			| System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right)));
			this.txtMessage.Font = new System.Drawing.Font("Segoe UI", 11F);
			this.txtMessage.Location = new System.Drawing.Point(50, 15);
			this.txtMessage.Multiline = true;
			this.txtMessage.Name = "txtMessage";
			this.txtMessage.Size = new System.Drawing.Size(475, 25);
			this.txtMessage.TabIndex = 1;
			

			this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSend.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
			this.btnSend.Cursor = System.Windows.Forms.Cursors.Hand;
			this.btnSend.FlatAppearance.BorderSize = 0;
			this.btnSend.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnSend.Font = new System.Drawing.Font("Segoe UI Symbol", 12F, System.Drawing.FontStyle.Bold);
			this.btnSend.ForeColor = System.Drawing.Color.White;
			this.btnSend.Location = new System.Drawing.Point(588, 5);
			this.btnSend.Name = "btnSend";
			this.btnSend.Size = new System.Drawing.Size(40, 40);
			this.btnSend.TabIndex = 2;
			this.btnSend.Text = "➤";
			this.btnSend.UseVisualStyleBackColor = false;
			

			this.btnFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnFile.BackColor = System.Drawing.Color.WhiteSmoke;
			this.btnFile.Cursor = System.Windows.Forms.Cursors.Hand;
			this.btnFile.FlatAppearance.BorderSize = 0;
			this.btnFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnFile.Font = new System.Drawing.Font("Segoe UI Symbol", 12F);
			this.btnFile.ForeColor = System.Drawing.Color.DimGray;
			this.btnFile.Location = new System.Drawing.Point(542, 5);
			this.btnFile.Name = "btnFile";
			this.btnFile.Size = new System.Drawing.Size(40, 40);
			this.btnFile.TabIndex = 3;
			this.btnFile.Text = "📎";
			this.btnFile.UseVisualStyleBackColor = false;
			

			this.label1.Dock = System.Windows.Forms.DockStyle.Top;
			this.label1.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
			this.label1.ForeColor = System.Drawing.Color.Gray;
			this.label1.Location = new System.Drawing.Point(0, 0);
			this.label1.Name = "label1";
			this.label1.Padding = new System.Windows.Forms.Padding(10, 10, 0, 0);
			this.label1.Size = new System.Drawing.Size(160, 30);
			this.label1.TabIndex = 4;
			this.label1.Text = "CONTACTS";
			

			this.lstUsers.BackColor = System.Drawing.Color.White;
			this.lstUsers.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.lstUsers.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lstUsers.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.lstUsers.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.lstUsers.FormattingEnabled = true;
			this.lstUsers.IntegralHeight = false;
			this.lstUsers.ItemHeight = 35;
			this.lstUsers.Location = new System.Drawing.Point(0, 30);
			this.lstUsers.Name = "lstUsers";
			this.lstUsers.Size = new System.Drawing.Size(160, 436);
			this.lstUsers.TabIndex = 5;
			

			this.pnlBottom.BackColor = System.Drawing.Color.White;
			this.pnlBottom.Controls.Add(this.txtMessage);
			this.pnlBottom.Controls.Add(this.btnSend);
			this.pnlBottom.Controls.Add(this.btnFile);
			this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlBottom.Location = new System.Drawing.Point(0, 416);
			this.pnlBottom.Name = "pnlBottom";
			this.pnlBottom.Size = new System.Drawing.Size(640, 50);
			this.pnlBottom.TabIndex = 7;
			

			this.pnlRight.BackColor = System.Drawing.Color.White;
			this.pnlRight.Controls.Add(this.lstUsers);
			this.pnlRight.Controls.Add(this.label1);
			this.pnlRight.Dock = System.Windows.Forms.DockStyle.Right;
			this.pnlRight.Location = new System.Drawing.Point(640, 40);
			this.pnlRight.Name = "pnlRight";
			this.pnlRight.Size = new System.Drawing.Size(160, 466);
			this.pnlRight.TabIndex = 8;
			

			this.pnlCenter.BackColor = System.Drawing.Color.White;
			this.pnlCenter.Controls.Add(this.lstMessages);
			this.pnlCenter.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlCenter.Location = new System.Drawing.Point(0, 40);
			this.pnlCenter.Name = "pnlCenter";
			this.pnlCenter.Padding = new System.Windows.Forms.Padding(10);
			this.pnlCenter.Size = new System.Drawing.Size(640, 376);
			this.pnlCenter.TabIndex = 9;
			

			this.pnlHeader.BackColor = System.Drawing.Color.WhiteSmoke;
			this.pnlHeader.Controls.Add(this.btnMinimize);
			this.pnlHeader.Controls.Add(this.btnCloseApp);
			this.pnlHeader.Controls.Add(this.lblTitle);
			this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlHeader.Location = new System.Drawing.Point(0, 0);
			this.pnlHeader.Name = "pnlHeader";
			this.pnlHeader.Size = new System.Drawing.Size(800, 40);
			this.pnlHeader.TabIndex = 10;
			

			this.lblTitle.AutoSize = true;
			this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
			this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.lblTitle.Location = new System.Drawing.Point(12, 10);
			this.lblTitle.Name = "lblTitle";
			this.lblTitle.Size = new System.Drawing.Size(69, 19);
			this.lblTitle.TabIndex = 0;
			this.lblTitle.Text = "ChatApp";
			

			this.btnCloseApp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCloseApp.Cursor = System.Windows.Forms.Cursors.Hand;
			this.btnCloseApp.FlatAppearance.BorderSize = 0;
			this.btnCloseApp.FlatAppearance.MouseOverBackColor = System.Drawing.Color.IndianRed;
			this.btnCloseApp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnCloseApp.Font = new System.Drawing.Font("Segoe UI", 10F);
			this.btnCloseApp.Location = new System.Drawing.Point(760, 0);
			this.btnCloseApp.Name = "btnCloseApp";
			this.btnCloseApp.Size = new System.Drawing.Size(40, 40);
			this.btnCloseApp.TabIndex = 1;
			this.btnCloseApp.Text = "✕";
			this.btnCloseApp.UseVisualStyleBackColor = true;
			

			this.btnMinimize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnMinimize.Cursor = System.Windows.Forms.Cursors.Hand;
			this.btnMinimize.FlatAppearance.BorderSize = 0;
			this.btnMinimize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnMinimize.Font = new System.Drawing.Font("Segoe UI", 10F);
			this.btnMinimize.Location = new System.Drawing.Point(720, 0);
			this.btnMinimize.Name = "btnMinimize";
			this.btnMinimize.Size = new System.Drawing.Size(40, 40);
			this.btnMinimize.TabIndex = 2;
			this.btnMinimize.Text = "−";
			this.btnMinimize.UseVisualStyleBackColor = true;
			

			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 506);
			this.Controls.Add(this.pnlCenter);
			this.Controls.Add(this.pnlRight);
			this.Controls.Add(this.pnlBottom);
			this.Controls.Add(this.pnlHeader);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "ChatApp";
			this.pnlBottom.ResumeLayout(false);
			this.pnlBottom.PerformLayout();
			this.pnlRight.ResumeLayout(false);
			this.pnlCenter.ResumeLayout(false);
			this.pnlHeader.ResumeLayout(false);
			this.pnlHeader.PerformLayout();
			this.ResumeLayout(false);
		}

		#endregion

		private System.Windows.Forms.ListBox lstMessages;
		private System.Windows.Forms.TextBox txtMessage;
		private System.Windows.Forms.Button btnSend;
		private System.Windows.Forms.Button btnFile;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ListBox lstUsers;
		private System.Windows.Forms.Panel pnlBottom;
		private System.Windows.Forms.Panel pnlRight;
		private System.Windows.Forms.Panel pnlCenter;
		private System.Windows.Forms.Panel pnlHeader;
		private System.Windows.Forms.Label lblTitle;
		private System.Windows.Forms.Button btnCloseApp;
		private System.Windows.Forms.Button btnMinimize;
	}
}