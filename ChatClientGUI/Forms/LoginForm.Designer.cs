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
			txtUsername = new TextBox();
			txtPassword = new TextBox();
			btnLogin = new Button();
			btnRegister = new Button();
			lblUser = new Label();
			lblPass = new Label();
			pnlMain = new Panel();
			pnlMain.SuspendLayout();
			SuspendLayout();
			// 
			// txtUsername
			// 
			txtUsername.Font = new Font("Segoe UI", 10F);
			txtUsername.Location = new Point(35, 45);
			txtUsername.Name = "txtUsername";
			txtUsername.Size = new Size(230, 25);
			txtUsername.TabIndex = 0;
			// 
			// txtPassword
			// 
			txtPassword.Font = new Font("Segoe UI", 10F);
			txtPassword.Location = new Point(35, 105);
			txtPassword.Name = "txtPassword";
			txtPassword.PasswordChar = '•';
			txtPassword.Size = new Size(230, 25);
			txtPassword.TabIndex = 1;
			// 
			// btnLogin
			// 
			btnLogin.BackColor = Color.DodgerBlue;
			btnLogin.Cursor = Cursors.Hand;
			btnLogin.FlatAppearance.BorderSize = 0;
			btnLogin.FlatStyle = FlatStyle.Flat;
			btnLogin.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
			btnLogin.ForeColor = Color.White;
			btnLogin.Location = new Point(35, 150);
			btnLogin.Name = "btnLogin";
			btnLogin.Size = new Size(110, 35);
			btnLogin.TabIndex = 2;
			btnLogin.Text = "Login";
			btnLogin.UseVisualStyleBackColor = false;
			// 
			// btnRegister
			// 
			btnRegister.BackColor = Color.ForestGreen;
			btnRegister.Cursor = Cursors.Hand;
			btnRegister.FlatAppearance.BorderSize = 0;
			btnRegister.FlatStyle = FlatStyle.Flat;
			btnRegister.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
			btnRegister.ForeColor = Color.White;
			btnRegister.Location = new Point(155, 150);
			btnRegister.Name = "btnRegister";
			btnRegister.Size = new Size(110, 35);
			btnRegister.TabIndex = 3;
			btnRegister.Text = "Register";
			btnRegister.UseVisualStyleBackColor = false;
			// 
			// lblUser
			// 
			lblUser.AutoSize = true;
			lblUser.Font = new Font("Segoe UI", 9F);
			lblUser.ForeColor = Color.Gray;
			lblUser.Location = new Point(35, 27);
			lblUser.Name = "lblUser";
			lblUser.Size = new Size(60, 15);
			lblUser.TabIndex = 4;
			lblUser.Text = "Username";
			// 
			// lblPass
			// 
			lblPass.AutoSize = true;
			lblPass.Font = new Font("Segoe UI", 9F);
			lblPass.ForeColor = Color.Gray;
			lblPass.Location = new Point(35, 87);
			lblPass.Name = "lblPass";
			lblPass.Size = new Size(57, 15);
			lblPass.TabIndex = 5;
			lblPass.Text = "Password";
			// 
			// pnlMain
			// 
			pnlMain.BackColor = Color.White;
			pnlMain.Controls.Add(lblUser);
			pnlMain.Controls.Add(btnRegister);
			pnlMain.Controls.Add(txtUsername);
			pnlMain.Controls.Add(btnLogin);
			pnlMain.Controls.Add(lblPass);
			pnlMain.Controls.Add(txtPassword);
			pnlMain.Location = new Point(48, 41);
			pnlMain.Name = "pnlMain";
			pnlMain.Size = new Size(300, 220);
			pnlMain.TabIndex = 6;
			// 
			// LoginForm
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			BackColor = Color.WhiteSmoke;
			ClientSize = new Size(399, 311);
			Controls.Add(pnlMain);
			FormBorderStyle = FormBorderStyle.FixedSingle;
			MaximizeBox = false;
			Name = "LoginForm";
			StartPosition = FormStartPosition.CenterScreen;
			Text = "ChatApp - Login";
			pnlMain.ResumeLayout(false);
			pnlMain.PerformLayout();
			ResumeLayout(false);
		}

		#endregion

		private TextBox txtUsername;
		private TextBox txtPassword;
		private Button btnLogin;
		private Button btnRegister;
		private Label lblUser;
		private Label lblPass;
		private Panel pnlMain;
	}
}