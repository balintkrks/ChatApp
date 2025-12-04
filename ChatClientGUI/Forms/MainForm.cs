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
        private ClientService _service;
        private string _myUsername;
        private string _currentChatPartner = null;

        private List<string> _allMessages = new List<string>();

        public MainForm(ClientService service, string myName)
        {
            InitializeComponent();
            _service = service;
            _myUsername = string.IsNullOrEmpty(myName) ? "Én" : myName;
            this.Text = $"ChatApp - {_myUsername}";

            _service.MessageReceived += OnMessageReceived;
            _service.ConnectionLost += OnConnectionLost;
            _service.FileReceived += OnFileReceived;

            btnSend.Click += async (s, e) => await SendMessage();
            btnFile.Click += async (s, e) => await SendFile();

            lstUsers.SelectedIndexChanged += (s, e) =>
            {
                if (lstUsers.SelectedItem != null)
                {
                    _currentChatPartner = lstUsers.SelectedItem.ToString();
                }
                else
                {
                    _currentChatPartner = null;
                }
                RefreshChatView();
            };

            lstUsers.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Right)
                {
                    lstUsers.ClearSelected();
                    _currentChatPartner = null;
                    RefreshChatView();
                }
            };
        }

        private void RefreshChatView()
        {
            lstMessages.Items.Clear();

            foreach (var msg in _allMessages)
            {
                if (_currentChatPartner == null)
                {
                    if (!msg.Contains("(privát)") && !msg.Contains("[Privát ->"))
                    {
                        lstMessages.Items.Add(msg);
                    }
                }
                else
                {
                    bool fromPartner = msg.Contains($"(privát) {_currentChatPartner}:");
                    bool toPartner = msg.Contains($"[Privát -> {_currentChatPartner}]");

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
                string myLog = $"[Privát -> {_currentChatPartner}]: {text}";
                _allMessages.Add(myLog);
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
                string recipient = _currentChatPartner ?? "";
                await _service.SendFileAsync(dialog.FileName, recipient);

                string fileName = Path.GetFileName(dialog.FileName);

                if (!string.IsNullOrEmpty(recipient))
                {
                    _allMessages.Add($"[Privát -> {recipient}] Fájl küldve: {fileName}");
                }
                else
                {
                    _allMessages.Add($"[Fájl küldve -> Mindenki]: {fileName}");
                }

                RefreshChatView();
            }
        }

        private void OnMessageReceived(string msg)
        {
            if (IsDisposed) return;
            Invoke((MethodInvoker)delegate
            {
                _allMessages.Add(msg);

                if (msg.Contains(":"))
                {
                    string senderCandidate = "";
                    var parts = msg.Split(':');

                    if (parts.Length >= 2 && int.TryParse(parts[0].Trim(), out _))
                    {
                        string temp = msg.Substring(msg.IndexOf(':') + 1).Trim();
                        if (temp.Contains(":"))
                        {
                            string contentWithUser = temp.Substring(temp.IndexOf(' ') + 1);
                            if (contentWithUser.Contains(":"))
                            {
                                senderCandidate = contentWithUser.Split(':')[0].Trim();
                            }
                        }
                    }
                    else
                    {
                        senderCandidate = parts[0].Replace("(privát)", "").Trim();
                    }

                    if (!string.IsNullOrEmpty(senderCandidate) &&
                        !senderCandidate.Contains("SERVER") &&
                        !senderCandidate.Contains("Login") &&
                        !lstUsers.Items.Contains(senderCandidate) &&
                        senderCandidate != _myUsername)
                    {
                        lstUsers.Items.Add(senderCandidate);
                    }
                }

                RefreshChatView();
            });
        }

        private void OnFileReceived(string fileName, byte[] content)
        {
            if (IsDisposed) return;
            Invoke((MethodInvoker)delegate
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

                    string logMsg = $"[RENDSZER]: Fájl érkezett és mentve: {Path.GetFileName(fullPath)}";
                    _allMessages.Add(logMsg);
                    RefreshChatView();
                }
                catch { }
            });
        }

        private void OnConnectionLost()
        {
            if (IsDisposed) return;
            Invoke((MethodInvoker)delegate
            {
                MessageBox.Show("Kapcsolat megszakadt.");
                Application.Exit();
            });
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}