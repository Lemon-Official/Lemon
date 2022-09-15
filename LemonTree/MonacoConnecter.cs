using CefSharp.DevTools.Page;
using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LemonTree
{
    public class MonacoConnecter
    {
        public class Monaco
        {
            public struct MonarchEntry
            {
                public string Regex;
                public string Keyword;

                public string[] GetJS()
                {
                    return new string[]
                    {
                        "[/" + Regex + "/, {",
                        "cases: {",
                        "\"@keywords\": \"" + Keyword + "\"",
                        "\"@default\": \"" + Keyword + "\"",
                        "}",
                        "]"
                    };
                }
            }
        }

        ChromiumWebBrowser brwoser;

        public MonacoConnecter(ChromiumWebBrowser brwoser)
        {
            this.brwoser = brwoser;
        }

        public string[] GetIncludes()
        {
            if (brwoser.CanExecuteJavascriptInMainFrame)
            {
                JavascriptResponse response = brwoser.EvaluateScriptAsync("let v = \"\"\nfor(const index in window.incs) v = v + \",\" + window.incs(index)\nconsole.log(v)").Result;
                if (response.Success)
                {
                    return ((string)response.Message).Split(",");
                }
                return new string[0];
            }
            return new string[0];
        }

        public bool ResetMonarch()
        {
            if (brwoser.CanExecuteJavascriptInMainFrame)
            {
                JavascriptResponse response = brwoser.EvaluateScriptAsync("window.monarch = window.dmonarch ").Result;
                if (response.Success)
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        public void AddEntryToMonarch(Monaco.MonarchEntry entry)
        {
            if (brwoser.CanExecuteJavascriptInMainFrame)
            {
                JavascriptResponse response = brwoser.EvaluateScriptAsync("window.monarch = window.dmonarch ").Result;
                if (response.Success)
                {
                }
            }
        }

        public string[] GetLines()
        {
            if (brwoser.CanExecuteJavascriptInMainFrame)
            {
                JavascriptResponse response = brwoser.EvaluateScriptAsync("window.editor.getValue();").Result;
                if (response.Success)
                {
                    return ((string)response.Message).Split("\n");
                }
                return new string[0];
            }
            return new string[0];
        }

        public void SetLines(string[] lines)
        {
            SetLinesAsync(lines);
        }

        public async Task<string[]> GetLinesAsync()
        {
            if (brwoser.CanExecuteJavascriptInMainFrame)
            {
                JavascriptResponse response = await brwoser.EvaluateScriptAsync("window.editor.getValue();");
                if(response.Success)
                {
                    return ((string)response.Message).Split("\n");
                }
                return new string[0];
            }
            return new string[0];
        }
        
        public async Task<bool> SetLinesAsync(string[] lines)
        {
            if(brwoser.CanExecuteJavascriptInMainFrame)
            {
                List<string> usableLines = lines.ToList();
                for (int i = 0; i < usableLines.Count; i++)
                {
                    usableLines[i] = usableLines[i].Replace("\"", "\\\"");
                }
                JavascriptResponse response = await brwoser.EvaluateScriptAsync("window.editor.setValue(\"" + string.Join("\\n", usableLines) + "\");");
                System.Windows.Forms.MessageBox.Show(response.Success + "");
                System.Windows.Forms.MessageBox.Show(response.Result + "");
                Clipboard.SetText("window.editor.setValue(\"" + string.Join("\\n", lines) + "\");");
                if (response.Success)
                {
                    return true;
                }
                return false;
            }
            return false;
        }
    }
}
