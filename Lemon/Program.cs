using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Lemon
{
    class Program
    {
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
            string args1 = "";
            string[] cpp = parser.Parse(lines, includes, CPPMapper.LoadMap("cpp.map"), out args1);

            if(args.ToList().Contains("-op"))
            {
                File.WriteAllLines(outPath, cpp);
                watch.Stop();
                System.Console.WriteLine("Completed in %seconds% seconds!".Replace("%seconds%", "" + watch.Elapsed.TotalSeconds));
                return;
            }

            System.Console.WriteLine("Compiling...");
            CPPCompiler.Compile(cpp, outPath, args1);

            watch.Stop();
            System.Console.WriteLine("Completed in %seconds% seconds!".Replace("%seconds%", "" + watch.Elapsed.TotalSeconds));
        }
    }

    public class CPPCompiler
    {
        public static void Compile(string[] cpp, string outPath, string args)
        {
            string tmpPath = @"C:\Users\" + Environment.UserName + @"\AppData\Local\Temp\lemon-compiler-temporary-c++-output.cpp";
            File.WriteAllLines(tmpPath, cpp);

            Process proc = new Process();
            proc.StartInfo = new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                Arguments = "/C g++ " + tmpPath + " -o" + outPath + " " + args
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
        public string[] Parse(List<string> lines, List<string> includes, CPPMapper mapper, out string argsout)
        {
            List<string> asmLines = new();

            asmLines.Add("#include <iostream>");
            asmLines.Add("#include <string>");
            string args1 = "";

            foreach (string include in includes)
            {
                if(include.EndsWith(".clib"))
                {
                    Console.PrintError("NaN", 0, "Tried to include .clib, this include statement will be ignored.\n" + include);
                    continue;
                }

                foreach (string str in File.ReadAllLines("C:\\LemonLibs\\" + (include.Split(" ")[1] + "\\library.llib")))
                {
                    if (str.StartsWith(";")) continue;
                    if(str.StartsWith("CLIB:"))
                    {
                        foreach (string cline in File.ReadAllLines("C:\\LemonLibs\\" + include.Split(" ")[1] + "\\" + str.Substring(5)))
                        {
                            asmLines.Add(cline);
                        }
                    }
                    if (str.StartsWith("CPP_MAP:"))
                    {
                        CPPMapper.AddToMapper(mapper, str.Substring(8).Split(";VALUE=")[0], str.Substring(8).Split(";VALUE=")[1]);
                    }
                    if (str.StartsWith("GCC_ARG:"))
                    {
                        args1 += "" + str.Substring(("GCC_ARG:").Length);
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

                List<string> args = StringUtil.RemoveWhitespacesUntilFirstNonWhitespace(line).Split(" ").ToList();

                bool instring = false;
                for (int i = 0; i < args.Count; i++)
                {
                    if (instring)
                    {
                        if(i >= 1)
                        {
                            args[i - 1] += " " + args[i];
                            args.RemoveAt(i);
                        }
                    }

                    try
                    {
                        if (args[i].FirstOrDefault() == '\"' || args[i].LastOrDefault() == '\"')
                        {
                            if(instring)
                                args[i] += "\"";

                            instring = !instring;
                        }
                    }catch(ArgumentOutOfRangeException) { }
                }

                string mapped = mapper.Map(args[0], args.ToArray(), new ICOData() { File = "NaN", Line = 0 });
                if(mapped == "NaN")
                {
                    asmLines.Add(args[0] + "();");
                    continue;
                }
                asmLines.Add(mapped);
            }

            argsout = args1;
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
                {
                    if(!hasReachedEnd)
                        s += (str[i].ToString().Replace(" ", ""));

                    hasReachedEnd = true;
                }

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
                    List<string> argse1 = ((string[])args.Clone()).ToList();
                    argse1.RemoveAt(0);

                    if(argse1.Count != 0)
                        for (int i = 0; i < argse1.Last().Length; i++)
                        {
                            if (i == 0 | i == 1)
                                continue;

                            if (argse1.Last()[i] == '\"' && argse1.Last()[i - 1] == '\"')
                                argse1[argse1.IndexOf(argse1.Last())] = argse1.Last().Remove(argse1.Last().Length - 1, 1);
                        }

                    result = result.Replace("argse1", String.Join(" ", argse1));
                    switch(argse1.Count)
                    {
                        case 1:
                            result = result.Replace("{1}", args[1]);
                            break;
                        case 2:
                            result = result.Replace("{1}", args[1]);
                            result = result.Replace("{2}", args[2]);
                            break;
                        case 3:
                            result = result.Replace("{1}", args[1]);
                            result = result.Replace("{2}", args[2]);
                            result = result.Replace("{3}", args[3]);
                            break;
                        case 4:
                            result = result.Replace("{1}", args[1]);
                            result = result.Replace("{2}", args[2]);
                            result = result.Replace("{3}", args[3]);
                            result = result.Replace("{4}", args[4]);
                            break;
                        case 5:
                            result = result.Replace("{1}", args[1]);
                            result = result.Replace("{2}", args[2]);
                            result = result.Replace("{3}", args[3]);
                            result = result.Replace("{4}", args[4]);
                            result = result.Replace("{5}", args[5]);
                            break;
                        case 6:
                            result = result.Replace("{1}", args[1]);
                            result = result.Replace("{2}", args[2]);
                            result = result.Replace("{3}", args[3]);
                            result = result.Replace("{4}", args[4]);
                            result = result.Replace("{5}", args[5]);
                            result = result.Replace("{6}", args[6]);
                            break;
                        case 7:
                            result = result.Replace("{1}", args[1]);
                            result = result.Replace("{2}", args[2]);
                            result = result.Replace("{3}", args[3]);
                            result = result.Replace("{4}", args[4]);
                            result = result.Replace("{5}", args[5]);
                            result = result.Replace("{6}", args[6]);
                            result = result.Replace("{7}", args[7]);
                            break;
                        case 8:
                            result = result.Replace("{1}", args[1]);
                            result = result.Replace("{2}", args[2]);
                            result = result.Replace("{3}", args[3]);
                            result = result.Replace("{4}", args[4]);
                            result = result.Replace("{5}", args[5]);
                            result = result.Replace("{6}", args[6]);
                            result = result.Replace("{7}", args[7]);
                            result = result.Replace("{8}", args[8]);
                            break;
                        case 9:
                            result = result.Replace("{1}", args[1]);
                            result = result.Replace("{2}", args[2]);
                            result = result.Replace("{3}", args[3]);
                            result = result.Replace("{4}", args[4]);
                            result = result.Replace("{5}", args[5]);
                            result = result.Replace("{6}", args[6]);
                            result = result.Replace("{7}", args[7]);
                            result = result.Replace("{8}", args[8]);
                            result = result.Replace("{9}", args[9]);
                            break;
                        case 10:
                            result = result.Replace("{1}", args[1]);
                            result = result.Replace("{2}", args[2]);
                            result = result.Replace("{3}", args[3]);
                            result = result.Replace("{4}", args[4]);
                            result = result.Replace("{5}", args[5]);
                            result = result.Replace("{6}", args[6]);
                            result = result.Replace("{7}", args[7]);
                            result = result.Replace("{8}", args[8]);
                            result = result.Replace("{9}", args[9]);
                            result = result.Replace("{10}", args[10]);
                            break;
                    }
                    return result;
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
