using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Lemon
{
    class Program
    {
        public static string MinGWInstallationPath = "C:\\";

        static void Main(string[] args)
        {
            System.Console.Title = "Lemon";
            if(args.Length == 0)
            {
                Console.PrintCompilerError("No Code Directory specified.");
                Console.WaitKeyPress();
                return;
            }
            
            if(!Directory.Exists(args[0]))
            {
                Console.PrintCompilerError("Parsed Directory does not exist.");
                Console.WaitKeyPress();
                return;
            }

            Stopwatch watch = new Stopwatch();
            watch.Start();

            System.Console.WriteLine("Detecting Files...");

            string[] files = Directory.GetFiles(args[0], "*.lemon", SearchOption.AllDirectories);
            System.Console.WriteLine("Detected: " + string.Join(',', files));
            string outPath = args[0] + "\\" + args[1];
            System.Console.WriteLine(outPath);

            List<string> includes = new List<string>();
            List<string> lines = new List<string>();

            System.Console.WriteLine("Preparing Variables...");

            foreach (string line in files)
            {
                foreach (string lar in File.ReadAllLines(line))
                {
                    if(lar.StartsWith("include"))
                    {
                        if (!includes.Contains(lar))
                            includes.Add(lar);
                    }else
                    {
                        lines.Add(lar);
                    }
                }
            }

            System.Console.WriteLine("Parsing Lemon...");

            LemonParser parser = new LemonParser();
            string[] cpp = parser.Parse(lines, includes, CPPMapper.LoadMap("cpp.map"));

            if(args.ToList().Contains("-op"))
            {
                File.WriteAllLines(outPath, cpp);
                watch.Stop();
                System.Console.WriteLine("Completed in %seconds% seconds!".Replace("%seconds%", "" + watch.Elapsed.TotalSeconds));
                return;
            }

            System.Console.WriteLine("Compiling...");
            CPPCompiler.Compile(cpp, outPath);

            watch.Stop();
            System.Console.WriteLine("Completed in %seconds% seconds!".Replace("%seconds%", "" + watch.Elapsed.TotalSeconds));
        }
    }

    public class CPPCompiler
    {
        public static void Compile(string[] cpp, string outPath)
        {
            string tmpPath = @"C:\Users\" + Environment.UserName + @"\AppData\Local\Temp\lemon-compiler-temporary-c++-output.cpp";
            File.WriteAllLines(tmpPath, cpp);

            Process proc = new Process();
            proc.StartInfo = new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                Arguments = "/C g++ " + tmpPath + " -o" + outPath
            };
            proc.Start();
            proc.OutputDataReceived += (object sender, DataReceivedEventArgs data) =>
            {
                System.Console.WriteLine("[G++ Worker] " + data.Data);
            };
            proc.WaitForExit();
            File.Delete(tmpPath);
            File.Delete(@"C:\Users\" + Environment.UserName + @"\AppData\Local\Temp\lemon-compiler-batch.bat");
        }
    }

    public class LemonParser
    {
        public string[] Parse(List<string> lines, List<string> includes, CPPMapper mapper)
        {
            List<string> asmLines = new();

            asmLines.Add("#include <iostream>");
            asmLines.Add("#include <string>");

            foreach (string include in includes)
            {
                if(include.EndsWith(".clib"))
                {
                    Console.PrintError("NaN", 0, "Tried to include a CLIB File, You will need to include the LLIB file.\n" + include);
                    continue;
                }

                foreach (string str in File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "\\libs\\" + include.Split(" ")[1]))
                {
                    if (str.StartsWith(";")) continue;
                    if(str.StartsWith("CLIB:"))
                    {
                        System.Console.WriteLine("Including CLIB: " + str.Substring(5) + " [" + AppDomain.CurrentDomain.BaseDirectory + "\\libs\\" + str.Substring(5) + "]");
                        foreach (string cline in File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "\\libs\\" + str.Substring(5)))
                        {
                            asmLines.Add(cline);
                        }
                    }
                    if(str.StartsWith("CPP_MAP:"))
                    {
                        CPPMapper.AddToMapper(mapper, str.Substring(8).Split(";VALUE=")[0], str.Substring(8).Split(";VALUE=")[1]);
                    }
                    if (str.StartsWith("INJ:"))
                    {
                        asmLines.Add(str.Substring(4));
                    }
                }
            }

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) ||line.Replace(" ", null).StartsWith("//"))
                    continue;

                string argsLine = line;

                if (argsLine[0] == ' ')
                    argsLine = StringUtil.RemoveWhitespacesUntilFirstNonWhitespace(line);

                List<string> args = argsLine.Split(" ").ToList();

                for (int i = 0; i < args.Count; i++)
                {
                    if (i == 0)
                        continue;

                    int fiosd = 0;
                    int liosd = 0;
                    if(args[i - 1].Contains("\""))
                    {
                        fiosd = args[i - 1].IndexOf('\"');
                        liosd = args[i - 1].LastIndexOf('\"');
                    }

                    if(fiosd == liosd)
                    {
                        args[i - 1] = args[i - 1] + " " + args[i];
                    }
                }

                string mapped = mapper.Map(args[0], args.ToArray(), new ICOData() { File = "NaN", Line = 0 });
                if(mapped == "NaN")
                {
                    asmLines.Add(args[0] + "();");
                    continue;
                }
                asmLines.Add(mapped);
            }

            return asmLines.ToArray();
        }
    }

    public class StringUtil
    {
        public static string RemoveWhitespacesUntilFirstNonWhitespace(string str)
        {
            string s = "";

            int i = 0;
            bool hasReachedEnd = false;
            while(i < str.Length)
            {
                if (hasReachedEnd)
                    s += str[i];

                if (i + 1 < str.Length && str[i + 1] != ' ')
                    hasReachedEnd = true;

                i++;
            }

            return s;
        }
    }

    public class CPPMapper
    {
        private Dictionary<string, string> dc = new();

        public static CPPMapper LoadMap(string file)
        {
            Dictionary<string, string> dic = new();

            foreach (string line in File.ReadAllLines(file))
            {
                if (!line.StartsWith(";"))
                    dic.Add(line.Split(" = ")[0], line.Split(" = ")[1]);
            }

            return new CPPMapper()
            {
                dc = dic
            };
        }

        public static void AddToMapper(CPPMapper mapper, string mapperKey, string mapperValue)
        {
            mapper.dc.Add(mapperKey, mapperValue);
        }

        public string Map(string mapperKey, string[] args, ICOData ico)
        {
            foreach (KeyValuePair<string, string> kvp in dc)
            {
                if (mapperKey.Replace(" ", "").Contains(kvp.Key.Replace(" ", "")))
                {
                    string result = kvp.Value;
                    List<string> ars = new List<string>();
                    ars.AddRange(args);
                    ars.Add("");
                    return result.Replace("args1", ars[1]);
                }

            }

            return "NaN";
        }
    }

    public class ICOData
    {
        public string File;
        public int Line;
    }
}
