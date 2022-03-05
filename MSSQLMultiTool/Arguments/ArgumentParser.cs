using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MSSQLMultiTool.Arguments
{
    class ArgumentsParser
    {
        public static Dictionary<string, string> Parse(string[] args)
        {

            var arguments = new Dictionary<string, string>()
            {
                ["instance"] = null,
                ["module"] = null,
                ["target"] = null,
                ["impersonate-login"] = null,
                ["impersonate-user"] = null,
                ["db"] = null,
                ["attackerbox"] = null,
                ["check-group-membership"] = null,
                ["check-impersonation-login"] = null,
                ["check-impersonation-user"] = null,
                ["check-trust-worthy"] = null,
                ["set-trust-worthy"] = null,
                ["run"] = null,
                ["command"] = null,
                ["query"] = null,
            };

            var switches = new Dictionary<string, bool>()
            {
                ["enable-rpc-out"] = false,
            };

        var parsedArguments = new Dictionary<string, string>();

            try
            {
                foreach (string arg in arguments.Keys)
                {
                    int index = 0;
                    index = Array.IndexOf(args, "--"+arg);
                    if (index != -1)
                        parsedArguments[arg] = args[index + 1];
                }

                foreach (string switchVar in switches.Keys)
                {
                    int index = 0;
                    index = Array.IndexOf(args, "--"+switchVar);
                    if (index != -1)
                        parsedArguments[switchVar] = "";
                }

                return parsedArguments;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
