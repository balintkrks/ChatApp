using System;
using System.Collections.Generic;
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

			lstMessages.DoubleClick += (s, e) =>
			{
				if (lstMessages.SelectedItem != null)
				{
					HandleFileDownload(lstMessages.SelectedItem.ToString());
				}
			};

			lstUsers.SelectedIndexChanged += (s, e) =>
			{
				if (lstUsers.SelectedItem != null)
				{
					HandleUserSelection(lstUsers.SelectedItem.ToString());
				}
			};

			// Alapértelmezés
			lstUsers.Items.Add("[Global Chat]");
			lstUsers.SelectedIndex = 0;
		}

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
					if (user != _myUsername)
					{
						lstUsers.Items.Add(user);
					}
				}

				if (currentSelection != null && lstUsers.Items.Contains(currentSelection))
				{
					lstUsers.SelectedItem = currentSelection;
				}
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
					{
						lstMessages.Items.Add(msg);
					}
				}
				else
				{
					bool fromPartner = msg.Contains($"(privát) {_currentChatPartner}:") || msg.Contains($"(private) {_currentChatPartner}:");
					bool toPartner = msg.Contains($"[Private -> {_currentChatPartner}]");

					if (fromPartner || toPartner)
					{
						lstMessages.Items.Add(msg);
					}
				}
			}
			if (lstMessages.Items.Count > 0)
				lstMessages.TopIndex = lstMessages.Items.Count - 1;
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

				var result = MessageBox.Show(
					$"Save file?\n\nName: {fileData.FileName}\nSize: {fileData.Content.Length} bytes",
					"Download File",
					MessageBoxButtons.YesNo,
					MessageBoxIcon.Question);

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

				int count = 1;
				string nameOnly = Path.GetFileNameWithoutExtension(fileName);
				string ext = Path.GetExtension(fileName);
				while (File.Exists(fullPath))
				{
					fullPath = Path.Combine(folder, $"{nameOnly}_{count}{ext}");
					count++;
				}

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

		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			Application.Exit();
		}
	}
}