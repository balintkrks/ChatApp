using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChatClientGUI.Services;

namespace ChatClientGUI.Forms
{
	public partial class MainForm : Form
	{
		private readonly ClientService _service;
		private readonly string _myUsername;
		private string _currentChatPartner = null;
		private readonly List<string> _allMessages = new List<string>();
		private readonly Dictionary<string, (string FileName, byte[] Content)> _pendingFiles = new Dictionary<string, (string, byte[])>();

		public MainForm(ClientService service, string myName)
		{
			InitializeComponent();
			_service = service;
			_myUsername = string.IsNullOrEmpty(myName) ? "Me" : myName;
			this.Text = $"ChatApp - {_myUsername}";

			_service.MessageReceived += OnMessageReceived;
			_service.ConnectionLost += OnConnectionLost;
			_service.FileReceived += OnFileReceived;
			_service.UserListReceived += OnUserListReceived;

			btnSend.Click += async (s, e) => await SendMessage();
			btnFile.Click += async (s, e) => await SendFile();
			btnExit.Click += (s, e) => Application.Exit();

			// Enter küldés
			txtMessage.KeyDown += async (s, e) =>
			{
				if (e.KeyCode == Keys.Enter && !e.Shift)
				{
					e.SuppressKeyPress = true;
					await SendMessage();
				}
			};

			// Rajzolás
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

		// --- FELHASZNÁLÓ LISTA ---
		private void LstUsers_DrawItem(object sender, DrawItemEventArgs e)
		{
			if (e.Index < 0) return;

			e.DrawBackground();
			if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
				e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(220, 240, 255)), e.Bounds);
			else
				e.Graphics.FillRectangle(new SolidBrush(Color.WhiteSmoke), e.Bounds);

			string userName = lstUsers.Items[e.Index].ToString();
			Graphics g = e.Graphics;
			g.SmoothingMode = SmoothingMode.AntiAlias;
			g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit; // Élesebb szöveg

			int dotSize = 10;
			int dotX = e.Bounds.Left + 10;
			int dotY = e.Bounds.Top + (e.Bounds.Height - dotSize) / 2;
			Color dotColor = (userName == "[Global Chat]") ? Color.DodgerBlue : Color.LimeGreen;

			using (var brush = new SolidBrush(dotColor))
				g.FillEllipse(brush, dotX, dotY, dotSize, dotSize);

			Color textColor = Color.Black;
			Font font = (userName == "[Global Chat]") ? new Font(e.Font, FontStyle.Bold) : e.Font;
			int textX = dotX + dotSize + 10;
			Rectangle textRect = new Rectangle(textX, e.Bounds.Top, e.Bounds.Width - textX, e.Bounds.Height);

			TextRenderer.DrawText(g, userName, font, textRect, textColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
		}

		// --- DINAMIKUS MÉRETEZÉS ---
		private void LstMessages_MeasureItem(object sender, MeasureItemEventArgs e)
		{
			if (e.Index < 0 || e.Index >= lstMessages.Items.Count) return;

			string msg = lstMessages.Items[e.Index].ToString();

			// Max szélesség: a lista szélességének kb 70%-a
			int maxWidth = (int)(lstMessages.Width * 0.7);

			// Szöveg mérése tördeléssel
			Size size = TextRenderer.MeasureText(e.Graphics, msg, lstMessages.Font, new Size(maxWidth, 0), TextFormatFlags.WordBreak);

			// Magasság + margók (fent 10 + lent 10)
			e.ItemHeight = size.Height + 20;
		}

