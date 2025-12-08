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
			lstPrivateMessages = new ListBox();
			txtPrivateInput = new TextBox();
			btnSendPrivate = new Button();
			pnlBottom = new Panel();
			pnlCenter = new Panel();
			pnlBottom.SuspendLayout();
			pnlCenter.SuspendLayout();
			SuspendLayout();
			// 
			// lstPrivateMessages
			// 
			lstPrivateMessages.BackColor = Color.White;
			lstPrivateMessages.BorderStyle = BorderStyle.None;
			lstPrivateMessages.Dock = DockStyle.Fill;
			lstPrivateMessages.Font = new Font("Segoe UI", 10F);
			lstPrivateMessages.FormattingEnabled = true;
			lstPrivateMessages.IntegralHeight = false;
			lstPrivateMessages.ItemHeight = 17;
			lstPrivateMessages.Location = new Point(10, 10);
			lstPrivateMessages.Name = "lstPrivateMessages";
			lstPrivateMessages.Size = new Size(564, 301);
			lstPrivateMessages.TabIndex = 0;
			// 
			// txtPrivateInput
			// 
			txtPrivateInput.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			txtPrivateInput.Font = new Font("Segoe UI", 10F);
			txtPrivateInput.Location = new Point(10, 10);
			txtPrivateInput.Multiline = true;
			txtPrivateInput.Name = "txtPrivateInput";
			txtPrivateInput.Size = new Size(469, 30);
			txtPrivateInput.TabIndex = 1;
			// 
			// btnSendPrivate
			// 
			btnSendPrivate.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			btnSendPrivate.BackColor = Color.DodgerBlue;
			btnSendPrivate.Cursor = Cursors.Hand;
			btnSendPrivate.FlatAppearance.BorderSize = 0;
			btnSendPrivate.FlatStyle = FlatStyle.Flat;
			btnSendPrivate.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
			btnSendPrivate.ForeColor = Color.White;
			btnSendPrivate.Location = new Point(485, 10);
			btnSendPrivate.Name = "btnSendPrivate";
			btnSendPrivate.Size = new Size(89, 30);
			btnSendPrivate.TabIndex = 2;
			btnSendPrivate.Text = "SEND";
			btnSendPrivate.UseVisualStyleBackColor = false;
			// 
			// pnlBottom
			// 
			pnlBottom.BackColor = Color.Gainsboro;
			pnlBottom.Controls.Add(txtPrivateInput);
			pnlBottom.Controls.Add(btnSendPrivate);
			pnlBottom.Dock = DockStyle.Bottom;
			pnlBottom.Location = new Point(0, 321);
			pnlBottom.Name = "pnlBottom";
			pnlBottom.Size = new Size(584, 50);
			pnlBottom.TabIndex = 3;
			// 
			// pnlCenter
			// 
			pnlCenter.BackColor = Color.White;
			pnlCenter.Controls.Add(lstPrivateMessages);
			pnlCenter.Dock = DockStyle.Fill;
			pnlCenter.Location = new Point(0, 0);
			pnlCenter.Name = "pnlCenter";
			pnlCenter.Padding = new Padding(10);
			pnlCenter.Size = new Size(584, 321);
			pnlCenter.TabIndex = 4;
			// 
			// PrivateChatForm
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(584, 371);
			Controls.Add(pnlCenter);
			Controls.Add(pnlBottom);
			Name = "PrivateChatForm";
			StartPosition = FormStartPosition.CenterScreen;
			Text = "Private Chat";
			pnlBottom.ResumeLayout(false);
			pnlBottom.PerformLayout();
			pnlCenter.ResumeLayout(false);
			ResumeLayout(false);
		}

		#endregion

		private ListBox lstPrivateMessages;
		private TextBox txtPrivateInput;
		private Button btnSendPrivate;
		private Panel pnlBottom;
		private Panel pnlCenter;
	}
}