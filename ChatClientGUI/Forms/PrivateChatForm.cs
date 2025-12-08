using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChatClientGUI.Services;

namespace ChatClientGUI.Forms
{
    public partial class PrivateChatForm : Form
    {
        private ClientService _service;
        private string _targetUser; 

        public PrivateChatForm(ClientService service, string targetUser)
        {
            InitializeComponent();
            _service = service;
            _targetUser = targetUser;
            this.Text = $"Privát: {_targetUser}"; 

            _service.MessageReceived += OnMessageReceived;
            btnSendPrivate.Click += async (s, e) => await SendPrivate();
        }

        private async Task SendPrivate()
        {s
            if (string.IsNullOrWhiteSpace(txtPrivateInput.Text)) return;

            string msg = txtPrivateInput.Text;
            await _service.SendPrivateMessageAsync(_targetUser, msg);

            lstPrivateMessages.Items.Add($"Én: {msg}");
            txtPrivateInput.Clear();
        }

        private void OnMessageReceived(string msg)
        {
            if (IsDisposed) return;

            Invoke((MethodInvoker)delegate
            {
                if (msg.StartsWith($"(privát) {_targetUser}:"))
                {
                    lstPrivateMessages.Items.Add(msg);
                    lstPrivateMessages.TopIndex = lstPrivateMessages.Items.Count - 1;
                }
            });
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _service.MessageReceived -= OnMessageReceived;
            base.OnFormClosing(e);
        }
    }
}