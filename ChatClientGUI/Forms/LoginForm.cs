using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChatClientGUI.Services;

namespace ChatClientGUI.Forms
{
    public partial class LoginForm : Form
    {
        private ClientService _service;

        public LoginForm()
        {
            InitializeComponent();
            _service = new ClientService();
            _service.MessageReceived += OnMessageReceived;

            btnLogin.Click += async (s, e) => await HandleLogin();
            btnRegister.Click += async (s, e) => await HandleRegister();
        }

        private async Task ConnectIfNeeded()
        {
            await _service.ConnectAsync("127.0.0.1", 5000);
        }

        private async Task HandleLogin()
        {
            await ConnectIfNeeded();

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
            await ConnectIfNeeded();
            string user = txtUsername.Text.Trim();
            string pass = txtPassword.Text.Trim();

            if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(pass))
            {
                await _service.SendRegisterAsync(user, pass);
            }
            else
            {
                MessageBox.Show("A regisztrációhoz meg kell adni felhasználónevet és jelszót.");
            }
        }

        private void OnMessageReceived(string msg)
        {
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
                    MessageBox.Show("Sikeres regisztráció. Most már bejelentkezhetsz.");
                }
                else if (msg.Contains("failed"))
                {
                    MessageBox.Show(msg);
                }
            });
        }
    }
}