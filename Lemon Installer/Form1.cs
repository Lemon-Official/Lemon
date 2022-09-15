using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lemon_Installer
{
    public struct InstallerPage
    {
        public string header; 
        public string text;
        public string nextButtonText;
        public EventHandler load;
    }

    public partial class Form1 : Form
    {
        InstallerPage[] pages = new InstallerPage[]
        {
            new()
            {
                header = "Welcome to Lemon",
                text = "This Setup will guide you through the installation process.",
                nextButtonText = "Next"
            },
            new()
            {
                header = "License",
                text = "Lemon Installer, LemonTree and Lemon is licensed under CC BY-NC-ND 4.0.\nNone of the code by Lemon Official is allowed to be reused commercialy.\nIf you copy the code and change it, you may not use it commercialy.",
                nextButtonText = "Accept"
            },
            new()
            {
                header = "Last Step",
                text = "Click \"Install\" to start the installation.\nThere will be a folder created under \"C:\\LemonLibs\" do not delete this folder.",
                nextButtonText = "Install"
            },
            new()
            {
                header = "Installing...",
                text = "Please wait while were installing Lemon...",
                nextButtonText = "Installing...",
                load = (object sender, EventArgs a) =>
                {
                    Form1 f = (Form1)sender;
                    f.button1.Enabled = false;
                    int stepAt = 0;
                    int downloadPRG = 0;

                    #region Methods

                    void downloadResource(string url, string displayName, string file, bool doUnzipping = false)
                    {
                        using(var wc = new WebClient())
                        {
                            Application.DoEvents();
                            Uri.TryCreate(url, UriKind.Absolute, out Uri uri);
                            wc.DownloadProgressChanged += (object sender, DownloadProgressChangedEventArgs e) =>
                            {
                                downloadPRG++;

                                if(downloadPRG == 100)
                                    try
                                    {
                                        f.info2.Text = "Downloading " + displayName + " [" + e.ProgressPercentage + "% / " + e.BytesReceived / 1000 + "KB from " + e.TotalBytesToReceive / 1000 + "KB]";
                                        Application.DoEvents();
                                        downloadPRG = 0;
                                    }catch(StackOverflowException) {}
                            };
                            wc.DownloadFileCompleted += (object sender, AsyncCompletedEventArgs e) =>
                            {
                                stepAt++;
                                Application.DoEvents();
                                downloadPRG = 0;
                                return;
                            };
                            wc.DownloadFileAsync(uri, file);
                        }
                    }

                    void unzipResource(string source, string dest)
                    {
                        Application.DoEvents();
                        System.IO.Compression.ZipFile.ExtractToDirectory(source, dest);
                        Application.DoEvents();
                    }

                    void unzipResourceM(string source, string dest)
                    {
                        Application.DoEvents();
                        using (ZipFile zip = ZipFile.Read(source))
                        {
                            Application.DoEvents();
                            zip.ExtractAll(dest);
                        }
                        Application.DoEvents();
                    }

                    #endregion

                    downloadResource("https://github.com/Lemon-Official/LemonInstaller/raw/main/lemon.zip", "Lemon Compiler", AppDomain.CurrentDomain.BaseDirectory + "\\lemon.zip");
                    Application.DoEvents();
                    downloadResource("https://github.com/Lemon-Official/LemonInstaller/raw/main/lemontree.zip", "LemonTree IDE for Lemon (Part 1)", AppDomain.CurrentDomain.BaseDirectory + "\\lemontree.zip");
                    Application.DoEvents();
                    downloadResource("https://github.com/Lemon-Official/LemonInstaller/raw/main/lemontree.z01", "LemonTree IDE for Lemon (Part 2)", AppDomain.CurrentDomain.BaseDirectory + "\\lemontree.z01");
                    Application.DoEvents();
                    downloadResource("https://github.com/Lemon-Official/LemonInstaller/raw/main/lemontree.z02", "LemonTree IDE for Lemon (Part 3)", AppDomain.CurrentDomain.BaseDirectory + "\\lemontree.z02");
                    Application.DoEvents();
                    downloadResource("https://github.com/Lemon-Official/LemonInstaller/raw/main/lemontree.z03", "LemonTree IDE for Lemon (Part 4)", AppDomain.CurrentDomain.BaseDirectory + "\\lemontree.z03");
                    Application.DoEvents();
                    downloadResource("https://github.com/Lemon-Official/LemonInstaller/raw/main/lemontree.z04", "LemonTree IDE for Lemon (Part 5)", AppDomain.CurrentDomain.BaseDirectory + "\\lemontree.z04");
                    Application.DoEvents();
                    downloadResource("https://github.com/Lemon-Official/LemonInstaller/raw/main/lemontree.z05", "LemonTree IDE for Lemon (Part 6)", AppDomain.CurrentDomain.BaseDirectory + "\\lemontree.z05");
                    Application.DoEvents();
                    downloadResource("https://github.com/Lemon-Official/LemonInstaller/raw/main/lemontree.z06", "LemonTree IDE for Lemon (Part 7)", AppDomain.CurrentDomain.BaseDirectory + "\\lemontree.z06");
                    Application.DoEvents();
                    downloadResource("https://github.com/Lemon-Official/LemonInstaller/raw/main/lemontree.z07", "LemonTree IDE for Lemon (Part 8)", AppDomain.CurrentDomain.BaseDirectory + "\\lemontree.z07");
                    Application.DoEvents();
                    downloadResource("https://github.com/Lemon-Official/LemonInstaller/raw/main/lemontree.z08", "LemonTree IDE for Lemon (Part 9)", AppDomain.CurrentDomain.BaseDirectory + "\\lemontree.z08");
                    Application.DoEvents();
                    downloadResource("https://github.com/Lemon-Official/LemonInstaller/raw/main/lemontree.z09", "LemonTree IDE for Lemon (Part 10)", AppDomain.CurrentDomain.BaseDirectory + "\\lemontree.z09");
                    Application.DoEvents();
                    downloadResource("https://github.com/Lemon-Official/LemonInstaller/raw/main/lemontree.z10", "LemonTree IDE for Lemon (Part 11)", AppDomain.CurrentDomain.BaseDirectory + "\\lemontree.z10");
                    Application.DoEvents();
                    downloadResource("https://github.com/Lemon-Official/LemonInstaller/raw/main/lemontree.z11", "LemonTree IDE for Lemon (Part 12)", AppDomain.CurrentDomain.BaseDirectory + "\\lemontree.z11");
                    Application.DoEvents();
                    downloadResource("https://github.com/Lemon-Official/LemonInstaller/raw/main/lemontree.z12", "LemonTree IDE for Lemon (Part 13)", AppDomain.CurrentDomain.BaseDirectory + "\\lemontree.z12");
                    Application.DoEvents();
                    downloadResource("https://github.com/Lemon-Official/LemonInstaller/raw/main/lemontree.z13", "LemonTree IDE for Lemon (Part 14)", AppDomain.CurrentDomain.BaseDirectory + "\\lemontree.z13");
                    Application.DoEvents();
                    downloadResource("https://github.com/Lemon-Official/LemonInstaller/raw/main/lemontree.z14", "LemonTree IDE for Lemon (Part 15)", AppDomain.CurrentDomain.BaseDirectory + "\\lemontree.z14");
                    Application.DoEvents();
                    downloadResource("https://github.com/Lemon-Official/LemonInstaller/raw/main/lemontree.z15", "LemonTree IDE for Lemon (Part 16)", AppDomain.CurrentDomain.BaseDirectory + "\\lemontree.z15");
                    Application.DoEvents();
                    downloadResource("https://github.com/Lemon-Official/LemonInstaller/raw/main/lemontree.z16", "LemonTree IDE for Lemon (Part 17)", AppDomain.CurrentDomain.BaseDirectory + "\\lemontree.z16");
                    Application.DoEvents();
                    downloadResource("https://github.com/Lemon-Official/LemonInstaller/raw/main/lemontree.z17", "LemonTree IDE for Lemon (Part 18)", AppDomain.CurrentDomain.BaseDirectory + "\\lemontree.z17");
                    Application.DoEvents();
                    downloadResource("https://github.com/Lemon-Official/LemonInstaller/raw/main/lemontree.z18", "LemonTree IDE for Lemon (Part 19)", AppDomain.CurrentDomain.BaseDirectory + "\\lemontree.z18");
                    Application.DoEvents();
                    downloadResource("https://github.com/Lemon-Official/LemonInstaller/raw/main/lemontree.z19", "LemonTree IDE for Lemon (Part 20)", AppDomain.CurrentDomain.BaseDirectory + "\\lemontree.z19", true);
                    Application.DoEvents();

                    while(stepAt != 21)
                    {
                        Application.DoEvents();
                        Thread.Sleep(1);
                    }

                    if(Directory.Exists("C:\\Program Files\\Lemon"))
                        Directory.Delete("C:\\Program Files\\Lemon", true);

                    Directory.CreateDirectory("C:\\Program Files\\Lemon");

                    f.info2.Text = "Unzipping...";

                    unzipResourceM(AppDomain.CurrentDomain.BaseDirectory + "\\lemontree.zip", "C:\\Program Files\\Lemon\\LemonTree");
                    unzipResource(AppDomain.CurrentDomain.BaseDirectory + "\\lemon.zip", "C:\\Program Files\\Lemon\\Lemon");

                    var name = "PATH";
                    var scope = EnvironmentVariableTarget.Machine;
                    var oldValue = Environment.GetEnvironmentVariable(name, scope);
                    var newValue  = oldValue + @";C:\Program Files\Lemon\Lemon";
                    Environment.SetEnvironmentVariable(name, newValue, scope);

                    f.nextPage();
                }
            },
            new()
            {
                header = "Cleaning Files...",
                text = "Please wait while were cleaning up temporary files...",
                load = (object sender, EventArgs e) =>
                {
                    Form1 f = (Form1)sender;
                    f.button1.Enabled = false;
                    f.info2.Text = "Deleting lemontree.z19...";
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\lemontree.z19");
                    f.info2.Text = "Deleting lemontree.z18...";
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\lemontree.z18");
                    f.info2.Text = "Deleting lemontree.z17...";
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\lemontree.z17");
                    f.info2.Text = "Deleting lemontree.z16...";
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\lemontree.z16");
                    f.info2.Text = "Deleting lemontree.z15...";
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\lemontree.z15");
                    f.info2.Text = "Deleting lemontree.z14...";
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\lemontree.z14");
                    f.info2.Text = "Deleting lemontree.z13...";
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\lemontree.z13");
                    f.info2.Text = "Deleting lemontree.z12...";
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\lemontree.z12");
                    f.info2.Text = "Deleting lemontree.z11...";
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\lemontree.z11");
                    f.info2.Text = "Deleting lemontree.z10...";
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\lemontree.z10");
                    f.info2.Text = "Deleting lemontree.z9...";
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\lemontree.z09");
                    f.info2.Text = "Deleting lemontree.z8...";
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\lemontree.z08");
                    f.info2.Text = "Deleting lemontree.z7...";
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\lemontree.z07");
                    f.info2.Text = "Deleting lemontree.z6...";
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\lemontree.z06");
                    f.info2.Text = "Deleting lemontree.z5...";
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\lemontree.z05");
                    f.info2.Text = "Deleting lemontree.z4...";
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\lemontree.z04");
                    f.info2.Text = "Deleting lemontree.z3...";
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\lemontree.z03");
                    f.info2.Text = "Deleting lemontree.z2...";
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\lemontree.z02");
                    f.info2.Text = "Deleting lemontree.z1...";
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\lemontree.z01");
                    f.info2.Text = "Deleting lemontree.zip...";
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\lemontree.zip");
                    f.info2.Text = "Deleting lemon.zip...";
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\lemon.zip");
                    f.info2.Text = "Finished!";
                    f.button1.Enabled = true;
                    f.button1.Text = "Next";
                },
                nextButtonText = "Cleaning..."
            },
            new()
            {
                header = "Finished!",
                text = "Thank you for installing lemon!\nThe Lemon Compiler will be accessable in the command prompt with \"lemon\"!",
                nextButtonText = "Close"
            },
            new()
            {
                header = "",
                text = "",
                nextButtonText = "",
                load = (object sender, EventArgs a) =>
                {
                    ((Form1)sender).Hide();
                    if(MessageBox.Show("Your Machine needs to be restarted, do you want to restart now?", "Lemon Installer", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        System.Diagnostics.Process.Start("shutdown.exe", "-r -t 0");
                }
            }
        };

        int pageIndex = -1;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            nextPage();
        }

        void nextPage()
        {
            pageIndex++;

            if (pageIndex >= pages.Length)
                Environment.Exit(0);

            header.Text = pages[pageIndex].header;
            info2.Text = pages[pageIndex].text;
            button1.Text = pages[pageIndex].nextButtonText;

            if (pages[pageIndex].load != null)
                pages[pageIndex].load.Invoke(this, new());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            nextPage();
        }
    }
}
