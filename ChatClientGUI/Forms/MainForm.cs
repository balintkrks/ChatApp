using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChatClientGUI.Services;

namespace ChatClientGUI.Forms
{
    public partial class MainForm : Form
    {
        private ClientService _service;

        public MainForm(ClientService service)
        {
            InitializeComponent();
            _service = service;

            _service.MessageReceived += OnMessageReceived;
            _service.ConnectionLost += OnConnectionLost;
            _service.FileReceived += OnFileReceived;

            btnSend.Click += async (s, e) => await SendMessage();
            btnFile.Click += async (s, e) => await SendFile();

            lstUsers.DoubleClick += (s, e) => OpenPrivateChat();
        }

        private void OpenPrivateChat()
        {
            if (lstUsers.SelectedItem == null) return;

            string targetUser = lstUsers.SelectedItem.ToString();

            var privateForm = new PrivateChatForm(_service, targetUser);
            privateForm.Show();
        }

        private async Task SendMessage()
        {
            if (string.IsNullOrWhiteSpace(txtMessage.Text)) return;
            await _service.SendMessageAsync(txtMessage.Text);
            txtMessage.Clear();
        }

        private async Task SendFile()
        {
            using var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                await _service.SendFileAsync(dialog.FileName, "");
            }
        }

        private void OnMessageReceived(string msg)
        {
            if (IsDisposed) return;
            Invoke((MethodInvoker)delegate
            {
                if (msg.StartsWith("(privát)")) return;

                lstMessages.Items.Add(msg);
                lstMessages.TopIndex = lstMessages.Items.Count - 1;

                if (msg.Contains(":"))
                {
                    var parts = msg.Split(':', 2);
                    string sender = parts[0].Trim();

                    if (!sender.Contains("SERVER") && !lstUsers.Items.Contains(sender))
                    {
                        lstUsers.Items.Add(sender);
                    }
                }
            });
        }

        private void OnFileReceived(string fileName, byte[] content)
        {
            if (IsDisposed) return;
            Invoke((MethodInvoker)delegate
            {
                var result = MessageBox.Show($"Fájl érkezett a közösben: {fileName}\nSzeretnéd menteni?", "Fájl letöltés", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    using var dialog = new SaveFileDialog();
                    dialog.FileName = fileName;
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllBytes(dialog.FileName, content);
                        MessageBox.Show("Mentve!");
                    }
                }
            });
        }

        private void OnConnectionLost()
        {
            if (IsDisposed) return;
            Invoke((MethodInvoker)delegate
            {
                MessageBox.Show("Megszakadt a kapcsolat a szerverrel.");
                Application.Exit();
            });
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}