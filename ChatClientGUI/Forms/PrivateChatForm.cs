using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChatClientGUI.Services;

namespace ChatClientGUI.Forms
{
	public partial class PrivateChatForm : Form
	{
		private readonly ClientService _service;
		private readonly string _targetUser;

		public PrivateChatForm(ClientService service, string targetUser)
		{
			InitializeComponent();
			_service = service;
			_targetUser = targetUser;
			this.Text = $"Private Chat - {_targetUser}";

			_service.MessageReceived += OnMessageReceived;
			btnSendPrivate.Click += async (s, e) => await SendPrivate();

			this.FormClosing += (s, e) => _service.MessageReceived -= OnMessageReceived;
		}

		private async Task SendPrivate()
		{
			if (string.IsNullOrWhiteSpace(txtPrivateInput.Text)) return;

			string msg = txtPrivateInput.Text;
			await _service.SendPrivateMessageAsync(_targetUser, msg);

			lstPrivateMessages.Items.Add($"Me: {msg}");
			txtPrivateInput.Clear();
			lstPrivateMessages.TopIndex = lstPrivateMessages.Items.Count - 1;
		}

		private void OnMessageReceived(string msg)
		{
			if (IsDisposed || !IsHandleCreated) return;

			Invoke((MethodInvoker)delegate
			{
				if (msg.StartsWith($"(privát) {_targetUser}:") || msg.StartsWith($"(private) {_targetUser}:"))
				{
					lstPrivateMessages.Items.Add(msg);
					lstPrivateMessages.TopIndex = lstPrivateMessages.Items.Count - 1;
				}
			});
		}
	}
}