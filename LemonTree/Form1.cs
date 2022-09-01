using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using FastColoredTextBoxNS;

namespace LemonTree
{
    public partial class Form1 : Form
    {
        Style GreenStyle = new TextStyle(Brushes.Green, null, FontStyle.Italic);
        Style FunctionStyle = new TextStyle(Brushes.Yellow, null, FontStyle.Bold);
        Style EntrypointStyle = new TextStyle(Brushes.Aqua, null, FontStyle.Underline);
        Style EndStyle = new TextStyle(Brushes.Red, null, FontStyle.Underline);
        Style IncludeStyle = new TextStyle(Brushes.Fuchsia, null, FontStyle.Underline);

        AutocompleteMenu popupMenu;

        List<string> autocompleteItems = new();
        List<string> functionnames = new();

        string selectedFolder = "";
        string selectedFile = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SetupAutocomplete(autocompleteItems);
        }

        void SetupAutocomplete(List<string> functionNames)
        {
            functionNames.AddRange(new List<string>()
            {
                "include",
                "entrypoint",
                "end"
            });
            popupMenu = new AutocompleteMenu(fastColoredTextBox1);
            popupMenu.MinFragmentLength = 2;
            popupMenu.Items.SetAutocompleteItems(functionNames);
            popupMenu.Items.MaximumSize = new Size(200, 300);
            popupMenu.Items.Width = 200;
            popupMenu.HoveredColor = Color.FromArgb(14, 99, 156);
            popupMenu.SelectedColor = Color.FromArgb(34, 119, 176);
            popupMenu.ForeColor = Color.White;
            popupMenu.BackColor = BackColor;
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
            btn.Click += (object sender, EventArgs a) =>
            {
                FolderBrowserDialog e = new FolderBrowserDialog();
                e.SelectedPath = "C:\\";
                e.ShowNewFolderButton = false;
                e.ShowDialog();
                if (string.IsNullOrWhiteSpace(e.SelectedPath))
                    return;
                if (!System.IO.Directory.Exists(e.SelectedPath))
                    return;

                Text = e.SelectedPath + " - Lemon Tree";
                selectedFolder = e.SelectedPath;
                controlList1.Clear();
                string ff = "";
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
                    btn.Click += (object sender, EventArgs a) =>
                    {
                        loadFile(file);
                    };
                    btn.Size = new Size(controlList1.Width, 70);
                    btn.FlatAppearance.BorderSize = 0;
                    btn.FlatStyle = FlatStyle.Flat;
                    controlList1.Add(btn, new Point(15, 0));
                }
                loadFile(ff);
            };
            controlList1.Add(btn, new Point(15, 0));
        }

        void loadFile(string file)
        {
            Text = file + " - Lemon Tree";
            selectedFile = file;
            fastColoredTextBox1.Text = "";
            fastColoredTextBox1.InsertText(string.Join("\n", System.IO.File.ReadAllLines(file)));
            fastColoredTextBox1_TextChanged(null, new TextChangedEventArgs(fastColoredTextBox1.Range));
        }

        private void fastColoredTextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            e.ChangedRange.ClearStyle(GreenStyle, FunctionStyle, EntrypointStyle, EndStyle, IncludeStyle);
            e.ChangedRange.SetStyle(GreenStyle, @"//", RegexOptions.Multiline);
            e.ChangedRange.SetStyle(FunctionStyle, string.Join(" ", functionnames) , RegexOptions.Multiline);
            e.ChangedRange.SetStyle(EntrypointStyle, @"entrypoint", RegexOptions.Multiline);
            e.ChangedRange.SetStyle(EndStyle, @"end", RegexOptions.Multiline);
            e.ChangedRange.SetStyle(IncludeStyle, @"include", RegexOptions.Multiline);

            autocompleteItems.Clear();
            functionnames.Clear();
            foreach (string line in fastColoredTextBox1.Lines)
            {
                if(line.StartsWith("include"))
                {
                    string line1 = line + " ";
                    ILemonLib lib = ILemonLib.loadlib(line1.Split(" ")[1]);
                    autocompleteItems.AddRange(lib.functions);
                    functionnames.AddRange(lib.functions);
                    SetupAutocomplete(autocompleteItems);
                }
            }
        }



        private void fastColoredTextBox1_Load(object sender, EventArgs e)
        {
        }

        private void fastColoredTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == (Keys.Space | Keys.Control))
            {
                if (popupMenu == null)
                {
                    e.Handled = true;
                    return;
                }

                popupMenu.Show(true);
                e.Handled = true;
            }
        }

        public class ILemonLib
        {
            public List<string> functions;

            public static ILemonLib loadlib(string libname)
            {
                ILemonLib lib = new ILemonLib();
                lib.functions = new();

                if (!System.IO.File.Exists(Settings.LemonInstallationPath + "\\libs\\" + libname))
                    return lib;

                foreach (string line in System.IO.File.ReadAllLines(Settings.LemonInstallationPath + "\\libs\\" + libname))
                {
                    if(line.StartsWith("CPP_MAP:"))
                    {
                        lib.functions.Add(line.Substring(8).Split(";VALUE=")[0]);
                    }
                }

                return lib;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
        }

        // All of the controls
        private void fastColoredTextBox1_KeyDown_1(object sender, KeyEventArgs e)
        {
            if(e.KeyData == (Keys.S | Keys.Control))
            {
                System.IO.File.WriteAllLines(selectedFile, fastColoredTextBox1.Lines);
            }
            if (e.KeyData == (Keys.Z | Keys.Control))
            {
                fastColoredTextBox1.Undo();
            }
            if (e.KeyData == (Keys.U | Keys.Control))
            {
                fastColoredTextBox1.Redo();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] batchLines = new string[]
            {
                "@echo off",
                "lemon %~dp0 a.cpp -op",
                "g++ a.cpp",
                "del a.cpp",
                "exit"
            };
            string[] ba2 = new string[]
            {
                "cd \"" + Settings.LemonInstallationPath + "\"",
                "compile.cmd"
            };
            System.IO.File.WriteAllLines(Settings.LemonInstallationPath + "\\compile.cmd", batchLines);
            System.IO.File.WriteAllLines("comp.cmd", ba2);
            new Process() { StartInfo = new ProcessStartInfo() { FileName = "comp.cmd", Domain = AppDomain.CurrentDomain.BaseDirectory, WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory, UseShellExecute = true } }.Start();
            Clipboard.SetText(AppDomain.CurrentDomain.BaseDirectory);
            new Process() { StartInfo = new ProcessStartInfo() { FileName = "explorer", Arguments = "\"" + selectedFolder + "\"" } }.Start();
        }
    }
}
