using System;
using System.Windows.Forms;
using ChatClientGUI.Forms;

namespace ChatClientGUI
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new LoginForm());
        }
    }
}