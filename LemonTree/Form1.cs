using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using CefSharp.WinForms;
using CefSharp;
using System.Threading;
using System.IO;

namespace LemonTree
{
    public partial class Form1 : Form
    {
        string selectedFolder = "";
        string selectedFile = "";
        string log = "LemonTree Initialized!";

        public MonacoConnecter monaco;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var cefSettings = new CefSettings();
            Cef.Initialize(cefSettings);

            NodeServer.Start("./Node/", Settings.LemonInstallationPath + "\\libs");
            ChromiumWebBrowser browser = new ChromiumWebBrowser("http://localhost:13005/mncp");
            browser.Dock = DockStyle.Fill;
            monacoPanel.Controls.Add(browser);
            monaco = new MonacoConnecter(browser);
            browser.Show();
        }

        private void controlList1_Load(object sender, EventArgs e)
        {
            controlList1.YOffset = 15;
            Label l1 = new Label()
            {
                Text = "No Directory Open!",
                ForeColor = Color.White
            };
            l1.Font = Font = new Font(l1.Font.FontFamily, 15);
            l1.Visible = true;
            l1.TextAlign = ContentAlignment.MiddleCenter;
            l1.Size = new Size(controlList1.Width, 60);
            controlList1.Add(l1, new Point(0, 0));
            Button btn = new Button()
            {
                Text = "Open Directory",
                BackColor = Color.FromArgb(14, 99, 156),
                ForeColor = Color.White
            };
            btn.Size = new Size(controlList1.Width, 70);
            btn.Click += (object sender, EventArgs a) =>
            {
                FolderBrowserDialog e = new FolderBrowserDialog();
                e.SelectedPath = "";
                e.ShowNewFolderButton = false;
                e.ShowDialog();
                if (string.IsNullOrWhiteSpace(e.SelectedPath))
                    return;
                if (!System.IO.Directory.Exists(e.SelectedPath))
                    return;

                try
                {
                    selectedFolder = e.SelectedPath;
                    System.IO.Directory.GetFiles(selectedFolder, "*.lemon", System.IO.SearchOption.AllDirectories);
                    string ff = "";
                    controlList1.Clear();
                    Button btn1 = new Button();
                    btn1.Text = "Create new File";
                    btn1.BackColor = Color.FromArgb(14, 99, 156);
                    btn1.ForeColor = Color.White;
                    btn1.Click += btn1_clickEvent;
                    btn1.FlatStyle = FlatStyle.Flat;
                    btn1.FlatAppearance.BorderSize = 0;
                    btn1.Size = new Size(controlList1.Width, 70);
                    controlList1.Add(btn1, new Point(0, 0));
                    foreach (string file in System.IO.Directory.GetFiles(selectedFolder, "*.lemon", System.IO.SearchOption.AllDirectories))
                    {
                        if (ff == "")
                            ff = file;

                        Button btn = new Button()
                        {
                            Text = System.IO.Path.GetRelativePath(selectedFolder, file),
                            BackColor = Color.FromArgb(14, 99, 156),
                            ForeColor = Color.White
                        };
                        btn.MouseClick += (object sender, MouseEventArgs e) =>
                        {
                            if (e.Button == MouseButtons.Right)
                            {
                                if (MessageBox.Show("Are you sure you want to delete \"" + btn.Text + "\"?", "LemonTree", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                {
                                    System.IO.File.Delete(selectedFolder + "\\" + btn.Text);
                                }
                            }
                        };
                        btn.Click += (object sender, EventArgs a) =>
                        {
                            loadFile(file);
                        };
                        btn.Size = new Size(controlList1.Width, 70);
                        btn.FlatAppearance.BorderSize = 0;
                        btn.FlatStyle = FlatStyle.Flat;
                        controlList1.Add(btn, new Point(15, 0));
                    }
                    if(ff != "")
                        loadFile(ff);
                }
                catch (UnauthorizedAccessException) { MessageBox.Show("Unauthorized.", "LemonTree", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            };
            controlList1.Add(btn, new Point(0, 0));
        }

        void btn1_clickEvent(object sender, EventArgs args)
        {
            string input = Interaction.InputBox("Please enter the new file name:", "Create File", "");
            if (!System.IO.File.Exists(selectedFolder + "\\" + input) && input != "")
            {
                System.IO.File.Create(selectedFolder + "\\" + input);
                controlList1.Clear();
                Button btn1 = new Button();
                btn1.Text = "Create new File";
                btn1.BackColor = Color.FromArgb(14, 99, 156);
                btn1.ForeColor = Color.White;
                btn1.Click += btn1_clickEvent;
                btn1.FlatStyle = FlatStyle.Flat;
                btn1.FlatAppearance.BorderSize = 0;
                btn1.Size = new Size(controlList1.Width, 70);
                controlList1.Add(btn1, new Point(0, 0));
                foreach (string file in System.IO.Directory.GetFiles(selectedFolder, "*.lemon", System.IO.SearchOption.AllDirectories))
                {
                    Button btn = new Button()
                    {
                        Text = System.IO.Path.GetRelativePath(selectedFolder, file),
                        BackColor = Color.FromArgb(14, 99, 156),
                        ForeColor = Color.White
                    };
                    btn.Click += (object sender, EventArgs a) =>
                    {
                        loadFile(file);
                    };
                    btn.Size = new Size(controlList1.Width, 70);
                    btn.FlatAppearance.BorderSize = 0;
                    btn.FlatStyle = FlatStyle.Flat;
                    controlList1.Add(btn, new Point(15, 0));
                }
            }
        }

        void loadFile(string file)
        {
            if (file == selectedFile)
                return;
            Text = file + " - Lemon Tree";
            selectedFile = file;
            monaco.SetLines(File.ReadAllLines(file));
        }

        public class ILemonLib
        {
            public List<string> functions;

            public static ILemonLib loadlib(string libname)
            {
                ILemonLib lib = new ILemonLib();
                lib.functions = new();

                if (!System.IO.File.Exists(Settings.LemonInstallationPath + "\\libs\\" + libname + "\\library.llib"))
                    return lib;

                foreach (string line in System.IO.File.ReadAllLines(Settings.LemonInstallationPath + "\\libs\\" + libname + "\\library.llib"))
                {
                    if(line.StartsWith("CPP_MAP:"))
                    {
                        lib.functions.Add(line.Substring(8).Split(";VALUE=")[0]);
                        Debug.WriteLine("Added to List: " + line.Substring(8).Split(";VALUE=")[0]);
                    }
                }

                return lib;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
        }


        [DllImport("User32.dll")]
        static extern int SetForegroundWindow(IntPtr point);

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;
        [DllImport("User32")]
        private static extern int ShowWindow(int hwnd, int nCmdShow);

        private void button1_Click(object sender, EventArgs e)
        {
            log += "Compilation Started...\n";
            log += "Preparing Compilation...\n";
            Process p = new Process();
            p.StartInfo = new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                WorkingDirectory = Settings.LemonInstallationPath,
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            log += "Start Compilation Services...\n";
            p.Start();
            log += "Running Compilation Services...\n";
            p.StandardInput.WriteLine("@echo off");
            p.StandardInput.WriteLine("cd \"" + Settings.LemonInstallationPath + "\"");
            p.StandardInput.WriteLine("lemon " + selectedFolder + " output.exe");
            p.StandardInput.WriteLine("mov output.exe \"" + selectedFolder + "\\output.exe\"");
            p.StandardInput.WriteLine("exit");
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            if (!string.IsNullOrWhiteSpace(output))
            {
                foreach (string outLine in output.Split("\n"))
                {
                    if (outLine.StartsWith("cd") || outLine.StartsWith("lemon") || outLine.StartsWith("mov") || outLine.StartsWith("exit") || outLine.StartsWith("(c)") || outLine.StartsWith("Microsoft Windows"))
                        continue;
                    else
                        log += "\n" + outLine;

                }
            }
            log += "Compilation Successful!\n";
            Process.Start("explorer", Settings.LemonInstallationPath);
            MessageBox.Show(log);
        }

        private void button2_Click(object sender, EventArgs e)
        {
        }

        private void Form1_ClientSizeChanged(object sender, EventArgs e)
        {
            monacoPanel.Height = ClientSize.Height - monacoPanel.Location.Y;
            monacoPanel.Width = ClientSize.Width - monacoPanel.Location.X;
        }
    }
}
