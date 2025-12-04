using System;
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
            btnSend.Click += async (s, e) => await SendMessage();
        }

        private async Task SendMessage()
        {
            if (string.IsNullOrWhiteSpace(txtMessage.Text)) return;
            await _service.SendMessageAsync(txtMessage.Text);
            txtMessage.Clear();
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

        private void OnConnectionLost()
        {
            if (IsDisposed) return;
            Invoke((MethodInvoker)delegate
            {
                MessageBox.Show("Connection lost from server.");
                Application.Exit();
            });
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}