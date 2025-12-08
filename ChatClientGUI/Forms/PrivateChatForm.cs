using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChatClientGUI.Services;

namespace ChatClientGUI.Forms
{
	public partial class PrivateChatForm : Form
	{
		// --- WINAPI ---
		private const int WM_NCHITTEST = 0x84;
		private const int HTCLIENT = 0x1;
		private const int HTLEFT = 10;
		private const int HTRIGHT = 11;
		private const int HTTOP = 12;
		private const int HTTOPLEFT = 13;
		private const int HTTOPRIGHT = 14;
		private const int HTBOTTOM = 15;
		private const int HTBOTTOMLEFT = 16;
		private const int HTBOTTOMRIGHT = 17;

		[DllImport("user32.dll", EntryPoint = "ReleaseCapture")]
		private extern static void ReleaseCapture();
		[DllImport("user32.dll", EntryPoint = "SendMessage")]
		private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);

		private readonly ClientService _service;
		private readonly string _targetUser;

		public PrivateChatForm(ClientService service, string targetUser)
		{
			InitializeComponent();
			this.SetStyle(ControlStyles.ResizeRedraw, true);
			this.DoubleBuffered = true;

			_service = service;
			_targetUser = targetUser;

			lblTitle.Text = $"Private Chat - {_targetUser}";

			UpdateControlRegions();

			// INPUT STÍLUS
			txtPrivateInput.BackColor = Color.FromArgb(240, 242, 245);
			txtPrivateInput.BorderStyle = BorderStyle.None;

			_service.MessageReceived += OnMessageReceived;
			btnSendPrivate.Click += async (s, e) => await SendPrivate();

			btnCloseApp.Click += (s, e) => this.Close();
			btnMinimize.Click += (s, e) => this.WindowState = FormWindowState.Minimized;
			pnlHeader.MouseDown += PnlHeader_MouseDown;
			lblTitle.MouseDown += PnlHeader_MouseDown;

			pnlBottom.Paint += PnlBottom_Paint;

			// ÁTMÉRETEZÉS FRISSÍTÉS
			this.SizeChanged += (s, e) =>
			{
				this.Invalidate();
				UpdateControlRegions();
				pnlBottom.Invalidate();
			};

			txtPrivateInput.SizeChanged += (s, e) =>
			{
				if (txtPrivateInput.Width > 4 && txtPrivateInput.Height > 4)
					txtPrivateInput.Region = new Region(new Rectangle(2, 2, txtPrivateInput.Width - 4, txtPrivateInput.Height - 4));
			};

			txtPrivateInput.KeyDown += async (s, e) =>
			{
				if (e.KeyCode == Keys.Enter && !e.Shift)
				{
					e.SuppressKeyPress = true;
					await SendPrivate();
				}
			};

			lstPrivateMessages.MeasureItem += LstPrivateMessages_MeasureItem;
			lstPrivateMessages.DrawItem += LstPrivateMessages_DrawItem;

			this.FormClosing += (s, e) => _service.MessageReceived -= OnMessageReceived;
		}

