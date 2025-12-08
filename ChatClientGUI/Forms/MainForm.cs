using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChatClientGUI.Services;

namespace ChatClientGUI.Forms
{
	public partial class MainForm : Form
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
		private readonly string _myUsername;
		private string _currentChatPartner = null;
		private readonly List<string> _allMessages = new List<string>();
		private readonly Dictionary<string, (string FileName, byte[] Content)> _pendingFiles = new Dictionary<string, (string, byte[])>();

		public MainForm(ClientService service, string myName)
		{
			InitializeComponent();
			this.SetStyle(ControlStyles.ResizeRedraw, true);
			this.DoubleBuffered = true;

			_service = service;
			_myUsername = string.IsNullOrEmpty(myName) ? "Me" : myName;

			lblTitle.Text = $"ChatApp - {_myUsername}";

			// KEZDETI FORMÁZÁS (Kör alakú gombok)
			UpdateControlRegions();

			// INPUT MEZŐ ALAP STÍLUS
			txtMessage.BackColor = Color.FromArgb(245, 245, 245);
			txtMessage.BorderStyle = BorderStyle.None;

			_service.MessageReceived += OnMessageReceived;
			_service.ConnectionLost += OnConnectionLost;
			_service.FileReceived += OnFileReceived;
			_service.UserListReceived += OnUserListReceived;

			btnSend.Click += async (s, e) => await SendMessage();
			btnFile.Click += async (s, e) => await SendFile();

			// Felső gombok
			btnCloseApp.Click += (s, e) => Application.Exit();
			btnMinimize.Click += (s, e) => this.WindowState = FormWindowState.Minimized;

			pnlHeader.MouseDown += PnlHeader_MouseDown;
			lblTitle.MouseDown += PnlHeader_MouseDown;

			pnlBottom.Paint += PnlBottom_Paint;

			this.SizeChanged += (s, e) =>
			{
				this.Invalidate();
				UpdateControlRegions();
				pnlBottom.Invalidate();
			};

			txtMessage.SizeChanged += (s, e) =>
			{
				if (txtMessage.Width > 4 && txtMessage.Height > 4)
					txtMessage.Region = new Region(new Rectangle(2, 2, txtMessage.Width - 4, txtMessage.Height - 4));
			};

			txtMessage.KeyDown += async (s, e) =>
			{
				if (e.KeyCode == Keys.Enter && !e.Shift)
				{
					e.SuppressKeyPress = true;
					await SendMessage();
				}
			};

			lstMessages.MeasureItem += LstMessages_MeasureItem;
			lstMessages.DrawItem += LstMessages_DrawItem;
			lstUsers.DrawItem += LstUsers_DrawItem;

			lstMessages.DoubleClick += (s, e) =>
			{
				if (lstMessages.SelectedItem != null)
					HandleFileDownload(lstMessages.SelectedItem.ToString());
			};

			lstUsers.SelectedIndexChanged += (s, e) =>
			{
				if (lstUsers.SelectedItem != null)
					HandleUserSelection(lstUsers.SelectedItem.ToString());
			};

			lstUsers.Items.Add("[Global Chat]");
			lstUsers.SelectedIndex = 0;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			using (Pen pen = new Pen(Color.Gainsboro, 1))
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
			// KÖR ALAKÚ GOMBOK
			ApplyCircleRegion(btnSend);
			ApplyCircleRegion(btnFile);

			if (txtMessage.Width > 4 && txtMessage.Height > 4)
				txtMessage.Region = new Region(new Rectangle(2, 2, txtMessage.Width - 4, txtMessage.Height - 4));
		}

		private void PnlBottom_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

