using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChatClientGUI.Services;

namespace ChatClientGUI.Forms
{
	public partial class LoginForm : Form
	{
		// --- ABLAK MOZGATÁS (WinAPI) ---
		[DllImport("user32.dll", EntryPoint = "ReleaseCapture")]
		private extern static void ReleaseCapture();
		[DllImport("user32.dll", EntryPoint = "SendMessage")]
		private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);

		private readonly ClientService _service;

		public LoginForm()
		{
			InitializeComponent();
			_service = new ClientService();
			_service.MessageReceived += OnMessageReceived;

			// Ablak stílus
			this.FormBorderStyle = FormBorderStyle.None;
			this.DoubleBuffered = true;

			// Gombok kerekítése
			ApplyRoundedRegion(btnLogin, 20);
			ApplyRoundedRegion(btnRegister, 20);

			// Input mezők stílusa (keret eltüntetése, hogy mi rajzolhassuk)
			txtUsername.BorderStyle = BorderStyle.None;
			txtUsername.BackColor = Color.FromArgb(240, 242, 245);
			txtPassword.BorderStyle = BorderStyle.None;
			txtPassword.BackColor = Color.FromArgb(240, 242, 245);

			// Események
			btnLogin.Click += async (s, e) => await HandleLogin();
			btnRegister.Click += async (s, e) => await HandleRegister();
			btnClose.Click += (s, e) => Application.Exit();
			btnMinimize.Click += (s, e) => this.WindowState = FormWindowState.Minimized;

			// Mozgatás
			pnlHeader.MouseDown += Header_MouseDown;
			lblTitle.MouseDown += Header_MouseDown;

			// Input mezők hátterének rajzolása
			this.Paint += LoginForm_Paint;

			// Enter a jelszónál
			txtPassword.KeyDown += async (s, e) =>
			{
				if (e.KeyCode == Keys.Enter)
				{
					e.SuppressKeyPress = true;
					await HandleLogin();
				}
			};
		}

		// --- RAJZOLÁS ---
		private void LoginForm_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

			// Vékony keret az ablak köré
			using (var borderPen = new Pen(Color.LightGray))
			{
				e.Graphics.DrawRectangle(borderPen, 0, 0, this.Width - 1, this.Height - 1);
			}

			// Username mező háttere
			Rectangle rectUser = new Rectangle(txtUsername.Location.X - 10, txtUsername.Location.Y - 5, txtUsername.Width + 20, txtUsername.Height + 10);
			using (GraphicsPath path = GetRoundedPath(rectUser, 15))
			using (var brush = new SolidBrush(Color.FromArgb(240, 242, 245)))
			{
				e.Graphics.FillPath(brush, path);
			}

			// Password mező háttere
			Rectangle rectPass = new Rectangle(txtPassword.Location.X - 10, txtPassword.Location.Y - 5, txtPassword.Width + 20, txtPassword.Height + 10);
			using (GraphicsPath path = GetRoundedPath(rectPass, 15))
			using (var brush = new SolidBrush(Color.FromArgb(240, 242, 245)))
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

		// --- LOGIKA (Változatlan) ---
		private async Task<bool> ConnectIfNeeded()
		{
			return await _service.ConnectAsync("127.0.0.1", 5000);
		}

		private async Task HandleLogin()
		{
			if (!await ConnectIfNeeded())
			{
				MessageBox.Show("Could not connect to server.", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			string user = txtUsername.Text.Trim();
			string pass = txtPassword.Text.Trim();

			if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
			{
				await _service.SendLoginAnonAsync();
			}
			else
			{
				await _service.SendLoginAsync(user, pass);
			}
		}

		private async Task HandleRegister()
		{
			if (!await ConnectIfNeeded())
			{
				MessageBox.Show("Could not connect to server.", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			string user = txtUsername.Text.Trim();
			string pass = txtPassword.Text.Trim();

			if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(pass))
			{
				await _service.SendRegisterAsync(user, pass);
			}
			else
			{
				MessageBox.Show("Username and password required.", "Registration", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		private void OnMessageReceived(string msg)
		{
			if (!IsHandleCreated) return;

			Invoke((MethodInvoker)delegate
			{
				if (msg.Contains("Login successful"))
				{
					string username = msg.Contains("(Anon)") ? "Anon" : txtUsername.Text;
					if (string.IsNullOrEmpty(username)) username = "Anon";

					var mainForm = new MainForm(_service, username);
					mainForm.Show();
					this.Hide();
				}
				else if (msg.Contains("Registration successful"))
				{
					MessageBox.Show("Registration successful! You can now login.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
				else if (msg.Contains("failed"))
				{
					MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			});
		}
	}
}