using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraTab;
using Siticone.UI.WinForms.Suite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Django
{
    public partial class DjangoTestForm : DevExpress.XtraBars.FluentDesignSystem.FluentDesignForm
    {

        string toolName = "Django";

        string version = "1.0.0";

        public Module _module = new Module();

        public string selectedDLL;

        public static bool IsloggedIn;

        public DjangoTestForm()
        {
            InitializeComponent();
        }

        static string random_string()
        {
            string str = null;

            Random random = new Random();
            for (int i = 0; i < 5; i++)
            {
                str += Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65))).ToString();
            }
            return str;

        }

        private void GetProcesses()
        {
            Process[] processes = Process.GetProcesses();
            List<string> list = new List<string>();
            foreach (Process p in processes)
            {
                if (!String.IsNullOrEmpty(p.MainWindowTitle))
                {
                    list.Add(p.MainWindowTitle);
                    //list.Add(p.ProcessName);
                    //Thread.Sleep(1000);
                }
                listBoxControl2.DataSource = list;
            }
        }

        public static bool IsAdmin =>
    new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

        private void DjangoTestForm_Load(object sender, EventArgs e)
        {
            KeyAuthApp.init();

            if (KeyAuthApp.response.message == "invalidver")
            {
                if (!string.IsNullOrEmpty(KeyAuthApp.app_data.downloadLink))
                {
                    DialogResult dialogResult = XtraMessageBox.Show($"Django version {version} is out of date. Would you like to be redirected to the download page?\nSelect 'Yes' to open file in browser or 'No' to download file automatically", $"{toolName} Auto update", MessageBoxButtons.YesNo);
                    switch (dialogResult)
                    {
                        case DialogResult.Yes:
                            Process.Start(KeyAuthApp.app_data.downloadLink);
                            Environment.Exit(0);
                            break;
                        case DialogResult.No:
                            WebClient webClient = new WebClient();
                            string destFile = Application.ExecutablePath;

                            string rand = random_string();

                            destFile = destFile.Replace(".exe", $"-{rand}.exe");
                            webClient.DownloadFile(KeyAuthApp.app_data.downloadLink, destFile);

                            Process.Start(destFile);
                            Process.Start(new ProcessStartInfo()
                            {
                                Arguments = "/C choice /C Y /N /D Y /T 3 & Del \"" + Application.ExecutablePath + "\"",
                                WindowStyle = ProcessWindowStyle.Hidden,
                                CreateNoWindow = true,
                                FileName = "cmd.exe"
                            });
                            Environment.Exit(0);

                            break;
                        default:
                            XtraMessageBox.Show("Invalid option", toolName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Environment.Exit(0);
                            break;
                    }
                }
                XtraMessageBox.Show($"Version {version} is out of date. Furthermore, the download link isn't set online. You will need to manually obtain the download link from the developer", toolName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }

            if (!KeyAuthApp.response.success)
            {
                XtraMessageBox.Show(KeyAuthApp.response.message, toolName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }

            if (IsAdmin)
            {
                adminLabel.Caption = "Admin Status: True";
            }
            else if (!IsAdmin)
            {
                adminLabel.Caption = "Admin Status: False";
            }

            versionLabel.Caption = $"v{KeyAuthApp.version}";

            chatButton.Enabled = false;
            logoutButton.Enabled = false;

            // Hiding tab headers
            djangoTabs.ShowTabHeader = (DevExpress.Utils.DefaultBoolean)1;

            // Clear
            subDaysLabel.Text = "";
            createDateLabel.Text = "";
            hwidLabel.Text = "";
            ipLabel.Text = "";
            lastLoginLabel.Text = "";
            loggedInLabel.Caption = "";

            themeSelection.Enabled = false;

            GetProcesses();
        }

        private void accordionControlElement1_Click(object sender, EventArgs e)
        {

        }

        private void iconsButton_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://icons8.com/icons/");
        }

        private void homeGroup_Click(object sender, EventArgs e)
        {
            djangoTabs.SelectedTabPage = homePage;
        }

        private void spooferGroup_Click(object sender, EventArgs e)
        {
            djangoTabs.SelectedTabPage = spooferPage;
        }

        private void injectorGroup_Click(object sender, EventArgs e)
        {
            djangoTabs.SelectedTabPage = injectorPage;
        }

        private void accountGroup_Click(object sender, EventArgs e)
        {
        }

        private void settingsGroup_Click(object sender, EventArgs e)
        {
            djangoTabs.SelectedTabPage = settingsPage;
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            djangoTabs.SelectedTabPage = loginPage;
        }

        private void chatButton_Click(object sender, EventArgs e)
        {
            djangoTabs.SelectedTabPage = chatPage;
        }

        private void forgotMyPasswordLink_Click(object sender, EventArgs e)
        {
            KeyAuthApp.forgot(username.Text, email.Text);
            XtraMessageBox.Show(KeyAuthApp.response.message, "Django", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public string expirydaysleft()
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Local);
            dtDateTime = dtDateTime.AddSeconds(long.Parse(KeyAuthApp.user_data.subscriptions[0].expiry)).ToLocalTime();
            TimeSpan difference = dtDateTime - DateTime.Now;
            return Convert.ToString(difference.Days + " Days " + difference.Hours + " Hours Left");
        }

        public DateTime UnixTimeToDateTime(long unixtime)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Local);
            try
            {
                dtDateTime = dtDateTime.AddSeconds(unixtime).ToLocalTime();
            }
            catch
            {
                dtDateTime = DateTime.MaxValue;
            }
            return dtDateTime;
        }

        private void loginAuthButton_Click(object sender, EventArgs e)
        {
            KeyAuthApp.login(username.Text, password.Text);
            if (KeyAuthApp.response.success)
            {
                XtraMessageBox.Show("Logged In Successfully", "Django", MessageBoxButtons.OK, MessageBoxIcon.Information);

                chatButton.Enabled = true;
                logoutButton.Enabled = true;

                // Get User Information
                subDaysLabel.Text = $"Expires In : {expirydaysleft()}";
                createDateLabel.Text = $"Creation Date: {UnixTimeToDateTime(long.Parse(KeyAuthApp.user_data.createdate))}";
                hwidLabel.Text = $"HWID: {KeyAuthApp.user_data.hwid}";
                ipLabel.Text = $"IP: {KeyAuthApp.user_data.ip}";
                lastLoginLabel.Text = $"Last Login: {UnixTimeToDateTime(long.Parse(KeyAuthApp.user_data.lastlogin))}";

                themeSelection.Enabled = true;

                IsloggedIn = true;

                loggedInLabel.Caption = $"Hello, {KeyAuthApp.user_data.username}";
 
            }
            else
            {
                IsloggedIn = false;
                XtraMessageBox.Show(KeyAuthApp.response.message, "Django", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void registerButton_Click(object sender, EventArgs e)
        {
            string email = this.email.Text;
            if (email == "Email")
            {
                email = null;
            }

            KeyAuthApp.register(username.Text, password.Text, license.Text, email);
            if (KeyAuthApp.response.success)
            {
                email = null;
                XtraMessageBox.Show("Registered Successfully", "Django", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                XtraMessageBox.Show(KeyAuthApp.response.message, "Django", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void logoutButton_Click(object sender, EventArgs e)
        {
            if (IsloggedIn)
            {
                KeyAuthApp.logout();
                if (KeyAuthApp.response.success)
                {
                    XtraMessageBox.Show("Successfully logged out. Hope to see you soon!", "Django", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    chatButton.Enabled = false;
                    logoutButton.Enabled = false;
                    themeSelection.Enabled = false;

                    loggedInLabel.Caption = $"";
                }
            }
            else if (!IsloggedIn)
            {
                XtraMessageBox.Show("You are not logged in!", toolName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void selectDLLButton_Click(object sender, EventArgs e)
        {
            XtraOpenFileDialog openDll = new XtraOpenFileDialog();
            openDll.Filter = "Dynamic Link Library (*.dll)|*.dll";
            openDll.FilterIndex = 2;
            openDll.RestoreDirectory = true;

            if (openDll.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    System.IO.Stream stream;
                    if ((stream = openDll.OpenFile()) == null)
                    {
                        return;
                    }

                    using (stream)
                    {
                        selectedDLL = openDll.FileName;
                        selectedDLLLabel.Text = $"Selected DLL: {openDll.SafeFileName}";
                    }

                    injectDLLButton.Enabled = true;

                    injectionStatusLabel.ForeColor = Color.Yellow;
                    injectionStatusLabel.Text = "Pending!";
                }
                catch (Exception)
                {
                    injectDLLButton.Enabled = false;
                    XtraMessageBox.Show("An unexpected error has occured! Please try again.", "Unexpected error!", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    selectedDLLLabel.ForeColor = Color.Red;
                    selectedDLLLabel.Text = "Error!";
                }
            }
        }

        private void injectDLLButton_Click(object sender, EventArgs e)
        {
            injectionStatusLabel.ForeColor = Color.Cyan;
            injectionStatusLabel.Text = "Injecting...";
            try
            {
                if (listBoxControl2.SelectedItem != "")
                {
                    if (selectedDLL != null)
                    {
                        string _new = listBoxControl2.SelectedItem.ToString().Substring(0, listBoxControl2.SelectedItem.ToString().Length - 4);
                        _module.InjectDLL(_new, selectedDLL);

                        injectionStatusLabel.ForeColor = Color.Green;
                        injectionStatusLabel.Text = "Injected";
                    }
                    else
                    {
                        XtraMessageBox.Show("Please Select A Dll!", "Error: No Dll Selected", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);


                    }
                }
                else
                {
                    XtraMessageBox.Show("Please Select A Process From The List", "Error: No Process Selected", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);


                }
            }
            catch
            {
                XtraMessageBox.Show("An unexpected error has occured! Please try again.", "Unexpected error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                injectionStatusLabel.ForeColor = Color.Red;
                injectionStatusLabel.Text = "Failed!";
            }
        }

        private void listBoxControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedProcessLabel.Text = $"Selected Process: {listBoxControl2.SelectedItem}";
        }

        private void refreshProcButton_Click(object sender, EventArgs e)
        {
            GetProcesses();
            XtraMessageBox.Show("Processes refreshed!", toolName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
