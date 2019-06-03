//#define INSTALL_IN_PROGRAM_FILES
// Uncomment the line above to install to Program Files (this requires running the installer as admin).

using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormInst
{
    public partial class Form1 : Form
    {
        // To change the files to install (ZIP) or the EULA, change the InstallFiles or License resources respectively.

        const string productName = "My Product";    // The product's name.
        const string companyName = "Software";      // The company's name.
        bool is32Bit = true;                        // Is the product 32-bit?
        bool useSDInstaller = true;                 // Use a Setup and Deployment setup.exe?
        
        // Do not change anything below this line.
        const string introText = "This wizard will install {R} on your computer.";
        const string nameText = "{R} Installer";
        const string instText = "Now installing {R}.";
        const string doneText = "Successfully installed {R}.";

        public Label label3 = new System.Windows.Forms.Label();
        public Panel panel1 = new System.Windows.Forms.Panel();
        public Button button2 = new System.Windows.Forms.Button();
        public ProgressBar progressBar1 = new System.Windows.Forms.ProgressBar();

        #region 64-Bit Check
        static bool is64BitProcess = (IntPtr.Size == 8);
        static bool is64BitOperatingSystem = is64BitProcess || InternalCheckIsWow64();

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWow64Process(
            [In] IntPtr hProcess,
            [Out] out bool wow64Process
        );

        public static bool InternalCheckIsWow64()
        {
            if ((Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor >= 1) ||
                Environment.OSVersion.Version.Major >= 6)
            {
                using (Process p = Process.GetCurrentProcess())
                {
                    bool retVal;
                    if (!IsWow64Process(p.Handle, out retVal))
                    {
                        return false;
                    }
                    return retVal;
                }
            }
            else
            {
                return false;
            }
        }
        #endregion

        private string ProgramFilesPath { 
            get {
                var temp = "";
                if (is64BitOperatingSystem) {
                    if (is32Bit) {
                        // installing x32 software on x64 system
                        temp = Environment.GetEnvironmentVariable("ProgramFiles(x86)");
                    } else {
                        // installing x64 software on x64 system
                        temp = Environment.GetEnvironmentVariable("ProgramFiles");
                    }
                } else {
                    // installing x32 software on x32 system
                    temp = Environment.GetEnvironmentVariable("ProgramFiles");
                }
                return temp;
            }
        }

        public Form1()
        {
            InitializeComponent();
            Text = nameText.Replace("{R}", productName);
            label2.Text = introText.Replace("{R}", productName);
        }

        private void Start(object sender, EventArgs e)
        {
            License();
        }

        void License()
        {
            label1.Text = "LICENSE";
            label2.Text = "Please review the License Agreement.";
            button1.Text = "Yes";
            button1.Click += new System.EventHandler(this.InstCall);
            // below is code that adds a panel, a label (nested in the panel) and a button
            // yes, 25 lines just to do that
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = Properties.Resources.License;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Location = new System.Drawing.Point(16, 86);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(612, 319);
            this.panel1.TabIndex = 4;
            this.panel1.Controls.Add(label3);
            this.panel1.AutoScroll = true;
            this.button2.Location = new System.Drawing.Point(459, 415);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "No";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.NotAccepted);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.button2);
        }

        private void NotAccepted(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void InstCall(object sender, EventArgs e)
        {
            Install();
        }

        void Install()
        {
            button1.Visible = false;
            this.button2.Click -= new System.EventHandler(this.NotAccepted);
            this.Controls.Remove(button2);
            this.Controls.Remove(panel1);
            button2.Dispose();
            panel1.Dispose();
            label1.Text = "INSTALL";
            label2.Text = instText.Replace("{R}", productName);
            /*this.progressBar1.Location = new System.Drawing.Point(19, 91);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(598, 26);
            this.progressBar1.TabIndex = 3;
            this.Controls.Add(progressBar1);*/
            Stream str = new MemoryStream(Properties.Resources.InstallFiles);
            using (ZipFile zip = ZipFile.Read(str))
            {
                var uniqueDir = companyName.Replace(" ", "") + @"/" + productName.Replace(" ", "");
                if (!Directory.Exists(Environment.GetEnvironmentVariable("localappdata") + @"/" + uniqueDir))
                {
#if INSTALL_IN_PROGRAM_FILES
                zip.ExtractAll(ProgramFilesPath + @"/" + uniqueDir);
#else
                    zip.ExtractAll(Environment.GetEnvironmentVariable("localappdata") + @"/" + uniqueDir);
#endif
                } else
                {
                    MessageBox.Show("Directory already exists.");
                }
            }
            Exit();
        }

        void Exit()
        {
            label1.Text = "DONE";
            label2.Text = doneText.Replace("{R}", productName);
            button1.Click += new System.EventHandler(this.NotAccepted);
            button1.Click -= new System.EventHandler(this.InstCall);
            button1.Click -= new System.EventHandler(this.InstCall);
            button1.Click -= new System.EventHandler(this.InstCall);
            button1.Text = "Exit";
            button1.Visible = true;
        }
    }
}
