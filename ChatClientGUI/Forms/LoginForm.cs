using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChatClientGUI.Services;

namespace ChatClientGUI.Forms
{
    public partial class LoginForm : Form
    {
        private readonly ClientService _service;

        public LoginForm()
        {
            InitializeComponent();
            _service = new ClientService();
            _service.MessageReceived += OnMessageReceived;

            btnLogin.Click += async (s, e) => await HandleLogin();
            btnRegister.Click += async (s, e) => await HandleRegister();
        }

        private async Task<bool> ConnectIfNeeded()
        {
            return await _service.ConnectAsync("127.0.0.1", 5000);
        }

        private async Task HandleLogin()
        {
            if (!await ConnectIfNeeded())
            {
                MessageBox.Show("Could not connect to server.", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string user = txtUsername.Text.Trim();
            string pass = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                await _service.SendLoginAnonAsync();
            }
            else
            {
                await _service.SendLoginAsync(user, pass);
            }
        }

        private async Task HandleRegister()
        {
            if (!await ConnectIfNeeded())
            {
                MessageBox.Show("Could not connect to server.", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string user = txtUsername.Text.Trim();
            string pass = txtPassword.Text.Trim();

            if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(pass))
            {
                await _service.SendRegisterAsync(user, pass);
            }
            else
            {
                MessageBox.Show("Username and password required.", "Registration", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void OnMessageReceived(string msg)
        {
            if (!IsHandleCreated) return;

            Invoke((MethodInvoker)delegate
            {
                if (msg.Contains("Login successful"))
                {
                    string username = msg.Contains("(Anon)") ? "Anon" : txtUsername.Text;
                    if (string.IsNullOrEmpty(username)) username = "Anon";

                    var mainForm = new MainForm(_service, username);
                    mainForm.Show();
                    this.Hide();
                }
                else if (msg.Contains("Registration successful"))
                {
                    MessageBox.Show("Registration successful! You can now login.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (msg.Contains("failed"))
                {
                    MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {

        }
    }
}