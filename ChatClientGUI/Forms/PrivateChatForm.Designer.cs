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
			this.pnlBottom.SuspendLayout();
			this.pnlCenter.SuspendLayout();
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
			this.pnlCenter.Location = new System.Drawing.Point(0, 0);
			this.pnlCenter.Name = "pnlCenter";
			this.pnlCenter.Padding = new System.Windows.Forms.Padding(10);
			this.pnlCenter.Size = new System.Drawing.Size(584, 321);
			this.pnlCenter.TabIndex = 4;
			// 
			// PrivateChatForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(584, 371);
			this.Controls.Add(this.pnlCenter);
			this.Controls.Add(this.pnlBottom);
			this.Name = "PrivateChatForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Private Chat";
			this.pnlBottom.ResumeLayout(false);
			this.pnlBottom.PerformLayout();
			this.pnlCenter.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListBox lstPrivateMessages;
		private System.Windows.Forms.TextBox txtPrivateInput;
		private System.Windows.Forms.Button btnSendPrivate;
		private System.Windows.Forms.Panel pnlBottom;
		private System.Windows.Forms.Panel pnlCenter;
	}
}