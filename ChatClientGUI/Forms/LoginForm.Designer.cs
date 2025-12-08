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
			this.pnlMain = new System.Windows.Forms.Panel();
			this.pnlMain.SuspendLayout();
			this.SuspendLayout();
			
			this.txtUsername.Font = new System.Drawing.Font("Segoe UI", 10F);
			this.txtUsername.Location = new System.Drawing.Point(35, 45);
			this.txtUsername.Name = "txtUsername";
			this.txtUsername.Size = new System.Drawing.Size(230, 25);
			this.txtUsername.TabIndex = 0;
			
			this.txtPassword.Font = new System.Drawing.Font("Segoe UI", 10F);
			this.txtPassword.Location = new System.Drawing.Point(35, 105);
			this.txtPassword.Name = "txtPassword";
			this.txtPassword.PasswordChar = '•';
			this.txtPassword.Size = new System.Drawing.Size(230, 25);
			this.txtPassword.TabIndex = 1;
			
			this.btnLogin.BackColor = System.Drawing.Color.DodgerBlue;
			this.btnLogin.Cursor = System.Windows.Forms.Cursors.Hand;
			this.btnLogin.FlatAppearance.BorderSize = 0;
			this.btnLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnLogin.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
			this.btnLogin.ForeColor = System.Drawing.Color.White;
			this.btnLogin.Location = new System.Drawing.Point(35, 150);
			this.btnLogin.Name = "btnLogin";
			this.btnLogin.Size = new System.Drawing.Size(110, 35);
			this.btnLogin.TabIndex = 2;
			this.btnLogin.Text = "Login";
			this.btnLogin.UseVisualStyleBackColor = false;
			
			this.btnRegister.BackColor = System.Drawing.Color.ForestGreen;
			this.btnRegister.Cursor = System.Windows.Forms.Cursors.Hand;
			this.btnRegister.FlatAppearance.BorderSize = 0;
			this.btnRegister.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnRegister.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
			this.btnRegister.ForeColor = System.Drawing.Color.White;
			this.btnRegister.Location = new System.Drawing.Point(155, 150);
			this.btnRegister.Name = "btnRegister";
			this.btnRegister.Size = new System.Drawing.Size(110, 35);
			this.btnRegister.TabIndex = 3;
			this.btnRegister.Text = "Register";
			this.btnRegister.UseVisualStyleBackColor = false;
			
			this.lblUser.AutoSize = true;
			this.lblUser.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.lblUser.ForeColor = System.Drawing.Color.Gray;
			this.lblUser.Location = new System.Drawing.Point(35, 27);
			this.lblUser.Name = "lblUser";
			this.lblUser.Size = new System.Drawing.Size(60, 15);
			this.lblUser.TabIndex = 4;
			this.lblUser.Text = "Username";
			
			this.lblPass.AutoSize = true;
			this.lblPass.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.lblPass.ForeColor = System.Drawing.Color.Gray;
			this.lblPass.Location = new System.Drawing.Point(35, 87);
			this.lblPass.Name = "lblPass";
			this.lblPass.Size = new System.Drawing.Size(57, 15);
			this.lblPass.TabIndex = 5;
			this.lblPass.Text = "Password";
			
			this.pnlMain.BackColor = System.Drawing.Color.White;
			this.pnlMain.Controls.Add(this.lblUser);
			this.pnlMain.Controls.Add(this.btnRegister);
			this.pnlMain.Controls.Add(this.txtUsername);
			this.pnlMain.Controls.Add(this.btnLogin);
			this.pnlMain.Controls.Add(this.lblPass);
			this.pnlMain.Controls.Add(this.txtPassword);
			this.pnlMain.Location = new System.Drawing.Point(48, 41);
			this.pnlMain.Name = "pnlMain";
			this.pnlMain.Size = new System.Drawing.Size(300, 220);
			this.pnlMain.TabIndex = 6;
			
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.WhiteSmoke;
			this.ClientSize = new System.Drawing.Size(399, 311);
			this.Controls.Add(this.pnlMain);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "LoginForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "ChatApp - Login";
			this.pnlMain.ResumeLayout(false);
			this.pnlMain.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TextBox txtUsername;
		private System.Windows.Forms.TextBox txtPassword;
		private System.Windows.Forms.Button btnLogin;
		private System.Windows.Forms.Button btnRegister;
		private System.Windows.Forms.Label lblUser;
		private System.Windows.Forms.Label lblPass;
		private System.Windows.Forms.Panel pnlMain;
	}
}