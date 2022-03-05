using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using MSSQLMultiTool.outputsLib;


namespace MSSQLMultiTool.Modules
{
    class enumeration
    {
        public static void whoCanImpersonate(SqlConnection con)
        {
            String res = SQLModule.executeQuery("SELECT distinct b.name FROM sys.server_permissions a INNER JOIN sys.server_principals b ON a.grantor_principal_id = b.principal_id WHERE a.permission_name = 'IMPERSONATE'; ", con);
            outputs.printOutput($"User can impersonate the following logins: {res}");
        }

        public static void checkImpersonationLogin(string impersonate_to, SqlConnection con)
        {
            outputs.printOutput($"Impersonating Login: {impersonate_to}");
            SQLModule.executeQuery($"EXECUTE AS LOGIN = '{impersonate_to}';", con);
            getLoginInformation(con);
        }

        public static void checkImpersonationUser(string impersonate_to, SqlConnection con)
        {
            outputs.printOutput($"Impersonating User: {impersonate_to}");
            SQLModule.executeQuery($"EXECUTE AS USER = '{impersonate_to}';", con);
            getLoginInformation(con);
        }
        public static void getLoginInformation(SqlConnection con)
        {
            string su, un;
            su = SQLModule.executeQuery("SELECT SYSTEM_USER;", con);
            un = SQLModule.executeQuery("SELECT USER_NAME();", con);
            outputs.printOutput($"Current database login is '{su}' with system user '{un}'.");
        }
        public static void getGroupMembership(String groupToCheck, SqlConnection con)
        {
            String res = SQLModule.executeQuery($"SELECT IS_SRVROLEMEMBER('{groupToCheck}');", con);
            int role = int.Parse(res);
            if (role == 1)
            {
                Console.WriteLine($"[+] User is a member of the '{groupToCheck}' group.");
            }
            else
            {
                Console.WriteLine($"[-] User is not a member of the '{groupToCheck}' group.");
            }
        }

        public static void availableDatabases(SqlConnection con)
        {
            String res = SQLModule.executeQuery("SELECT * from master..sysdatabases",con);
            outputs.printOutput($"Available Databases: \n{res}");
        }

        public static void checkTrustWorthy(SqlConnection con)
        {
            String res = SQLModule.executeQuery($"select name,is_trustworthy_on from sys.databases where is_trustworthy_on = 1", con);
            outputs.printOutput($"The following databases have Trust Worthy setting as ON: \n {res}");
        }

        public static void checkTrustWorthy(SqlConnection con, string db)
        {
            String res = SQLModule.executeQuery($"select is_trustworthy_on from sys.databases where name = '{db}'",con);
            outputs.printOutput($"TrustWorthy for DB: {db} is: {res}");
        }

        public static void setTrustWorthy(SqlConnection con, string db)
        {
            String res = SQLModule.executeQuery($"ALTER DATABASE [{db}] SET TRUSTWORTHY ON", con);
            checkTrustWorthy(con, db);
        }

        public static void checkSysAdmins(SqlConnection con)
        {
            String res = SQLModule.executeQuery("SELECT 'Name' = sp.NAME ,sp.is_disabled AS [Is_disabled] FROM sys.server_role_members rm ,sys.server_principals sp WHERE rm.role_principal_id = SUSER_ID('Sysadmin') AND rm.member_principal_id = sp.principal_id",con);
            outputs.printOutput($"The Following logins are SysAdmins: \n{res}");
        }
        public static void help()
        {
            Console.WriteLine("--module enumeration\n");
            Console.WriteLine("     Options: \n");
            Console.WriteLine("     --check-group-membership <group>\n");
            Console.WriteLine("     --check-impersonation-login <impersonate_to>\n");
            Console.WriteLine("     --check-impersonation-user <impersonate_to>\n");
            Console.WriteLine("     --check-trust-worthy <db>\n");
            Console.WriteLine("     --set-trust-worthy <db>\n");
        }
        public static void run(Dictionary<string, string> arguments, SqlConnection con)
        {
            outputs.printHeader("Loading Enumeration Module");

            try
            {
                if (arguments.ContainsKey("help"))
                {
                    getGroupMembership(arguments["check-group-membership"], con);
                }
            }
            catch (Exception e)
            {
            }

            String login = SQLModule.executeQuery("SELECT SYSTEM_USER;", con);
            outputs.printOutput($"Logged in as: {login}");
            String uname = SQLModule.executeQuery("SELECT USER_NAME();", con);
            outputs.printOutput($"Database username: {uname}");
            getGroupMembership("public", con);
            getGroupMembership("sysadmin", con);
            whoCanImpersonate(con);
            availableDatabases(con);
            Links.enumerateLinkedServers(con);
            checkTrustWorthy(con);
            checkSysAdmins(con);

            try
            {
                if (arguments.ContainsKey("check-group-membership"))
                {
                    getGroupMembership(arguments["check-group-membership"],con);
                }
            }
            catch (Exception e)
            {
                return;
            }

            try
            {
                if (arguments.ContainsKey("check-impersonation-login"))
                {
                    checkImpersonationLogin(arguments["check-impersonation-login"], con);
                }
            }
            catch (Exception e)
            {
                outputs.printError(e.Message);
            }

            try
            {
                if (arguments.ContainsKey("check-impersonation-user"))
                {
                    checkImpersonationUser(arguments["check-impersonation-user"], con);
                }
            }
            catch (Exception e)
            {
                outputs.printError(e.Message);
            }

            try
            {
                if (arguments.ContainsKey("check-trust-worthy"))
                {
                    checkTrustWorthy(con,arguments["check-trust-worthy"]);
                }
            }
            catch (Exception e)
            {
                outputs.printError(e.Message);
            }

            try
            {
                if (arguments.ContainsKey("set-trust-worthy"))
                {
                    setTrustWorthy(con, arguments["set-trust-worthy"]);
                }
            }
            catch (Exception e)
            {
                outputs.printError(e.Message);
            }
        }
    }
}
