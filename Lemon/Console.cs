using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SC = System.Console;

namespace Lemon
{
    public static class Console
    {
        public static void WriteLine(string text)
        {
            SC.WriteLine(text);
        }

        public static void Write(string text)
        {
            SC.Write(text);
        }

        public static void PrintError(string file, int line, string error)
        {
            SC.ForegroundColor = ConsoleColor.Red;
            WriteLine("Error!");
            SC.ForegroundColor = ConsoleColor.White;
            SC.WriteLine("In File \"" + file + "\", line " + line + "\nError: " + error);
        }

        public static void PrintWarning(string file, int line, string error)
        {
            SC.ForegroundColor = ConsoleColor.Yellow;
            WriteLine("Warning!");
            SC.ForegroundColor = ConsoleColor.White;
            SC.WriteLine("In File \"" + file + "\", line " + line + "\nWarning: " + error);
        }

        public static void PrintDeprecated(string file, int line)
        {
            SC.ForegroundColor = ConsoleColor.Cyan;
            WriteLine("Deprecation Warning!");
            SC.ForegroundColor = ConsoleColor.White;
            SC.WriteLine("In File \"" + file + "\", line " + line);
        }

        public static void PrintCompilerError(string error)
        {
            SC.ForegroundColor = ConsoleColor.Red;
            WriteLine("Compiler Error!");
            SC.ForegroundColor = ConsoleColor.White;
            SC.WriteLine(error);
        }

        public static void WaitKeyPress() { SC.ReadKey(); }
    }
}
