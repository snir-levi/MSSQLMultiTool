using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSSQLMultiTool.outputsLib
{
    class outputs
    {
        public static void printOutput(string output)
        {
            Console.WriteLine("[*] " + output);
        }

        public static void printSuccess(string output)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[+] " + output);
            Console.ForegroundColor = ConsoleColor.White;
        }
        public static void printHeader(string output)
        {
            Console.WriteLine("\n *** " + output + " *** \n");
        }
        public static void printError(string output)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[-] " + output);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
