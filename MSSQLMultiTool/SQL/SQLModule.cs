using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using MSSQLMultiTool.outputsLib;


    class SQLModule
    {
        public static SqlConnection CreateSQLConnection(string conStr)
        {
            SqlConnection con = new SqlConnection(conStr);
            try
            {
                con.Open();
                outputs.printOutput("Authenticated to MSSQL Server!");
                return con;
            }
            catch
            {
                outputs.printError("Authentication failed.");
                Environment.Exit(0);
                return null;
            }
        }

    public static String executeQuery(String query, SqlConnection con)
    {
        {
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataReader reader = cmd.ExecuteReader();
            try
            {
                String result = "";
                while (reader.Read() == true)
                {
                    result += reader[0] + "\n";
                }
                reader.Close();
                return result;
            }
            catch (Exception e)
            {
                outputs.printError(e.Message);
                return "";
            }
        }
    }
}

