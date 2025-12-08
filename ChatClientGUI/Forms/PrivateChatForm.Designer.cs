namespace ChatClientGUI.Forms
{
	partial class PrivateChatForm
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
			this.lstPrivateMessages = new System.Windows.Forms.ListBox();
			this.txtPrivateInput = new System.Windows.Forms.TextBox();
			this.btnSendPrivate = new System.Windows.Forms.Button();
			this.pnlBottom = new System.Windows.Forms.Panel();
			this.pnlCenter = new System.Windows.Forms.Panel();
			this.pnlHeader = new System.Windows.Forms.Panel();
			this.lblTitle = new System.Windows.Forms.Label();
			this.btnCloseApp = new System.Windows.Forms.Button();
			this.btnMinimize = new System.Windows.Forms.Button();
			this.pnlBottom.SuspendLayout();
			this.pnlCenter.SuspendLayout();
			this.pnlHeader.SuspendLayout();
			this.SuspendLayout();
			// 
			// lstPrivateMessages
			// 
			this.lstPrivateMessages.BackColor = System.Drawing.Color.White;
			this.lstPrivateMessages.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.lstPrivateMessages.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lstPrivateMessages.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
			this.lstPrivateMessages.Font = new System.Drawing.Font("Segoe UI", 10F);
			this.lstPrivateMessages.FormattingEnabled = true;
			this.lstPrivateMessages.IntegralHeight = false;
			this.lstPrivateMessages.ItemHeight = 35;
			this.lstPrivateMessages.Location = new System.Drawing.Point(10, 10);
			this.lstPrivateMessages.Name = "lstPrivateMessages";
			this.lstPrivateMessages.Size = new System.Drawing.Size(564, 301);
			this.lstPrivateMessages.TabIndex = 0;
			// 
			// txtPrivateInput
			// 
			this.txtPrivateInput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
			| System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right)));
			this.txtPrivateInput.Font = new System.Drawing.Font("Segoe UI", 10F);
			this.txtPrivateInput.Location = new System.Drawing.Point(10, 10);
			this.txtPrivateInput.Multiline = true;
			this.txtPrivateInput.Name = "txtPrivateInput";
			this.txtPrivateInput.Size = new System.Drawing.Size(469, 30);
			this.txtPrivateInput.TabIndex = 1;
			// 
			// btnSendPrivate
			// 
			this.btnSendPrivate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSendPrivate.BackColor = System.Drawing.Color.DodgerBlue;
			this.btnSendPrivate.Cursor = System.Windows.Forms.Cursors.Hand;
			this.btnSendPrivate.FlatAppearance.BorderSize = 0;
			this.btnSendPrivate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnSendPrivate.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			this.btnSendPrivate.ForeColor = System.Drawing.Color.White;
			this.btnSendPrivate.Location = new System.Drawing.Point(485, 10);
			this.btnSendPrivate.Name = "btnSendPrivate";
			this.btnSendPrivate.Size = new System.Drawing.Size(89, 30);
			this.btnSendPrivate.TabIndex = 2;
			this.btnSendPrivate.Text = "SEND";
			this.btnSendPrivate.UseVisualStyleBackColor = false;
			// 
			// pnlBottom
			// 
			this.pnlBottom.BackColor = System.Drawing.Color.Gainsboro;
			this.pnlBottom.Controls.Add(this.txtPrivateInput);
			this.pnlBottom.Controls.Add(this.btnSendPrivate);
			this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlBottom.Location = new System.Drawing.Point(0, 321);
			this.pnlBottom.Name = "pnlBottom";
			this.pnlBottom.Size = new System.Drawing.Size(584, 50);
			this.pnlBottom.TabIndex = 3;
			// 
			// pnlCenter
			// 
			this.pnlCenter.BackColor = System.Drawing.Color.White;
			this.pnlCenter.Controls.Add(this.lstPrivateMessages);
			this.pnlCenter.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlCenter.Location = new System.Drawing.Point(0, 40);
			this.pnlCenter.Name = "pnlCenter";
			this.pnlCenter.Padding = new System.Windows.Forms.Padding(10);
			this.pnlCenter.Size = new System.Drawing.Size(584, 281);
			this.pnlCenter.TabIndex = 4;
			// 
			// pnlHeader
			// 
			this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(246)))), ((int)(((byte)(247)))));
			this.pnlHeader.Controls.Add(this.btnMinimize);
			this.pnlHeader.Controls.Add(this.btnCloseApp);
			this.pnlHeader.Controls.Add(this.lblTitle);
			this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlHeader.Location = new System.Drawing.Point(0, 0);
			this.pnlHeader.Name = "pnlHeader";
			this.pnlHeader.Size = new System.Drawing.Size(584, 40);
			this.pnlHeader.TabIndex = 5;
			// 
			// lblTitle
			// 
			this.lblTitle.AutoSize = true;
			this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
			this.lblTitle.ForeColor = System.Drawing.Color.DimGray;
			this.lblTitle.Location = new System.Drawing.Point(12, 10);
			this.lblTitle.Name = "lblTitle";
			this.lblTitle.Size = new System.Drawing.Size(89, 19);
			this.lblTitle.TabIndex = 0;
			this.lblTitle.Text = "Private Chat";
			// 
			// btnCloseApp
			// 
			this.btnCloseApp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCloseApp.Cursor = System.Windows.Forms.Cursors.Hand;
			this.btnCloseApp.FlatAppearance.BorderSize = 0;
			this.btnCloseApp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnCloseApp.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			this.btnCloseApp.Location = new System.Drawing.Point(544, 0);
			this.btnCloseApp.Name = "btnCloseApp";
			this.btnCloseApp.Size = new System.Drawing.Size(40, 40);
			this.btnCloseApp.TabIndex = 1;
			this.btnCloseApp.Text = "X";
			this.btnCloseApp.UseVisualStyleBackColor = true;
			// 
			// btnMinimize
			// 
			this.btnMinimize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnMinimize.Cursor = System.Windows.Forms.Cursors.Hand;
			this.btnMinimize.FlatAppearance.BorderSize = 0;
			this.btnMinimize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnMinimize.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			this.btnMinimize.Location = new System.Drawing.Point(504, 0);
			this.btnMinimize.Name = "btnMinimize";
			this.btnMinimize.Size = new System.Drawing.Size(40, 40);
			this.btnMinimize.TabIndex = 2;
			this.btnMinimize.Text = "_";
			this.btnMinimize.UseVisualStyleBackColor = true;
			// 
			// PrivateChatForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(584, 371);
			this.Controls.Add(this.pnlCenter);
			this.Controls.Add(this.pnlBottom);
			this.Controls.Add(this.pnlHeader);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "PrivateChatForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Private Chat";
			this.pnlBottom.ResumeLayout(false);
			this.pnlBottom.PerformLayout();
			this.pnlCenter.ResumeLayout(false);
			this.pnlHeader.ResumeLayout(false);
			this.pnlHeader.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListBox lstPrivateMessages;
		private System.Windows.Forms.TextBox txtPrivateInput;
		private System.Windows.Forms.Button btnSendPrivate;
		private System.Windows.Forms.Panel pnlBottom;
		private System.Windows.Forms.Panel pnlCenter;
		private System.Windows.Forms.Panel pnlHeader;
		private System.Windows.Forms.Label lblTitle;
		private System.Windows.Forms.Button btnCloseApp;
		private System.Windows.Forms.Button btnMinimize;
	}
}