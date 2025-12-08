using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChatClientGUI.Services;

namespace ChatClientGUI.Forms
{
	public partial class LoginForm : Form
	{
		private const int CS_DROPSHADOW = 0x00020000;

		[DllImport("user32.dll", EntryPoint = "ReleaseCapture")]
		private extern static void ReleaseCapture();
		[DllImport("user32.dll", EntryPoint = "SendMessage")]
		private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);

		private readonly ClientService _service;

		// ÁRNYÉK
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ClassStyle |= CS_DROPSHADOW;
				return cp;
			}
		}

		public LoginForm()
		{
			InitializeComponent();
			_service = new ClientService();
			_service.MessageReceived += OnMessageReceived;

			this.FormBorderStyle = FormBorderStyle.None;
			this.DoubleBuffered = true;
			this.SetStyle(ControlStyles.ResizeRedraw | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);

			// Gombok kerekítése
			ApplyRoundedRegion(btnLogin, 20);
			ApplyRoundedRegion(btnRegister, 20);

			// HOVER EFFEKTEK
			btnLogin.MouseEnter += (s, e) => btnLogin.BackColor = Color.FromArgb(0, 90, 180);
			btnLogin.MouseLeave += (s, e) => btnLogin.BackColor = Color.DodgerBlue;

			btnRegister.MouseEnter += (s, e) => btnRegister.BackColor = Color.DimGray;
			btnRegister.MouseLeave += (s, e) => btnRegister.BackColor = Color.Gray;

			btnClose.Click += (s, e) => Application.Exit();
			btnClose.MouseEnter += (s, e) => { btnClose.BackColor = Color.IndianRed; btnClose.ForeColor = Color.White; };
			btnClose.MouseLeave += (s, e) => { btnClose.BackColor = Color.Transparent; btnClose.ForeColor = Color.Black; };

			btnMinimize.Click += (s, e) => this.WindowState = FormWindowState.Minimized;
			btnMinimize.MouseEnter += (s, e) => btnMinimize.BackColor = Color.LightGray;
			btnMinimize.MouseLeave += (s, e) => btnMinimize.BackColor = Color.Transparent;

			// Input stílus
			txtUsername.BorderStyle = BorderStyle.None;
			txtUsername.BackColor = Color.FromArgb(245, 245, 245);
			txtPassword.BorderStyle = BorderStyle.None;
			txtPassword.BackColor = Color.FromArgb(245, 245, 245);

			btnLogin.Click += async (s, e) => await HandleLogin();
			btnRegister.Click += async (s, e) => await HandleRegister();

			pnlHeader.MouseDown += Header_MouseDown;
			lblTitle.MouseDown += Header_MouseDown;

			this.Paint += LoginForm_Paint;

			txtPassword.KeyDown += async (s, e) =>
			{
				if (e.KeyCode == Keys.Enter)
				{
					e.SuppressKeyPress = true;
					await HandleLogin();
				}
			};
		}

		private void LoginForm_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
			e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

			using (var borderPen = new Pen(Color.LightGray))
			{
				e.Graphics.DrawRectangle(borderPen, 0, 0, this.Width - 1, this.Height - 1);
			}

			Rectangle rectUser = new Rectangle(txtUsername.Location.X - 10, txtUsername.Location.Y - 5, txtUsername.Width + 20, txtUsername.Height + 10);
			using (GraphicsPath path = GetRoundedPath(rectUser, 15))
			using (var brush = new SolidBrush(Color.FromArgb(245, 245, 245)))
			{
				e.Graphics.FillPath(brush, path);
			}

			Rectangle rectPass = new Rectangle(txtPassword.Location.X - 10, txtPassword.Location.Y - 5, txtPassword.Width + 20, txtPassword.Height + 10);
			using (GraphicsPath path = GetRoundedPath(rectPass, 15))
			using (var brush = new SolidBrush(Color.FromArgb(245, 245, 245)))
			{
				e.Graphics.FillPath(brush, path);
			}
		}

		private void Header_MouseDown(object sender, MouseEventArgs e)
		{
			ReleaseCapture();
			SendMessage(this.Handle, 0x112, 0xf012, 0);
		}

		private void ApplyRoundedRegion(Control control, int radius)
		{
			Rectangle bounds = new Rectangle(0, 0, control.Width, control.Height);
			using (GraphicsPath path = GetRoundedPath(bounds, radius))
			{
				control.Region = new Region(path);
			}
		}

		private GraphicsPath GetRoundedPath(Rectangle rect, int radius)
		{
			GraphicsPath path = new GraphicsPath();
			int d = radius * 2;
			path.AddArc(rect.X, rect.Y, d, d, 180, 90);
			path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
			path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
			path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
			path.CloseFigure();
			return path;
		}

		// --- LOGIKA ---
		private async Task<bool> ConnectIfNeeded() { return await _service.ConnectAsync("127.0.0.1", 5000); }
		private async Task HandleLogin() { if (!await ConnectIfNeeded()) { MessageBox.Show("Could not connect to server.", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; } string user = txtUsername.Text.Trim(); string pass = txtPassword.Text.Trim(); if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass)) await _service.SendLoginAnonAsync(); else await _service.SendLoginAsync(user, pass); }
		private async Task HandleRegister() { if (!await ConnectIfNeeded()) { MessageBox.Show("Could not connect to server.", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; } string user = txtUsername.Text.Trim(); string pass = txtPassword.Text.Trim(); if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(pass)) await _service.SendRegisterAsync(user, pass); else MessageBox.Show("Username and password required.", "Registration", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
		private void OnMessageReceived(string msg) { if (!IsHandleCreated) return; Invoke((MethodInvoker)delegate { if (msg.Contains("Login successful")) { string username = msg.Contains("(Anon)") ? "Anon" : txtUsername.Text; if (string.IsNullOrEmpty(username)) username = "Anon"; var mainForm = new MainForm(_service, username); mainForm.Show(); this.Hide(); } else if (msg.Contains("Registration successful")) { MessageBox.Show("Registration successful! You can now login.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information); } else if (msg.Contains("failed")) { MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); } }); }
	}
}