			// Textbox háttér ("Pill" alak) - letisztult szürke
			Rectangle rect = new Rectangle(txtMessage.Location.X - 15, txtMessage.Location.Y - 8, txtMessage.Width + 30, txtMessage.Height + 16);
			using (GraphicsPath path = GetRoundedPath(rect, 18))
			using (var brush = new SolidBrush(Color.FromArgb(245, 245, 245)))
			{
				e.Graphics.FillPath(brush, path);
			}
		}

		private void ApplyCircleRegion(Control control)
		{
			using (GraphicsPath path = new GraphicsPath())
			{
				path.AddEllipse(0, 0, control.Width, control.Height);
				control.Region = new Region(path);
			}
		}

		private void PnlHeader_MouseDown(object sender, MouseEventArgs e)
		{
			ReleaseCapture();
			SendMessage(this.Handle, 0x112, 0xf012, 0);
		}

		// --- RAJZOLÓ LOGIKA ---
		private void LstUsers_DrawItem(object sender, DrawItemEventArgs e)
		{
			if (e.Index < 0) return;
			e.DrawBackground();
			if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
				e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(235, 245, 255)), e.Bounds);
			else
				e.Graphics.FillRectangle(new SolidBrush(Color.White), e.Bounds);

			string userName = lstUsers.Items[e.Index].ToString();
			Graphics g = e.Graphics;
			g.SmoothingMode = SmoothingMode.AntiAlias;
			g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

			int dotSize = 8;
			int dotX = e.Bounds.Left + 15;
			int dotY = e.Bounds.Top + (e.Bounds.Height - dotSize) / 2;
			Color dotColor = (userName == "[Global Chat]") ? Color.DodgerBlue : Color.LimeGreen;

			using (var brush = new SolidBrush(dotColor))
				g.FillEllipse(brush, dotX, dotY, dotSize, dotSize);

			Color textColor = Color.FromArgb(64, 64, 64);
			System.Drawing.Font font = (userName == "[Global Chat]") ? new System.Drawing.Font("Segoe UI", 9F, FontStyle.Bold) : new System.Drawing.Font("Segoe UI", 9F);

			int textX = dotX + dotSize + 15;
			Rectangle textRect = new Rectangle(textX, e.Bounds.Top, e.Bounds.Width - textX, e.Bounds.Height);
			TextRenderer.DrawText(g, userName, font, textRect, textColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
		}

		private void LstMessages_MeasureItem(object sender, MeasureItemEventArgs e)
		{
			if (e.Index < 0 || e.Index >= lstMessages.Items.Count) return;
			string msg = lstMessages.Items[e.Index].ToString();
			int maxWidth = (int)(lstMessages.Width * 0.7);
			Size size = TextRenderer.MeasureText(e.Graphics, msg, lstMessages.Font, new Size(maxWidth, 0), TextFormatFlags.WordBreak);
			e.ItemHeight = size.Height + 24;
		}

		private void LstMessages_DrawItem(object sender, DrawItemEventArgs e)
		{
			if (e.Index < 0) return;
			e.DrawBackground();
			string msg = lstMessages.Items[e.Index].ToString();
			Graphics g = e.Graphics;
			g.SmoothingMode = SmoothingMode.AntiAlias;
			g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

			bool isMe = IsMyMessage(msg);
			bool isSystem = msg.Contains("FILE") || msg.Contains("System") || msg.Contains("Server");

			if (isSystem)
			{
				TextRenderer.DrawText(g, msg, new System.Drawing.Font(e.Font, FontStyle.Italic), e.Bounds, Color.Gray, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak);
			}
			else
			{
				Color bubbleColor = isMe ? Color.FromArgb(0, 132, 255) : Color.FromArgb(240, 240, 240);
				Color textColor = isMe ? Color.White : Color.Black;

				int maxWidth = (int)(lstMessages.Width * 0.7);
				Size size = TextRenderer.MeasureText(g, msg, e.Font, new Size(maxWidth, 0), TextFormatFlags.WordBreak);

				int bubbleWidth = size.Width + 24;
				int bubbleHeight = size.Height + 12;

				Rectangle bubbleRect;
				if (isMe)
					bubbleRect = new Rectangle(e.Bounds.Right - bubbleWidth - 15, e.Bounds.Top + 6, bubbleWidth, bubbleHeight);
				else
					bubbleRect = new Rectangle(e.Bounds.Left + 15, e.Bounds.Top + 6, bubbleWidth, bubbleHeight);

				using (GraphicsPath path = GetRoundedPath(bubbleRect, 12))
				using (var brush = new SolidBrush(bubbleColor))
				{
					g.FillPath(brush, path);
				}
				TextRenderer.DrawText(g, msg, e.Font, bubbleRect, textColor, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter | TextFormatFlags.WordBreak);
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

		private bool IsMyMessage(string msg) => msg.Contains($" {_myUsername}:") || msg.Contains($"[Private ->");

		// --- SEGÉDFÜGGVÉNYEK ---
		private void HandleUserSelection(string selection)
		{
			if (selection == "[Global Chat]") { _currentChatPartner = null; lblTitle.Text = $"ChatApp - {_myUsername} (Global)"; }
			else { _currentChatPartner = selection; lblTitle.Text = $"ChatApp - {_myUsername} -> {_currentChatPartner}"; }
			RefreshChatView();
		}

		private void OnUserListReceived(string[] users)
		{
			if (IsDisposed || !IsHandleCreated) return;
			Invoke((MethodInvoker)delegate {
				var currentSelection = lstUsers.SelectedItem;
				lstUsers.Items.Clear();
				lstUsers.Items.Add("[Global Chat]");
				foreach (var user in users) if (user != _myUsername) lstUsers.Items.Add(user);
				if (currentSelection != null && lstUsers.Items.Contains(currentSelection)) lstUsers.SelectedItem = currentSelection;
			});
		}

		private void RefreshChatView()
		{
			lstMessages.Items.Clear();
			foreach (var msg in _allMessages)
			{
				if (_currentChatPartner == null) { if (!msg.Contains("(privát)") && !msg.Contains("[Private ->") && !msg.Contains("(private)")) lstMessages.Items.Add(msg); }
				else { bool fromPartner = msg.Contains($"(privát) {_currentChatPartner}:") || msg.Contains($"(private) {_currentChatPartner}:"); bool toPartner = msg.Contains($"[Private -> {_currentChatPartner}]"); if (fromPartner || toPartner) lstMessages.Items.Add(msg); }
			}
			if (lstMessages.Items.Count > 0) lstMessages.TopIndex = lstMessages.Items.Count - 1;
		}

		private async Task SendMessage()
		{
			if (string.IsNullOrWhiteSpace(txtMessage.Text)) return;
			string text = txtMessage.Text;
			if (_currentChatPartner != null) { await _service.SendPrivateMessageAsync(_currentChatPartner, text); _allMessages.Add($"[Private -> {_currentChatPartner}]: {text}"); }
			else { await _service.SendMessageAsync(text); }
			txtMessage.Clear(); RefreshChatView();
		}

		private async Task SendFile()
		{
			using var dialog = new OpenFileDialog();
			if (dialog.ShowDialog() == DialogResult.OK)
			{
				string filePath = dialog.FileName; string fileName = Path.GetFileName(filePath); byte[] fileBytes = await File.ReadAllBytesAsync(filePath); string recipient = _currentChatPartner ?? "";
				await _service.SendFileAsync(filePath, recipient);
				string displayMsg = !string.IsNullOrEmpty(recipient) ? $"FILE [Private -> {recipient}]: {fileName} ({fileBytes.Length} byte) >>> CLICK TO SAVE <<<" : $"FILE [Global]: {fileName} ({fileBytes.Length} byte) >>> CLICK TO SAVE <<<";
				if (_pendingFiles.ContainsKey(displayMsg)) displayMsg += $" [{DateTime.Now.Ticks}]"; _pendingFiles[displayMsg] = (fileName, fileBytes); _allMessages.Add(displayMsg); RefreshChatView();
			}
		}

		private void HandleFileDownload(string selectedText)
		{
			if (_pendingFiles.ContainsKey(selectedText))
			{
				var fileData = _pendingFiles[selectedText]; var result = MessageBox.Show($"Save file?\n\nName: {fileData.FileName}\nSize: {fileData.Content.Length} bytes", "Download", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
				if (result == DialogResult.Yes) { SaveFileToDisk(fileData.FileName, fileData.Content); string newText = selectedText.Replace(">>> CLICK TO SAVE <<<", "[SAVED]"); int index = _allMessages.IndexOf(selectedText); if (index != -1) _allMessages[index] = newText; _pendingFiles.Remove(selectedText); _pendingFiles[newText] = fileData; RefreshChatView(); }
			}
		}

		private void OnMessageReceived(string msg) { if (IsDisposed || !IsHandleCreated) return; Invoke((MethodInvoker)delegate { _allMessages.Add(msg); RefreshChatView(); }); }
		private void OnFileReceived(string fileName, byte[] content) { if (IsDisposed || !IsHandleCreated) return; Invoke((MethodInvoker)delegate { string displayMsg = $"INCOMING FILE: {fileName} ({content.Length} byte) >>> CLICK TO SAVE <<<"; if (_pendingFiles.ContainsKey(displayMsg)) displayMsg += $" [{DateTime.Now.Ticks}]"; _pendingFiles[displayMsg] = (fileName, content); _allMessages.Add(displayMsg); RefreshChatView(); }); }
		private void SaveFileToDisk(string fileName, byte[] content) { try { string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads"); Directory.CreateDirectory(folder); string fullPath = Path.Combine(folder, fileName); File.WriteAllBytes(fullPath, content); MessageBox.Show($"File saved: {fullPath}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information); } catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); } }
		private void OnConnectionLost() { if (IsDisposed || !IsHandleCreated) return; Invoke((MethodInvoker)delegate { MessageBox.Show("Disconnected from server.", "Connection Lost", MessageBoxButtons.OK, MessageBoxIcon.Warning); Application.Exit(); }); }
	}
}