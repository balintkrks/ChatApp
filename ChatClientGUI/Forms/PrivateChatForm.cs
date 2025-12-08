using System;
using System.Drawing;
using System.Drawing.Drawing2D;
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

			lstPrivateMessages.MeasureItem += LstPrivateMessages_MeasureItem;
			lstPrivateMessages.DrawItem += LstPrivateMessages_DrawItem;

			this.FormClosing += (s, e) => _service.MessageReceived -= OnMessageReceived;
		}

		private void LstPrivateMessages_MeasureItem(object sender, MeasureItemEventArgs e)
		{
			e.ItemHeight = 30;
		}

		private void LstPrivateMessages_DrawItem(object sender, DrawItemEventArgs e)
		{
			if (e.Index < 0) return;

			e.DrawBackground();
			string msg = lstPrivateMessages.Items[e.Index].ToString();
			Graphics g = e.Graphics;
			g.SmoothingMode = SmoothingMode.AntiAlias;

			bool isMe = msg.StartsWith("Me:");

			Color bubbleColor = isMe ? Color.DodgerBlue : Color.LightGray;
			Color textColor = isMe ? Color.White : Color.Black;

			var size = TextRenderer.MeasureText(g, msg, e.Font);
			int padding = 6;
			int bubbleWidth = size.Width + 2 * padding;
			int bubbleHeight = e.Bounds.Height - 6;

			Rectangle bubbleRect;
			if (isMe)
				bubbleRect = new Rectangle(e.Bounds.Right - bubbleWidth - 5, e.Bounds.Top + 3, bubbleWidth, bubbleHeight);
			else
				bubbleRect = new Rectangle(e.Bounds.Left + 5, e.Bounds.Top + 3, bubbleWidth, bubbleHeight);

			using (var brush = new SolidBrush(bubbleColor))
				g.FillRectangle(brush, bubbleRect);

			TextRenderer.DrawText(g, msg, e.Font, bubbleRect, textColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

			e.DrawFocusRectangle();
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