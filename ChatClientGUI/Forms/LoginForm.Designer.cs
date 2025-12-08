namespace ChatClientGUI.Forms
{
	partial class LoginForm
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
			this.txtUsername = new System.Windows.Forms.TextBox();
			this.txtPassword = new System.Windows.Forms.TextBox();
			this.btnLogin = new System.Windows.Forms.Button();
			this.btnRegister = new System.Windows.Forms.Button();
			this.lblUser = new System.Windows.Forms.Label();
			this.lblPass = new System.Windows.Forms.Label();
			this.pnlHeader = new System.Windows.Forms.Panel();
			this.btnMinimize = new System.Windows.Forms.Button();
			this.btnClose = new System.Windows.Forms.Button();
			this.lblTitle = new System.Windows.Forms.Label();
			this.pnlHeader.SuspendLayout();
			this.SuspendLayout();
			
			this.txtUsername.Font = new System.Drawing.Font("Segoe UI", 11F);
			this.txtUsername.Location = new System.Drawing.Point(50, 80);
			this.txtUsername.Name = "txtUsername";
			this.txtUsername.Size = new System.Drawing.Size(250, 27);
			this.txtUsername.TabIndex = 0;
			
			this.txtPassword.Font = new System.Drawing.Font("Segoe UI", 11F);
			this.txtPassword.Location = new System.Drawing.Point(50, 150);
			this.txtPassword.Name = "txtPassword";
			this.txtPassword.PasswordChar = '•';
			this.txtPassword.Size = new System.Drawing.Size(250, 27);
			this.txtPassword.TabIndex = 1;
			
			this.btnLogin.BackColor = System.Drawing.Color.DodgerBlue;
			this.btnLogin.Cursor = System.Windows.Forms.Cursors.Hand;
			this.btnLogin.FlatAppearance.BorderSize = 0;
			this.btnLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnLogin.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
			this.btnLogin.ForeColor = System.Drawing.Color.White;
			this.btnLogin.Location = new System.Drawing.Point(50, 210);
			this.btnLogin.Name = "btnLogin";
			this.btnLogin.Size = new System.Drawing.Size(115, 40);
			this.btnLogin.TabIndex = 2;
			this.btnLogin.Text = "Login";
			this.btnLogin.UseVisualStyleBackColor = false;
			
			this.btnRegister.BackColor = System.Drawing.Color.Gray;
			this.btnRegister.Cursor = System.Windows.Forms.Cursors.Hand;
			this.btnRegister.FlatAppearance.BorderSize = 0;
			this.btnRegister.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnRegister.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
			this.btnRegister.ForeColor = System.Drawing.Color.White;
			this.btnRegister.Location = new System.Drawing.Point(185, 210);
			this.btnRegister.Name = "btnRegister";
			this.btnRegister.Size = new System.Drawing.Size(115, 40);
			this.btnRegister.TabIndex = 3;
			this.btnRegister.Text = "Register";
			this.btnRegister.UseVisualStyleBackColor = false;
			
			this.lblUser.AutoSize = true;
			this.lblUser.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			this.lblUser.ForeColor = System.Drawing.Color.Gray;
			this.lblUser.Location = new System.Drawing.Point(50, 60);
			this.lblUser.Name = "lblUser";
			this.lblUser.Size = new System.Drawing.Size(64, 15);
			this.lblUser.TabIndex = 4;
			this.lblUser.Text = "Username";
			
			this.lblPass.AutoSize = true;
			this.lblPass.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			this.lblPass.ForeColor = System.Drawing.Color.Gray;
			this.lblPass.Location = new System.Drawing.Point(50, 130);
			this.lblPass.Name = "lblPass";
			this.lblPass.Size = new System.Drawing.Size(59, 15);
			this.lblPass.TabIndex = 5;
			this.lblPass.Text = "Password";
			
			this.pnlHeader.BackColor = System.Drawing.Color.WhiteSmoke;
			this.pnlHeader.Controls.Add(this.btnMinimize);
			this.pnlHeader.Controls.Add(this.btnClose);
			this.pnlHeader.Controls.Add(this.lblTitle);
			this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlHeader.Location = new System.Drawing.Point(0, 0);
			this.pnlHeader.Name = "pnlHeader";
			this.pnlHeader.Size = new System.Drawing.Size(350, 40);
			this.pnlHeader.TabIndex = 6;
			
			this.btnMinimize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnMinimize.Cursor = System.Windows.Forms.Cursors.Hand;
			this.btnMinimize.FlatAppearance.BorderSize = 0;
			this.btnMinimize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnMinimize.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			this.btnMinimize.Location = new System.Drawing.Point(270, 0);
			this.btnMinimize.Name = "btnMinimize";
			this.btnMinimize.Size = new System.Drawing.Size(40, 40);
			this.btnMinimize.TabIndex = 2;
			this.btnMinimize.Text = "−";
			this.btnMinimize.UseVisualStyleBackColor = true;
			
			this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
			this.btnClose.FlatAppearance.BorderSize = 0;
			this.btnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.IndianRed;
			this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnClose.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.btnClose.Location = new System.Drawing.Point(310, 0);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(40, 40);
			this.btnClose.TabIndex = 1;
			this.btnClose.Text = "✕";
			this.btnClose.UseVisualStyleBackColor = true;
			
			this.lblTitle.AutoSize = true;
			this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
			this.lblTitle.ForeColor = System.Drawing.Color.DimGray;
			this.lblTitle.Location = new System.Drawing.Point(12, 10);
			this.lblTitle.Name = "lblTitle";
			this.lblTitle.Size = new System.Drawing.Size(46, 19);
			this.lblTitle.TabIndex = 0;
			this.lblTitle.Text = "Login";
			
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.ClientSize = new System.Drawing.Size(350, 300);
			this.Controls.Add(this.pnlHeader);
			this.Controls.Add(this.lblPass);
			this.Controls.Add(this.lblUser);
			this.Controls.Add(this.btnRegister);
			this.Controls.Add(this.btnLogin);
			this.Controls.Add(this.txtPassword);
			this.Controls.Add(this.txtUsername);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "LoginForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "ChatApp - Login";
			this.pnlHeader.ResumeLayout(false);
			this.pnlHeader.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		#endregion

		private System.Windows.Forms.TextBox txtUsername;
		private System.Windows.Forms.TextBox txtPassword;
		private System.Windows.Forms.Button btnLogin;
		private System.Windows.Forms.Button btnRegister;
		private System.Windows.Forms.Label lblUser;
		private System.Windows.Forms.Label lblPass;
		private System.Windows.Forms.Panel pnlHeader;
		private System.Windows.Forms.Label lblTitle;
		private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.Button btnMinimize;
	}
}