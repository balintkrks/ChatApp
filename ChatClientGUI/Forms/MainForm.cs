using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
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
		private const int HTBOTTOMRIGHT = 17;
		private const int CS_DROPSHADOW = 0x00020000;

		[DllImport("user32.dll")]
		private extern static void ReleaseCapture();
		[DllImport("user32.dll")]
		private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);

		private readonly ClientService _service;
		private readonly string _myUsername;
		private string _currentChatPartner = null;
		private readonly List<string> _allMessages = new List<string>();
		private readonly Dictionary<string, (string FileName, byte[] Content)> _pendingFiles = new Dictionary<string, (string, byte[])>();

		// Placeholder szöveg
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

		public MainForm(ClientService service, string myName)
		{
			InitializeComponent();
			this.SetStyle(ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
			this.DoubleBuffered = true;

			_service = service;
			_myUsername = string.IsNullOrEmpty(myName) ? "Me" : myName;

			lblTitle.Text = $"ChatApp - {_myUsername}";

			// Placeholder beállítás
			txtMessage.Text = PLACEHOLDER;
			txtMessage.ForeColor = Color.Gray;
			txtMessage.BackColor = Color.FromArgb(245, 245, 245);
			txtMessage.BorderStyle = BorderStyle.None;

			UpdateControlRegions();

			_service.MessageReceived += OnMessageReceived;
			_service.ConnectionLost += OnConnectionLost;
			_service.FileReceived += OnFileReceived;
			_service.UserListReceived += OnUserListReceived;

			btnSend.Click += async (s, e) => await SendMessage();
			btnFile.Click += async (s, e) => await SendFile();
			btnCloseApp.Click += (s, e) => Application.Exit();
			btnMinimize.Click += (s, e) => this.WindowState = FormWindowState.Minimized;

			// Placeholder logika
			txtMessage.Enter += (s, e) => {
				if (txtMessage.Text == PLACEHOLDER)
				{
					txtMessage.Text = "";
					txtMessage.ForeColor = Color.Black;
				}
			};
			txtMessage.Leave += (s, e) => {
				if (string.IsNullOrWhiteSpace(txtMessage.Text))
				{
					txtMessage.Text = PLACEHOLDER;
					txtMessage.ForeColor = Color.Gray;
				}
			};

			// Enter küldés
			txtMessage.KeyDown += async (s, e) =>
			{
				if (e.KeyCode == Keys.Enter && !e.Shift)
				{
					e.SuppressKeyPress = true;
					if (txtMessage.Text != PLACEHOLDER) await SendMessage();
				}
			};

			// Rajzolás
			pnlBottom.Paint += PnlBottom_Paint;
			lstMessages.MeasureItem += LstMessages_MeasureItem;
			lstMessages.DrawItem += LstMessages_DrawItem;
			lstUsers.DrawItem += LstUsers_DrawItem;

			// Fejléc mozgatás
			pnlHeader.MouseDown += (s, e) => { ReleaseCapture(); SendMessage(this.Handle, 0x112, 0xf012, 0); };
			lblTitle.MouseDown += (s, e) => { ReleaseCapture(); SendMessage(this.Handle, 0x112, 0xf012, 0); };

			// Ablak átméretezés
			this.SizeChanged += (s, e) => { this.Invalidate(); UpdateControlRegions(); pnlBottom.Invalidate(); };
			txtMessage.SizeChanged += (s, e) => { if (txtMessage.Width > 0 && txtMessage.Height > 0) txtMessage.Region = new Region(new Rectangle(2, 2, txtMessage.Width - 4, txtMessage.Height - 4)); };

			// Listák
			lstMessages.DoubleClick += (s, e) => { if (lstMessages.SelectedItem != null) HandleFileDownload(lstMessages.SelectedItem.ToString()); };
			lstUsers.SelectedIndexChanged += (s, e) => { if (lstUsers.SelectedItem != null) HandleUserSelection(lstUsers.SelectedItem.ToString()); };

			lstUsers.Items.Add("[Global Chat]");
			lstUsers.SelectedIndex = 0;
		}

		// --- OKOS BUBORÉK MÉRÉS ---
		private void LstMessages_MeasureItem(object sender, MeasureItemEventArgs e)
		{
			if (e.Index < 0 || e.Index >= lstMessages.Items.Count) return;

			string fullMsg = lstMessages.Items[e.Index].ToString();

			// Ha rendszerüzenet
			if (fullMsg.Contains("FILE") || fullMsg.Contains("System") || fullMsg.Contains("Server"))
			{
				e.ItemHeight = 40;
				return;
			}

			// Próbáljuk szétszedni: "HH:mm Név: Üzenet"
			var parts = ParseMessage(fullMsg);
			string content = parts.Content;

			int maxWidth = (int)(lstMessages.Width * 0.7);

			// Fejléc (Név + Idő) magassága kb 20px
			// Üzenet szöveg mérése
			Size size = TextRenderer.MeasureText(e.Graphics, content, new Font("Segoe UI", 10), new Size(maxWidth, 0), TextFormatFlags.WordBreak);

			e.ItemHeight = size.Height + 45; // + fejléc + margók
		}

		// --- OKOS BUBORÉK RAJZOLÁS ---
		private void LstMessages_DrawItem(object sender, DrawItemEventArgs e)
		{
			if (e.Index < 0) return;
			e.Graphics.FillRectangle(Brushes.White, e.Bounds);

			string fullMsg = lstMessages.Items[e.Index].ToString();
			Graphics g = e.Graphics;
			g.SmoothingMode = SmoothingMode.AntiAlias;
			g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

			// Rendszerüzenet
			if (fullMsg.Contains("FILE") || fullMsg.Contains("System") || fullMsg.Contains("Server"))
			{
				TextRenderer.DrawText(g, fullMsg, new System.Drawing.Font("Segoe UI", 9, FontStyle.Italic), e.Bounds, Color.Gray, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak);
				return;
			}

			// Normál üzenet bontása
			var parsed = ParseMessage(fullMsg);
			bool isMe = parsed.Name == _myUsername || parsed.Name == "Me" || fullMsg.Contains("[Private ->");

			// Buborék színek
			Color bubbleColor = isMe ? Color.FromArgb(220, 248, 198) : Color.FromArgb(240, 240, 240); // WhatsApp-szerű zöldes a sajátnak
			if (isMe) bubbleColor = Color.FromArgb(0, 132, 255); // Vagy maradjunk a kéknél
			Color textColor = isMe ? Color.White : Color.Black;
			Color timeColor = isMe ? Color.FromArgb(200, 200, 200) : Color.Gray;

			// Méretek
			int maxWidth = (int)(lstMessages.Width * 0.7);
			Size contentSize = TextRenderer.MeasureText(g, parsed.Content, new System.Drawing.Font("Segoe UI", 10), new Size(maxWidth, 0), TextFormatFlags.WordBreak);

			int bubbleWidth = Math.Max(contentSize.Width + 20, 100); // Minimum szélesség
			int bubbleHeight = contentSize.Height + 35; // Hely a névnek és időnek

			// Pozíció
			Rectangle bubbleRect;
			if (isMe)
				bubbleRect = new Rectangle(e.Bounds.Right - bubbleWidth - 15, e.Bounds.Top + 5, bubbleWidth, bubbleHeight);
			else
				bubbleRect = new Rectangle(e.Bounds.Left + 15, e.Bounds.Top + 5, bubbleWidth, bubbleHeight);

			// Buborék rajzolása
			using (GraphicsPath path = GetRoundedPath(bubbleRect, 12))
			using (var brush = new SolidBrush(bubbleColor))
			{
				g.FillPath(brush, path);
			}

			// TARTALOM RAJZOLÁSA A BUBORÉKBA
			// 1. Név (Bal felül)
			if (!isMe)
			{
				TextRenderer.DrawText(g, parsed.Name, new System.Drawing.Font("Segoe UI", 9, FontStyle.Bold),
					new Point(bubbleRect.Left + 10, bubbleRect.Top + 5),
					GetUserColor(parsed.Name));
			}

			// 2. Idő (Jobb felül)
			Size timeSize = TextRenderer.MeasureText(g, parsed.Time, new System.Drawing.Font("Segoe UI", 7));
			TextRenderer.DrawText(g, parsed.Time, new System.Drawing.Font("Segoe UI", 7),
				new Point(bubbleRect.Right - timeSize.Width - 8, bubbleRect.Top + 5),
				timeColor);

			// 3. Üzenet (Alatta)
			Rectangle textRect = new Rectangle(bubbleRect.Left + 10, bubbleRect.Top + 22, bubbleRect.Width - 20, bubbleRect.Height - 25);
			TextRenderer.DrawText(g, parsed.Content, new System.Drawing.Font("Segoe UI", 10),
				textRect, textColor, TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.WordBreak);
		}

		// --- AVATÁROS USER LISTA ---
		private void LstUsers_DrawItem(object sender, DrawItemEventArgs e)
		{
			if (e.Index < 0) return;

			// Háttér
			if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
				e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(230, 240, 255)), e.Bounds);
			else
				e.Graphics.FillRectangle(Brushes.White, e.Bounds);

			string userName = lstUsers.Items[e.Index].ToString();
			Graphics g = e.Graphics;
			g.SmoothingMode = SmoothingMode.AntiAlias;
			g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

			// AVATÁR KÖR
			int avatarSize = 24;
			int avatarX = e.Bounds.Left + 10;
			int avatarY = e.Bounds.Top + (e.Bounds.Height - avatarSize) / 2;

			// Egyedi szín a név alapján
			Color avatarColor = (userName == "[Global Chat]") ? Color.DodgerBlue : GetUserColor(userName);

			using (var brush = new SolidBrush(avatarColor))
			{
				g.FillEllipse(brush, avatarX, avatarY, avatarSize, avatarSize);
			}

			// Kezdőbetű az avatárba
			string initial = (userName.Length > 0 && userName != "[Global Chat]") ? userName.Substring(0, 1).ToUpper() : "#";
			if (userName == "[Global Chat]") initial = "G";

			TextRenderer.DrawText(g, initial, new System.Drawing.Font("Segoe UI", 9, FontStyle.Bold),
				new Rectangle(avatarX, avatarY, avatarSize, avatarSize),
				Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

			// Név
			Color textColor = Color.FromArgb(50, 50, 50);
			System.Drawing.Font font = (userName == "[Global Chat]") ? new System.Drawing.Font("Segoe UI", 10, FontStyle.Bold) : new System.Drawing.Font("Segoe UI", 10);

			int textX = avatarX + avatarSize + 10;
			Rectangle textRect = new Rectangle(textX, e.Bounds.Top, e.Bounds.Width - textX, e.Bounds.Height);

			TextRenderer.DrawText(g, userName, font, textRect, textColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
		}

		// --- SEGÉDFÜGGVÉNYEK ---

		// Szín generálása név alapján (hogy mindig ugyanaz legyen az adott usernek)
		private Color GetUserColor(string username)
		{
			int hash = Math.Abs(username.GetHashCode());
			// Pasztell színek generálása
			int r = (hash % 100) + 100; // 100-200
			int g = ((hash / 100) % 100) + 100;
			int b = ((hash / 10000) % 100) + 100;
			// Kicsit sötétítünk rajta, hogy fehér alapon jól látsszon
			return Color.FromArgb(r, g, b);
		}

		private (string Time, string Name, string Content) ParseMessage(string raw)
		{
			// Formátum feltételezés: "HH:mm Név: Üzenet"
			// Ha nem illeszkedik, visszaadjuk az egészet tartalomnak
			try
			{
				var parts = raw.Split(new[] { ' ' }, 2);
				string time = parts[0];
				if (time.Contains(":"))
				{
					var rest = parts[1].Split(new[] { ':' }, 2);
					if (rest.Length == 2)
					{
						return (time, rest[0], rest[1].Trim());
					}
				}
			}
			catch { }
			return ("", "", raw);
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

		private void PnlBottom_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
			using (var pen = new Pen(Color.FromArgb(230, 230, 230))) { e.Graphics.DrawLine(pen, 0, 0, pnlBottom.Width, 0); }
			Rectangle rect = new Rectangle(txtMessage.Location.X - 10, txtMessage.Location.Y - 5, txtMessage.Width + 20, txtMessage.Height + 10);
			using (GraphicsPath path = GetRoundedPath(rect, 18)) using (var brush = new SolidBrush(Color.FromArgb(245, 245, 245))) { e.Graphics.FillPath(brush, path); }
		}

		private void UpdateControlRegions()
		{
			using (GraphicsPath path = new GraphicsPath()) { path.AddEllipse(0, 0, btnSend.Width, btnSend.Height); btnSend.Region = new Region(path); }
			using (GraphicsPath path = new GraphicsPath()) { path.AddEllipse(0, 0, btnFile.Width, btnFile.Height); btnFile.Region = new Region(path); }
			if (txtMessage.Width > 0 && txtMessage.Height > 0) txtMessage.Region = new Region(new Rectangle(2, 2, txtMessage.Width - 4, txtMessage.Height - 4));
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

		// Logika maradéka...
		private bool IsMyMessage(string msg) => msg.Contains($" {_myUsername}:") || msg.Contains($"[Private ->");
		private void HandleUserSelection(string selection) { if (selection == "[Global Chat]") { _currentChatPartner = null; lblTitle.Text = $"ChatApp - {_myUsername} (Global)"; } else { _currentChatPartner = selection; lblTitle.Text = $"ChatApp - {_myUsername} -> {_currentChatPartner}"; } RefreshChatView(); }
		private void OnUserListReceived(string[] users) { if (IsDisposed || !IsHandleCreated) return; Invoke((MethodInvoker)delegate { var currentSelection = lstUsers.SelectedItem; lstUsers.Items.Clear(); lstUsers.Items.Add("[Global Chat]"); foreach (var user in users) if (user != _myUsername) lstUsers.Items.Add(user); if (currentSelection != null && lstUsers.Items.Contains(currentSelection)) lstUsers.SelectedItem = currentSelection; }); }
		private void RefreshChatView() { lstMessages.Items.Clear(); foreach (var msg in _allMessages) { if (_currentChatPartner == null) { if (!msg.Contains("(privát)") && !msg.Contains("[Private ->") && !msg.Contains("(private)")) lstMessages.Items.Add(msg); } else { bool fromPartner = msg.Contains($"(privát) {_currentChatPartner}:") || msg.Contains($"(private) {_currentChatPartner}:"); bool toPartner = msg.Contains($"[Private -> {_currentChatPartner}]"); if (fromPartner || toPartner) lstMessages.Items.Add(msg); } } if (lstMessages.Items.Count > 0) lstMessages.TopIndex = lstMessages.Items.Count - 1; }
		private async Task SendMessage() { if (string.IsNullOrWhiteSpace(txtMessage.Text) || txtMessage.Text == PLACEHOLDER) return; string text = txtMessage.Text; if (_currentChatPartner != null) { await _service.SendPrivateMessageAsync(_currentChatPartner, text); _allMessages.Add($"{DateTime.Now:HH:mm} Me: {text}"); } else { await _service.SendMessageAsync(text); } txtMessage.Clear(); txtMessage.Focus(); RefreshChatView(); }
		private async Task SendFile() { using var dialog = new OpenFileDialog(); if (dialog.ShowDialog() == DialogResult.OK) { string filePath = dialog.FileName; string fileName = Path.GetFileName(filePath); byte[] fileBytes = await File.ReadAllBytesAsync(filePath); string recipient = _currentChatPartner ?? ""; await _service.SendFileAsync(filePath, recipient); string displayMsg = !string.IsNullOrEmpty(recipient) ? $"FILE [Private -> {recipient}]: {fileName} ({fileBytes.Length} byte) >>> CLICK TO SAVE <<<" : $"FILE [Global]: {fileName} ({fileBytes.Length} byte) >>> CLICK TO SAVE <<<"; if (_pendingFiles.ContainsKey(displayMsg)) displayMsg += $" [{DateTime.Now.Ticks}]"; _pendingFiles[displayMsg] = (fileName, fileBytes); _allMessages.Add(displayMsg); RefreshChatView(); } }
		private void HandleFileDownload(string selectedText) { if (_pendingFiles.ContainsKey(selectedText)) { var fileData = _pendingFiles[selectedText]; var result = MessageBox.Show($"Save file?\n\nName: {fileData.FileName}\nSize: {fileData.Content.Length} bytes", "Download", MessageBoxButtons.YesNo, MessageBoxIcon.Question); if (result == DialogResult.Yes) { SaveFileToDisk(fileData.FileName, fileData.Content); string newText = selectedText.Replace(">>> CLICK TO SAVE <<<", "[SAVED]"); int index = _allMessages.IndexOf(selectedText); if (index != -1) _allMessages[index] = newText; _pendingFiles.Remove(selectedText); _pendingFiles[newText] = fileData; RefreshChatView(); } } }
		private void OnMessageReceived(string msg) { if (IsDisposed || !IsHandleCreated) return; Invoke((MethodInvoker)delegate { _allMessages.Add(msg); RefreshChatView(); }); }
		private void OnFileReceived(string fileName, byte[] content) { if (IsDisposed || !IsHandleCreated) return; Invoke((MethodInvoker)delegate { string displayMsg = $"INCOMING FILE: {fileName} ({content.Length} byte) >>> CLICK TO SAVE <<<"; if (_pendingFiles.ContainsKey(displayMsg)) displayMsg += $" [{DateTime.Now.Ticks}]"; _pendingFiles[displayMsg] = (fileName, content); _allMessages.Add(displayMsg); RefreshChatView(); }); }
		private void SaveFileToDisk(string fileName, byte[] content) { try { string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads"); Directory.CreateDirectory(folder); string fullPath = Path.Combine(folder, fileName); File.WriteAllBytes(fullPath, content); MessageBox.Show($"File saved: {fullPath}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information); } catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); } }
		private void OnConnectionLost() { if (IsDisposed || !IsHandleCreated) return; Invoke((MethodInvoker)delegate { MessageBox.Show("Disconnected from server.", "Connection Lost", MessageBoxButtons.OK, MessageBoxIcon.Warning); Application.Exit(); }); }
	}
}