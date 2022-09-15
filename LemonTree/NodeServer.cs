using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LemonTree
{
    public static class NodeServer
    {
        public static Process NodeProcess;

        public static Process Start(string path, params string[] linestoparse)
        {
            Process proc = new Process();
            proc.StartInfo = new ProcessStartInfo()
            {
                FileName = "cmd",
                RedirectStandardInput = true,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };
            proc.Start();
            proc.StandardInput.WriteLine("node \"" + path + "index.js\"");
            foreach (string line in linestoparse)
            {
                proc.StandardInput.WriteLine(line);
            }
            Application.ApplicationExit += (object sender, EventArgs a) =>
            {
                proc.Close();
            };
            return proc;
        }
    }
}
