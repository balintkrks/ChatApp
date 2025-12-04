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
                await _service.SendFileAsync(dialog.FileName);
            }
        }

        private void OnMessageReceived(string msg)
        {
            if (IsDisposed) return;
            Invoke((MethodInvoker)delegate
            {
                lstMessages.Items.Add(msg);
                lstMessages.TopIndex = lstMessages.Items.Count - 1;
            });
        }

        private void OnFileReceived(string fileName, byte[] content)
        {
            if (IsDisposed) return;
            Invoke((MethodInvoker)delegate
            {
                var result = MessageBox.Show($"Fájl érkezett: {fileName}. Szeretnéd menteni?", "Fájlátvitel", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    using var dialog = new SaveFileDialog();
                    dialog.FileName = fileName;
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllBytes(dialog.FileName, content);
                        MessageBox.Show("Sikeres mentés.");
                    }
                }
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