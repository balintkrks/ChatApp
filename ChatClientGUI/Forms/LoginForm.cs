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

            btnLogin.Click += async (s, e) => await Login();
            btnRegister.Click += async (s, e) => await Register();
        }

        private async Task ConnectIfNeeded()
        {
            bool connected = await _service.ConnectAsync("127.0.0.1", 5000);
            if (!connected)
            {
                MessageBox.Show("Could not connect to server!");
            }
        }

        private async Task Login()
        {
            await ConnectIfNeeded();
            string user = txtUsername.Text;
            string pass = txtPassword.Text;
            await _service.SendLoginAsync(user, pass);
        }

        private async Task Register()
        {
            await ConnectIfNeeded();
            string user = txtUsername.Text;
            string pass = txtPassword.Text;
            await _service.SendRegisterAsync(user, pass);
        }

        private void OnMessageReceived(string msg)
        {
            Invoke((MethodInvoker)delegate
            {
                if (msg.Contains("Login successful"))
                {
                    var mainForm = new MainForm(_service);
                    mainForm.Show();
                    this.Hide();
                }
                else if (msg.Contains("Login failed"))
                {
                    MessageBox.Show("Invalid credentials!");
                }
                else if (msg.Contains("Registration successful"))
                {
                    MessageBox.Show("Registration successful! Please login.");
                }
                else
                {
                    MessageBox.Show(msg);
                }
            });
        }
    }
}