		// --- BUBORÉK RAJZOLÁS ---
		private void LstMessages_DrawItem(object sender, DrawItemEventArgs e)
		{
			if (e.Index < 0) return;

			e.DrawBackground();
			string msg = lstMessages.Items[e.Index].ToString();
			Graphics g = e.Graphics;

			// Minőség javítása (homályosság ellen)
			g.SmoothingMode = SmoothingMode.AntiAlias;
			g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

			bool isMe = IsMyMessage(msg);
			bool isSystem = msg.Contains("FILE") || msg.Contains("System") || msg.Contains("Server");

			if (isSystem)
			{
				TextRenderer.DrawText(g, msg, new Font(e.Font, FontStyle.Italic), e.Bounds, Color.Gray, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak);
			}
			else
			{
				Color bubbleColor = isMe ? Color.FromArgb(0, 120, 215) : Color.FromArgb(230, 230, 230);
				Color textColor = isMe ? Color.White : Color.Black;

				// Max szélesség a buboréknak
				int maxWidth = (int)(lstMessages.Width * 0.7);

				// Szöveg mérése pontosan úgy, mint a MeasureItem-ben
				Size size = TextRenderer.MeasureText(g, msg, e.Font, new Size(maxWidth, 0), TextFormatFlags.WordBreak);

				// Buborék méretei
				int bubbleWidth = size.Width + 20; // + Padding oldalt
				int bubbleHeight = size.Height + 10; // + Padding fent/lent

				Rectangle bubbleRect;
				if (isMe)
				{
					// Jobbra igazítva
					bubbleRect = new Rectangle(e.Bounds.Right - bubbleWidth - 10, e.Bounds.Top + 5, bubbleWidth, bubbleHeight);
				}
				else
				{
					// Balra igazítva
					bubbleRect = new Rectangle(e.Bounds.Left + 10, e.Bounds.Top + 5, bubbleWidth, bubbleHeight);
				}

				// Buborék rajzolása
				using (GraphicsPath path = GetRoundedPath(bubbleRect, 10))
				using (var brush = new SolidBrush(bubbleColor))
				{
					g.FillPath(brush, path);
				}

				// Szöveg rajzolása a buborék közepére (WordBreak fontos!)
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

		private bool IsMyMessage(string msg)
		{
			return msg.Contains($" {_myUsername}:") || msg.Contains($"[Private ->");
		}

		// --- TOVÁBBI LOGIKA (változatlan) ---
		private void HandleUserSelection(string selection)
		{
			if (selection == "[Global Chat]")
			{
				_currentChatPartner = null;
				this.Text = $"ChatApp - {_myUsername} (Global)";
			}
			else
			{
				_currentChatPartner = selection;
				this.Text = $"ChatApp - {_myUsername} -> {_currentChatPartner}";
			}
			RefreshChatView();
		}

		private void OnUserListReceived(string[] users)
		{
			if (IsDisposed || !IsHandleCreated) return;
			Invoke((MethodInvoker)delegate
			{
				var currentSelection = lstUsers.SelectedItem;
				lstUsers.Items.Clear();
				lstUsers.Items.Add("[Global Chat]");
				foreach (var user in users)
				{
					if (user != _myUsername) lstUsers.Items.Add(user);
				}
				if (currentSelection != null && lstUsers.Items.Contains(currentSelection))
					lstUsers.SelectedItem = currentSelection;
			});
		}

		private void RefreshChatView()
		{
			lstMessages.Items.Clear();
			foreach (var msg in _allMessages)
			{
				if (_currentChatPartner == null)
				{
					if (!msg.Contains("(privát)") && !msg.Contains("[Private ->") && !msg.Contains("(private)"))
						lstMessages.Items.Add(msg);
				}
				else
				{
					bool fromPartner = msg.Contains($"(privát) {_currentChatPartner}:") || msg.Contains($"(private) {_currentChatPartner}:");
					bool toPartner = msg.Contains($"[Private -> {_currentChatPartner}]");
					if (fromPartner || toPartner) lstMessages.Items.Add(msg);
				}
			}
			if (lstMessages.Items.Count > 0) lstMessages.TopIndex = lstMessages.Items.Count - 1;
		}

		private async Task SendMessage()
		{
			if (string.IsNullOrWhiteSpace(txtMessage.Text)) return;
			string text = txtMessage.Text;
			if (_currentChatPartner != null)
			{
				await _service.SendPrivateMessageAsync(_currentChatPartner, text);
				_allMessages.Add($"[Private -> {_currentChatPartner}]: {text}");
			}
			else
			{
				await _service.SendMessageAsync(text);
			}
			txtMessage.Clear();
			RefreshChatView();
		}

		private async Task SendFile()
		{
			using var dialog = new OpenFileDialog();
			if (dialog.ShowDialog() == DialogResult.OK)
			{
				string filePath = dialog.FileName;
				string fileName = Path.GetFileName(filePath);
				byte[] fileBytes = await File.ReadAllBytesAsync(filePath);
				string recipient = _currentChatPartner ?? "";
				await _service.SendFileAsync(filePath, recipient);

				string displayMsg = !string.IsNullOrEmpty(recipient)
					? $"FILE [Private -> {recipient}]: {fileName} ({fileBytes.Length} byte) >>> CLICK TO SAVE <<<"
					: $"FILE [Global]: {fileName} ({fileBytes.Length} byte) >>> CLICK TO SAVE <<<";

				if (_pendingFiles.ContainsKey(displayMsg)) displayMsg += $" [{DateTime.Now.Ticks}]";
				_pendingFiles[displayMsg] = (fileName, fileBytes);
				_allMessages.Add(displayMsg);
				RefreshChatView();
			}
		}

		private void HandleFileDownload(string selectedText)
		{
			if (_pendingFiles.ContainsKey(selectedText))
			{
				var fileData = _pendingFiles[selectedText];
				var result = MessageBox.Show($"Save file?\n\nName: {fileData.FileName}\nSize: {fileData.Content.Length} bytes", "Download", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
				if (result == DialogResult.Yes)
				{
					SaveFileToDisk(fileData.FileName, fileData.Content);
					string newText = selectedText.Replace(">>> CLICK TO SAVE <<<", "[SAVED]");
					int index = _allMessages.IndexOf(selectedText);
					if (index != -1) _allMessages[index] = newText;
					_pendingFiles.Remove(selectedText);
					_pendingFiles[newText] = fileData;
					RefreshChatView();
				}
			}
		}

		private void OnMessageReceived(string msg)
		{
			if (IsDisposed || !IsHandleCreated) return;
			Invoke((MethodInvoker)delegate
			{
				_allMessages.Add(msg);
				RefreshChatView();
			});
		}

		private void OnFileReceived(string fileName, byte[] content)
		{
			if (IsDisposed || !IsHandleCreated) return;
			Invoke((MethodInvoker)delegate
			{
				string displayMsg = $"INCOMING FILE: {fileName} ({content.Length} byte) >>> CLICK TO SAVE <<<";
				if (_pendingFiles.ContainsKey(displayMsg)) displayMsg += $" [{DateTime.Now.Ticks}]";
				_pendingFiles[displayMsg] = (fileName, content);
				_allMessages.Add(displayMsg);
				RefreshChatView();
			});
		}

		private void SaveFileToDisk(string fileName, byte[] content)
		{
			try
			{
				string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
				Directory.CreateDirectory(folder);
				string fullPath = Path.Combine(folder, fileName);
				File.WriteAllBytes(fullPath, content);
				MessageBox.Show($"File saved: {fullPath}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void OnConnectionLost()
		{
			if (IsDisposed || !IsHandleCreated) return;
			Invoke((MethodInvoker)delegate
			{
				MessageBox.Show("Disconnected from server.", "Connection Lost", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				Application.Exit();
			});
		}
	}
}