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
        private const string PLACEHOLDER = "Írj egy üzenetet...";

        protected override CreateParams CreateParams
        {
            get { CreateParams cp = base.CreateParams; cp.ClassStyle |= CS_DROPSHADOW; return cp; }
        }

        public MainForm(ClientService service, string myName)
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            this.DoubleBuffered = true;

            _service = service;
            _myUsername = string.IsNullOrEmpty(myName) ? "Me" : myName;

            lblTitle.Text = $"ChatApp - {_myUsername}";

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

            txtMessage.Enter += (s, e) => { if (txtMessage.Text == PLACEHOLDER) { txtMessage.Text = ""; txtMessage.ForeColor = Color.Black; } };
            txtMessage.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(txtMessage.Text)) { txtMessage.Text = PLACEHOLDER; txtMessage.ForeColor = Color.Gray; } };

            txtMessage.TextChanged += (s, e) => AdjustInputHeight();

            txtMessage.KeyDown += async (s, e) =>
            {
                if (e.KeyCode == Keys.Enter && !e.Shift)
                {
                    e.SuppressKeyPress = true;
                    if (txtMessage.Text != PLACEHOLDER) await SendMessage();
                }
            };

            pnlBottom.Paint += PnlBottom_Paint;
            lstMessages.MeasureItem += LstMessages_MeasureItem;
            lstMessages.DrawItem += LstMessages_DrawItem;
            lstUsers.DrawItem += LstUsers_DrawItem;

            pnlHeader.MouseDown += (s, e) => { ReleaseCapture(); SendMessage(this.Handle, 0x112, 0xf012, 0); };
            lblTitle.MouseDown += (s, e) => { ReleaseCapture(); SendMessage(this.Handle, 0x112, 0xf012, 0); };

            this.SizeChanged += (s, e) => { this.Invalidate(); UpdateControlRegions(); pnlBottom.Invalidate(); };


            lstMessages.DoubleClick += (s, e) => { if (lstMessages.SelectedItem != null) HandleFileDownload(lstMessages.SelectedItem.ToString()); };
            lstUsers.SelectedIndexChanged += (s, e) => { if (lstUsers.SelectedItem != null) HandleUserSelection(lstUsers.SelectedItem.ToString()); };

            lstUsers.Items.Add("[Global Chat]");
            lstUsers.SelectedIndex = 0;
        }

        private void AdjustInputHeight()
        {
            if (txtMessage.Text == PLACEHOLDER) return;

            int padding = 25;
            int minHeight = 50;
            int maxHeight = 100;

            Size sz = txtMessage.GetPreferredSize(new Size(txtMessage.Width, 0));
            int requiredHeight = sz.Height + padding;

            if (requiredHeight < minHeight) requiredHeight = minHeight;

            if (requiredHeight >= maxHeight)
            {
                pnlBottom.Height = maxHeight;
                txtMessage.ScrollBars = ScrollBars.Vertical;
            }
            else
            {
                pnlBottom.Height = requiredHeight;
                txtMessage.ScrollBars = ScrollBars.None;
            }
        }

        private (string Time, string Name, string Content) ParseMessage(string raw)
        {
            if (raw.StartsWith("[Private ->"))
            {
                int closeBracket = raw.IndexOf(']');
                if (closeBracket > 0) raw = raw.Substring(closeBracket + 1).Trim();
            }

            if (raw.StartsWith("(privát)") || raw.StartsWith("(private)"))
            {
                raw = raw.Substring(8).Trim();
            }

            try
            {
                if (raw.StartsWith("Me:") || (raw.Contains("Me:") && !raw.Contains(": Me:")))
                {
                    var parts = raw.Split(new[] { ' ' }, 2);
                    if (parts.Length == 2 && parts[0].Contains(":"))
                    {
                        var contentPart = parts[1].Substring(3).Trim();
                        return (parts[0], "Me", contentPart);
                    }
                }

                var firstSpaceIndex = raw.IndexOf(' ');
                if (firstSpaceIndex > 0)
                {
                    string time = raw.Substring(0, firstSpaceIndex);
                    string rest = raw.Substring(firstSpaceIndex + 1);

                    var colonIndex = rest.IndexOf(':');
                    if (colonIndex > 0)
                    {
                        string name = rest.Substring(0, colonIndex);
                        string content = rest.Substring(colonIndex + 1).Trim();
                        return (time, name, content);
                    }
                }
            }
            catch { }

            return ("", "", raw);
        }

        private void LstMessages_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= lstMessages.Items.Count) return;
            string fullMsg = lstMessages.Items[e.Index].ToString();

            if (fullMsg.Contains("FILE") || fullMsg.Contains("System") || fullMsg.Contains("Server")) { e.ItemHeight = 40; return; }

            var parts = ParseMessage(fullMsg);

            int scrollBarWidth = SystemInformation.VerticalScrollBarWidth;
            int safeWidth = lstMessages.Width - scrollBarWidth - 10;
            int maxWidth = (int)(safeWidth * 0.7);
            if (maxWidth < 100) maxWidth = 100;

            Size size = TextRenderer.MeasureText(e.Graphics, parts.Content, new Font("Segoe UI", 10), new Size(maxWidth, 0), TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl);
            e.ItemHeight = size.Height + 45;
        }

        private void LstMessages_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;
            e.Graphics.FillRectangle(Brushes.White, e.Bounds);

            string fullMsg = lstMessages.Items[e.Index].ToString();
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            if (fullMsg.Contains("FILE") || fullMsg.Contains("System") || fullMsg.Contains("Server") || fullMsg.Contains("SERVER:"))
            {
                TextRenderer.DrawText(g, fullMsg, new System.Drawing.Font("Segoe UI", 9, FontStyle.Italic), e.Bounds, Color.Gray, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak);
                return;
            }

            var parsed = ParseMessage(fullMsg);
            bool isMe = parsed.Name == "Me" || parsed.Name == _myUsername;

            Color bubbleColor = isMe ? Color.FromArgb(0, 132, 255) : Color.FromArgb(240, 240, 240);
            Color textColor = isMe ? Color.White : Color.Black;
            Color metaColor = isMe ? Color.FromArgb(220, 220, 220) : Color.Gray;

            int scrollBarWidth = SystemInformation.VerticalScrollBarWidth;
            int safeWidth = lstMessages.Width - scrollBarWidth - 10;
            int maxWidth = (int)(safeWidth * 0.7);
            if (maxWidth < 100) maxWidth = 100;

            Size contentSize = TextRenderer.MeasureText(g, parsed.Content, new System.Drawing.Font("Segoe UI", 10), new Size(maxWidth, 0), TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl);

            int bubbleWidth = Math.Max(contentSize.Width + 40, 120);
            int bubbleHeight = contentSize.Height + 35;

            Rectangle bubbleRect;
            if (isMe)
                bubbleRect = new Rectangle(e.Bounds.Right - bubbleWidth - 25, e.Bounds.Top + 5, bubbleWidth, bubbleHeight);
            else
                bubbleRect = new Rectangle(e.Bounds.Left + 15, e.Bounds.Top + 5, bubbleWidth, bubbleHeight);

            using (GraphicsPath path = GetRoundedPath(bubbleRect, 12))
            using (var brush = new SolidBrush(bubbleColor))
            {
                g.FillPath(brush, path);
            }

            int paddingX = 12;
            int paddingY = 8;
            Font nameFont = new System.Drawing.Font("Segoe UI", 9, FontStyle.Bold);
            Size nameSize = TextRenderer.MeasureText(g, parsed.Name, nameFont);

            Point namePos = new Point(bubbleRect.Left + paddingX, bubbleRect.Top + paddingY);
            TextRenderer.DrawText(g, parsed.Name, nameFont, namePos, isMe ? Color.White : GetUserColor(parsed.Name));

            Point timePos = new Point(namePos.X + nameSize.Width + 8, namePos.Y + 2);

            string displayTime = string.IsNullOrEmpty(parsed.Time) ? "" : parsed.Time;
            if (fullMsg.Contains("(privát)") || fullMsg.Contains("(private)")) displayTime = "🔒";

            TextRenderer.DrawText(g, displayTime, new System.Drawing.Font("Segoe UI", 8), timePos, metaColor);

            Rectangle textRect = new Rectangle(bubbleRect.Left + paddingX, bubbleRect.Top + 25, bubbleRect.Width - (paddingX * 2), bubbleRect.Height - 30);
            TextRenderer.DrawText(g, parsed.Content, new System.Drawing.Font("Segoe UI", 10),
                textRect, textColor, TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl);
        }

        private Color GetUserColor(string username)
        {
            int hash = Math.Abs(username.GetHashCode());
            int r = (hash % 120) + 50;
            int g = ((hash / 100) % 120) + 50;
            int b = ((hash / 10000) % 120) + 50;
            return Color.FromArgb(r, g, b);
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

        private void LstUsers_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;
            e.DrawBackground();
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected) e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(235, 245, 255)), e.Bounds);
            else e.Graphics.FillRectangle(Brushes.White, e.Bounds);

            string userName = lstUsers.Items[e.Index].ToString();
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            int dotSize = 8;
            int dotX = e.Bounds.Left + 15;
            int dotY = e.Bounds.Top + (e.Bounds.Height - dotSize) / 2;
            Color dotColor = (userName == "[Global Chat]") ? Color.DodgerBlue : Color.LimeGreen;

            using (var brush = new SolidBrush(dotColor)) g.FillEllipse(brush, dotX, dotY, dotSize, dotSize);

            Color textColor = Color.FromArgb(50, 50, 50);
            System.Drawing.Font font = (userName == "[Global Chat]") ? new System.Drawing.Font("Segoe UI", 9F, FontStyle.Bold) : new System.Drawing.Font("Segoe UI", 9F);
            TextRenderer.DrawText(g, userName, font, new Rectangle(dotX + 20, e.Bounds.Top, e.Bounds.Width - 30, e.Bounds.Height), textColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
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

        }

        protected override void OnPaint(PaintEventArgs e) { e.Graphics.SmoothingMode = SmoothingMode.AntiAlias; using (Pen pen = new Pen(Color.LightGray, 1)) { e.Graphics.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1); } }
        private bool IsMyMessage(string msg) => msg.Contains($" {_myUsername}:") || msg.Contains($"[Private ->");
        private void HandleUserSelection(string selection) { if (selection == "[Global Chat]") { _currentChatPartner = null; lblTitle.Text = $"ChatApp - {_myUsername} (Global)"; } else { _currentChatPartner = selection; lblTitle.Text = $"ChatApp - {_myUsername} -> {_currentChatPartner}"; } RefreshChatView(); }
        private void OnUserListReceived(string[] users) { if (IsDisposed || !IsHandleCreated) return; Invoke((MethodInvoker)delegate { var currentSelection = lstUsers.SelectedItem; lstUsers.Items.Clear(); lstUsers.Items.Add("[Global Chat]"); foreach (var user in users) if (user != _myUsername) lstUsers.Items.Add(user); if (currentSelection != null && lstUsers.Items.Contains(currentSelection)) lstUsers.SelectedItem = currentSelection; }); }

        private void RefreshChatView()
        {
            lstMessages.Items.Clear();
            foreach (var msg in _allMessages)
            {
                if (msg.Contains("SERVER:") || msg.Contains("System"))
                {
                    lstMessages.Items.Add(msg);
                    continue;
                }

                if (_currentChatPartner == null)
                {
                    if (!msg.Contains("(privát)") && !msg.Contains("[Private ->") && !msg.Contains("(private)"))
                        lstMessages.Items.Add(msg);
                }
                else
                {
                    bool fromPartner = msg.Contains($"(privát) {_currentChatPartner}:") || msg.Contains($"(private) {_currentChatPartner}:");
                    bool toPartner = msg.Contains($"[Private -> {_currentChatPartner}]");
                    bool fileFromPartner = msg.Contains($"FILE [{_currentChatPartner}]:");

                    if (fromPartner || toPartner || fileFromPartner)
                        lstMessages.Items.Add(msg);
                }
            }
            if (lstMessages.Items.Count > 0) lstMessages.TopIndex = lstMessages.Items.Count - 1;
        }

        private async Task SendMessage()
        {
            if (string.IsNullOrWhiteSpace(txtMessage.Text) || txtMessage.Text == PLACEHOLDER) return;
            string text = txtMessage.Text;
            if (_currentChatPartner != null)
            {
                await _service.SendPrivateMessageAsync(_currentChatPartner, text);
                _allMessages.Add($"[Private -> {_currentChatPartner}] {DateTime.Now:HH:mm} Me: {text}");
            }
            else
            {
                await _service.SendMessageAsync(text);
            }
            txtMessage.Clear();
            txtMessage.Focus();
            RefreshChatView();
        }

        private async Task SendFile() { using var dialog = new OpenFileDialog(); if (dialog.ShowDialog() == DialogResult.OK) { string filePath = dialog.FileName; string fileName = Path.GetFileName(filePath); byte[] fileBytes = await File.ReadAllBytesAsync(filePath); string recipient = _currentChatPartner ?? ""; await _service.SendFileAsync(filePath, recipient); string displayMsg = !string.IsNullOrEmpty(recipient) ? $"FILE [Private -> {recipient}]: {fileName} ({fileBytes.Length} byte) >>> CLICK TO SAVE <<<" : $"FILE [Global]: {fileName} ({fileBytes.Length} byte) >>> CLICK TO SAVE <<<"; if (_pendingFiles.ContainsKey(displayMsg)) displayMsg += $" [{DateTime.Now.Ticks}]"; _pendingFiles[displayMsg] = (fileName, fileBytes); _allMessages.Add(displayMsg); RefreshChatView(); } }
        private void HandleFileDownload(string selectedText) { if (_pendingFiles.ContainsKey(selectedText)) { var fileData = _pendingFiles[selectedText]; var result = MessageBox.Show($"Save file?\n\nName: {fileData.FileName}\nSize: {fileData.Content.Length} bytes", "Download", MessageBoxButtons.YesNo, MessageBoxIcon.Question); if (result == DialogResult.Yes) { SaveFileToDisk(fileData.FileName, fileData.Content); string newText = selectedText.Replace(">>> CLICK TO SAVE <<<", "[SAVED]"); int index = _allMessages.IndexOf(selectedText); if (index != -1) _allMessages[index] = newText; _pendingFiles.Remove(selectedText); _pendingFiles[newText] = fileData; RefreshChatView(); } } }
        private void OnMessageReceived(string msg) { if (IsDisposed || !IsHandleCreated) return; Invoke((MethodInvoker)delegate { _allMessages.Add(msg); RefreshChatView(); }); }

        private void OnFileReceived(string sender, string fileName, byte[] content)
        {
            if (IsDisposed || !IsHandleCreated) return;
            Invoke((MethodInvoker)delegate
            {
                string displayMsg = $"FILE [{sender}]: {fileName} ({content.Length} byte) >>> CLICK TO SAVE <<<";
                if (_pendingFiles.ContainsKey(displayMsg)) displayMsg += $" [{DateTime.Now.Ticks}]";
                _pendingFiles[displayMsg] = (fileName, content);
                _allMessages.Add(displayMsg);
                RefreshChatView();
            });
        }

        private void SaveFileToDisk(string fileName, byte[] content) { try { string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads"); Directory.CreateDirectory(folder); string fullPath = Path.Combine(folder, fileName); File.WriteAllBytes(fullPath, content); MessageBox.Show($"File saved: {fullPath}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information); } catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); } }
        private void OnConnectionLost() { if (IsDisposed || !IsHandleCreated) return; Invoke((MethodInvoker)delegate { MessageBox.Show("Disconnected from server.", "Connection Lost", MessageBoxButtons.OK, MessageBoxIcon.Warning); Application.Exit(); }); }
    }
}