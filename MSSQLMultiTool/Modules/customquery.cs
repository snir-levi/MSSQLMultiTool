using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using MSSQLMultiTool.outputsLib;

namespace MSSQLMultiTool.Modules
{
    class customquery
    {
        public static void help()
        {
            Console.WriteLine("--module customquery\n");
            Console.WriteLine("     Arguments:\n");
            Console.WriteLine("     --query <query>\n");
        }
        static void checkArguments(Dictionary<string, string> arguments)
        {
            try
            {
                if (!arguments.ContainsKey("query"))
                {
                    outputs.printError("Missing Custom Query Argument \n");
                    help();
                    Environment.Exit(0);
                }
            }
            catch (Exception e)
            {
                outputs.printError("Missing Custom Query Argument \n");
                help();
                Environment.Exit(0);
            }
        }
        public static void run(Dictionary<string, string> arguments, SqlConnection con)
        {
            outputs.printHeader("Loading Custom Query Module");
            checkArguments(arguments);
            outputs.printOutput($"Query to execute: {arguments["query"]}\n");
            String res;
            res = SQLModule.executeQuery(arguments["query"],con);
            outputs.printSuccess("Executed Query");
            outputs.printSuccess(res);
        }
    }
}
