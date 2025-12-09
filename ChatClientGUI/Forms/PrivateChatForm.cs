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
		private const int WM_NCHITTEST = 0x84;
		private const int HTCLIENT = 0x1;
		private const int HTBOTTOMRIGHT = 17;
		private const int CS_DROPSHADOW = 0x00020000;

		[DllImport("user32.dll")]
		private extern static void ReleaseCapture();
		[DllImport("user32.dll")]
		private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);

		private readonly ClientService _service;
		private readonly string _targetUser;
		private const string PLACEHOLDER = "Írj egy üzenetet...";

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ClassStyle |= CS_DROPSHADOW;
				return cp;
			}
		}

		public PrivateChatForm(ClientService service, string targetUser)
		{
			InitializeComponent();
			this.SetStyle(ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
			this.DoubleBuffered = true;

			_service = service;
			_targetUser = targetUser;

			lblTitle.Text = $"Private Chat - {_targetUser}";

			
			txtPrivateInput.Text = PLACEHOLDER;
			txtPrivateInput.ForeColor = Color.Gray;
			txtPrivateInput.BackColor = Color.FromArgb(245, 245, 245);
			txtPrivateInput.BorderStyle = BorderStyle.None;

			UpdateControlRegions();

			_service.MessageReceived += OnMessageReceived;
			btnSendPrivate.Click += async (s, e) => await SendPrivate();

			btnCloseApp.Click += (s, e) => this.Close();
			btnMinimize.Click += (s, e) => this.WindowState = FormWindowState.Minimized;

			pnlHeader.MouseDown += (s, e) => { ReleaseCapture(); SendMessage(this.Handle, 0x112, 0xf012, 0); };
			lblTitle.MouseDown += (s, e) => { ReleaseCapture(); SendMessage(this.Handle, 0x112, 0xf012, 0); };

			pnlBottom.Paint += PnlBottom_Paint;

			this.SizeChanged += (s, e) => { this.Invalidate(); UpdateControlRegions(); pnlBottom.Invalidate(); };
			txtPrivateInput.SizeChanged += (s, e) => { if (txtPrivateInput.Width > 0) txtPrivateInput.Region = new Region(new Rectangle(2, 2, txtPrivateInput.Width - 4, txtPrivateInput.Height - 4)); };

			// Placeholder Események
			txtPrivateInput.Enter += (s, e) => { if (txtPrivateInput.Text == PLACEHOLDER) { txtPrivateInput.Text = ""; txtPrivateInput.ForeColor = Color.Black; } };
			txtPrivateInput.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(txtPrivateInput.Text)) { txtPrivateInput.Text = PLACEHOLDER; txtPrivateInput.ForeColor = Color.Gray; } };

			txtPrivateInput.KeyDown += async (s, e) =>
			{
				if (e.KeyCode == Keys.Enter && !e.Shift)
				{
					e.SuppressKeyPress = true;
					if (txtPrivateInput.Text != PLACEHOLDER) await SendPrivate();
				}
			};

			lstPrivateMessages.MeasureItem += LstPrivateMessages_MeasureItem;
			lstPrivateMessages.DrawItem += LstPrivateMessages_DrawItem;

			this.FormClosing += (s, e) => _service.MessageReceived -= OnMessageReceived;
		}

		private void LstPrivateMessages_MeasureItem(object sender, MeasureItemEventArgs e)
		{
			if (e.Index < 0 || e.Index >= lstPrivateMessages.Items.Count) return;
			string fullMsg = lstPrivateMessages.Items[e.Index].ToString();

			
			string content = fullMsg;
			if (fullMsg.StartsWith("Me:")) content = fullMsg.Substring(3).Trim();

			int maxWidth = (int)(lstPrivateMessages.Width * 0.7);
			Size size = TextRenderer.MeasureText(e.Graphics, content, new Font("Segoe UI", 10), new Size(maxWidth, 0), TextFormatFlags.WordBreak);
			e.ItemHeight = size.Height + 35; // + padding
		}

		private void LstPrivateMessages_DrawItem(object sender, DrawItemEventArgs e)
		{
			if (e.Index < 0) return;
			e.Graphics.FillRectangle(Brushes.White, e.Bounds);

			string msg = lstPrivateMessages.Items[e.Index].ToString();
			Graphics g = e.Graphics;
			g.SmoothingMode = SmoothingMode.AntiAlias;
			g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

			bool isMe = msg.StartsWith("Me:");
			string content = isMe ? msg.Substring(3).Trim() : msg;

			
			string senderName = _targetUser;
			if (!isMe && msg.Contains(":"))
			{
				var parts = msg.Split(new[] { ':' }, 2);
				content = parts[1].Trim();
			}

			Color bubbleColor = isMe ? Color.FromArgb(0, 132, 255) : Color.FromArgb(240, 240, 240);
			Color textColor = isMe ? Color.White : Color.Black;

			int maxWidth = (int)(lstPrivateMessages.Width * 0.7);
			Size size = TextRenderer.MeasureText(g, content, new Font("Segoe UI", 10), new Size(maxWidth, 0), TextFormatFlags.WordBreak);

			int bubbleWidth = size.Width + 24;
			int bubbleHeight = size.Height + 15;

			Rectangle bubbleRect;
			if (isMe)
				bubbleRect = new Rectangle(e.Bounds.Right - bubbleWidth - 15, e.Bounds.Top + 5, bubbleWidth, bubbleHeight);
			else
				bubbleRect = new Rectangle(e.Bounds.Left + 15, e.Bounds.Top + 5, bubbleWidth, bubbleHeight);

			using (GraphicsPath path = GetRoundedPath(bubbleRect, 12))
			using (var brush = new SolidBrush(bubbleColor))
			{
				g.FillPath(brush, path);
			}

			
			TextRenderer.DrawText(g, content, new Font("Segoe UI", 10), bubbleRect, textColor, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter | TextFormatFlags.WordBreak);
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
			if (string.IsNullOrWhiteSpace(txtPrivateInput.Text) || txtPrivateInput.Text == PLACEHOLDER) return;
			string msg = txtPrivateInput.Text;
			await _service.SendPrivateMessageAsync(_targetUser, msg);
			lstPrivateMessages.Items.Add($"Me: {msg}");
			txtPrivateInput.Clear();
			txtPrivateInput.Focus();
			lstPrivateMessages.TopIndex = lstPrivateMessages.Items.Count - 1;
		}

		
		private void PnlBottom_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
			using (var pen = new Pen(Color.FromArgb(230, 230, 230))) { e.Graphics.DrawLine(pen, 0, 0, pnlBottom.Width, 0); }
			Rectangle rect = new Rectangle(txtPrivateInput.Location.X - 10, txtPrivateInput.Location.Y - 5, txtPrivateInput.Width + 20, txtPrivateInput.Height + 10);
			using (GraphicsPath path = GetRoundedPath(rect, 18)) using (var brush = new SolidBrush(Color.FromArgb(245, 245, 245))) { e.Graphics.FillPath(brush, path); }
		}

		private void UpdateControlRegions()
		{
			using (GraphicsPath path = new GraphicsPath()) { path.AddEllipse(0, 0, btnSendPrivate.Width, btnSendPrivate.Height); btnSendPrivate.Region = new Region(path); }
			if (txtPrivateInput.Width > 0) txtPrivateInput.Region = new Region(new Rectangle(2, 2, txtPrivateInput.Width - 4, txtPrivateInput.Height - 4));
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			using (Pen pen = new Pen(Color.LightGray, 1)) { e.Graphics.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1); }
		}

		protected override void WndProc(ref Message m)
		{
			base.WndProc(ref m);
			if (m.Msg == WM_NCHITTEST && (int)m.Result == HTCLIENT)
			{
				Point cursor = this.PointToClient(Cursor.Position);
				if (cursor.X >= this.ClientSize.Width - 10 && cursor.Y >= this.ClientSize.Height - 10) m.Result = (IntPtr)HTBOTTOMRIGHT;
			}
		}

		private void OnMessageReceived(string msg) { if (IsDisposed || !IsHandleCreated) return; Invoke((MethodInvoker)delegate { if (msg.StartsWith($"(privát) {_targetUser}:") || msg.StartsWith($"(private) {_targetUser}:")) { lstPrivateMessages.Items.Add(msg); lstPrivateMessages.TopIndex = lstPrivateMessages.Items.Count - 1; } }); }
	}
}