		// --- KERET ÉS ÁTMÉRETEZÉS ---
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			using (Pen pen = new Pen(Color.LightGray, 1))
			{
				e.Graphics.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);
			}
		}

		protected override void WndProc(ref Message m)
		{
			base.WndProc(ref m);
			if (m.Msg == WM_NCHITTEST)
			{
				int resizeArea = 10;
				Point cursor = this.PointToClient(Cursor.Position);

				if (cursor.X >= this.ClientSize.Width - resizeArea && cursor.Y >= this.ClientSize.Height - resizeArea) m.Result = (IntPtr)HTBOTTOMRIGHT;
				else if (cursor.X <= resizeArea && cursor.Y >= this.ClientSize.Height - resizeArea) m.Result = (IntPtr)HTBOTTOMLEFT;
				else if (cursor.X >= this.ClientSize.Width - resizeArea && cursor.Y <= resizeArea) m.Result = (IntPtr)HTTOPRIGHT;
				else if (cursor.X <= resizeArea && cursor.Y <= resizeArea) m.Result = (IntPtr)HTTOPLEFT;
				else if (cursor.X <= resizeArea) m.Result = (IntPtr)HTLEFT;
				else if (cursor.X >= this.ClientSize.Width - resizeArea) m.Result = (IntPtr)HTRIGHT;
				else if (cursor.Y <= resizeArea) m.Result = (IntPtr)HTTOP;
				else if (cursor.Y >= this.ClientSize.Height - resizeArea) m.Result = (IntPtr)HTBOTTOM;
			}
		}

		private void UpdateControlRegions()
		{
			ApplyRoundedRegion(btnSendPrivate, 20);

			if (txtPrivateInput.Width > 4 && txtPrivateInput.Height > 4)
				txtPrivateInput.Region = new Region(new Rectangle(2, 2, txtPrivateInput.Width - 4, txtPrivateInput.Height - 4));
		}

		// --- LOGIKA ---

		private void PnlBottom_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			using (var pen = new Pen(Color.LightGray))
			{
				e.Graphics.DrawLine(pen, 0, 0, pnlBottom.Width, 0);
			}
			Rectangle rect = new Rectangle(txtPrivateInput.Location.X - 10, txtPrivateInput.Location.Y - 5, txtPrivateInput.Width + 20, txtPrivateInput.Height + 10);
			using (GraphicsPath path = GetRoundedPath(rect, 15))
			using (var brush = new SolidBrush(Color.FromArgb(240, 242, 245)))
			{
				e.Graphics.FillPath(brush, path);
			}
		}

		private void ApplyRoundedRegion(Control control, int radius)
		{
			Rectangle bounds = new Rectangle(0, 0, control.Width, control.Height);
			using (GraphicsPath path = GetRoundedPath(bounds, radius))
			{
				control.Region = new Region(path);
			}
		}

		private void PnlHeader_MouseDown(object sender, MouseEventArgs e)
		{
			ReleaseCapture();
			SendMessage(this.Handle, 0x112, 0xf012, 0);
		}

		private void LstPrivateMessages_MeasureItem(object sender, MeasureItemEventArgs e)
		{
			if (e.Index < 0 || e.Index >= lstPrivateMessages.Items.Count) return;
			string msg = lstPrivateMessages.Items[e.Index].ToString();
			int maxWidth = (int)(lstPrivateMessages.Width * 0.7);
			Size size = TextRenderer.MeasureText(e.Graphics, msg, lstPrivateMessages.Font, new Size(maxWidth, 0), TextFormatFlags.WordBreak);
			e.ItemHeight = size.Height + 20;
		}

		private void LstPrivateMessages_DrawItem(object sender, DrawItemEventArgs e)
		{
			if (e.Index < 0) return;
			e.DrawBackground();
			string msg = lstPrivateMessages.Items[e.Index].ToString();
			Graphics g = e.Graphics;
			g.SmoothingMode = SmoothingMode.AntiAlias;
			g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

			bool isMe = msg.StartsWith("Me:");

			Color bubbleColor = isMe ? Color.FromArgb(0, 120, 215) : Color.FromArgb(230, 230, 230);
			Color textColor = isMe ? Color.White : Color.Black;

			int maxWidth = (int)(lstPrivateMessages.Width * 0.7);
			Size size = TextRenderer.MeasureText(g, msg, e.Font, new Size(maxWidth, 0), TextFormatFlags.WordBreak);

			int bubbleWidth = size.Width + 20;
			int bubbleHeight = size.Height + 10;

			Rectangle bubbleRect;
			if (isMe)
				bubbleRect = new Rectangle(e.Bounds.Right - bubbleWidth - 10, e.Bounds.Top + 5, bubbleWidth, bubbleHeight);
			else
				bubbleRect = new Rectangle(e.Bounds.Left + 10, e.Bounds.Top + 5, bubbleWidth, bubbleHeight);

			using (GraphicsPath path = GetRoundedPath(bubbleRect, 10))
			using (var brush = new SolidBrush(bubbleColor))
			{
				g.FillPath(brush, path);
			}

			TextRenderer.DrawText(g, msg, e.Font, bubbleRect, textColor, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter | TextFormatFlags.WordBreak);
			e.DrawFocusRectangle();
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

		private async Task SendPrivate()
		{
			if (string.IsNullOrWhiteSpace(txtPrivateInput.Text)) return;

			string msg = txtPrivateInput.Text;
			await _service.SendPrivateMessageAsync(_targetUser, msg);

			lstPrivateMessages.Items.Add($"Me: {msg}");
			txtPrivateInput.Clear();
			lstPrivateMessages.TopIndex = lstPrivateMessages.Items.Count - 1;
		}

		private void OnMessageReceived(string msg)
		{
			if (IsDisposed || !IsHandleCreated) return;
			Invoke((MethodInvoker)delegate
			{
				if (msg.StartsWith($"(privát) {_targetUser}:") || msg.StartsWith($"(private) {_targetUser}:"))
				{
					lstPrivateMessages.Items.Add(msg);
					lstPrivateMessages.TopIndex = lstPrivateMessages.Items.Count - 1;
				}
			});
		}
	}
}