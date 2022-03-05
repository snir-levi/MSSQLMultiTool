using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSSQLMultiTool.Arguments;
using MSSQLMultiTool.Modules;
using MSSQLMultiTool.outputsLib;

using System.Data.SqlClient;

namespace MSSQLMultiTool
{

    class Program
    {
        static void checkAndImpersonate(Dictionary<string, string> arguments, SqlConnection con)
        {
            String su, un;
            try
            {
                if (arguments.ContainsKey("impersonate-login") && arguments.ContainsKey("impersonate-user"))
                {
                    outputs.printError("Can't impersonate user and login together");
                }
            }
            catch (Exception e)
            {
                outputs.printError(e.Message);
            }

            try
            {
                if (arguments.ContainsKey("impersonate-login"))
                {
                    outputs.printOutput($"Impersonating Login: {arguments["impersonate-login"]}");
                    SQLModule.executeQuery($"EXECUTE AS LOGIN = '{arguments["impersonate-login"]}';", con);
                    enumeration.getLoginInformation(con);
                    return;
                }
            }
            catch (Exception e)
            {
                outputs.printError(e.Message);
                return;
            }

            try
            {
                if (arguments.ContainsKey("impersonate-user"))
                {
                    outputs.printOutput($"Impersonating User: {arguments["impersonate-user"]}");
                    SQLModule.executeQuery($"EXECUTE AS USER = '{arguments["impersonate-user"]}';", con);
                    enumeration.getLoginInformation(con);
                    return;
                }
            }
            catch (Exception e)
            {
                outputs.printError(e.Message);
                return;
            }

            outputs.printOutput("No impersonation action entered. Continuing as default user");
        }
        static Dictionary<string, string> checkMandatoryArguments(Dictionary<string, string> arguments)
        {
            try
            {
                if (!arguments.ContainsKey("instance"))
                {
                    outputs.printError("Missing Instance \n");
                    help();
                    Environment.Exit(0);
                }
            }
            catch (Exception e)
            {
                outputs.printError("Missing Instance \n");
                help();
                Environment.Exit(0);
            }

            try
            {
                if (!arguments.ContainsKey("db"))
                {
                    arguments["db"] = "msdb";
                }
            }
            catch (Exception e)
            {
                arguments["db"] = "msdb";
            }


            try
            {
                if (!arguments.ContainsKey("module"))
                {
                    outputs.printError("Missing Module \n");
                    help();
                    Environment.Exit(0);
                }
            }
            catch (Exception e)
            {
                outputs.printError("Missing Module \n");
                help();
                Environment.Exit(0);
            }


            return arguments;
        }

        static void help()
        {
            Console.WriteLine("Usage: MSSQLMultiTool --module <module> --instance <instance> \n");
            enumeration.help();
            NtlmCapture.help();
            Links.help();
            commandExecution.help();
            customquery.help();
        }

        static void LoadModule(string module, Dictionary<string,string> arguments, SqlConnection con)
        {
            if (module.Equals("enumeration"))
                enumeration.run(arguments, con);
            else if (module.Equals("execution"))
                commandExecution.run(arguments, con);
            else if (module.Equals("links"))
                Links.run(arguments, con);
            else if (module.Equals("ntlm"))
                NtlmCapture.run(arguments, con);
            else if (module.Equals("customquery"))
                customquery.run(arguments, con);

            else
            {
                outputs.printError("Unkown Module: " + module);
            }
        }
        
        static void Main(string[] args)
        {
            var arguments = ArgumentsParser.Parse(args);
            if (arguments == null)
            {
                help();
                Environment.Exit(0);
            }

            arguments = checkMandatoryArguments(arguments);
            

            outputs.printOutput($"Instance will be: {arguments["instance"]}");
            outputs.printOutput($"Database will be: {arguments["db"]}");
            String serv = arguments["instance"];
            String db = arguments["db"];
            String conStr = $"Server = {serv}; Database = {db}; Integrated Security = True;";
            outputs.printOutput($"Connection String: {conStr}");

            SqlConnection con = null;
            try
            {
                con = SQLModule.CreateSQLConnection(conStr);
            }
            catch(Exception e)
            {
                Environment.Exit(1);
            }
            
            checkAndImpersonate(arguments,con);

            LoadModule(arguments["module"],arguments, con);
        }
    }
}



