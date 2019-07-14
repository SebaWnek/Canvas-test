using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using System.Data.SqlClient;
using System.Globalization;

namespace Canvas_test
{
    public class sqlLogger
    {
        public bool isConnected = false;
        string username = "wneku";
        string database = "SimpleTanks";
        string table = "shotLog";
        string server = "87.207.148.101";
        SqlConnection connection;
        SqlCommand command;
        string cmd;
        NumberFormatInfo nfi = new NumberFormatInfo();
        double testRatio = 0.25;
        

        public sqlLogger(SecureString pass)
        {
            try
            {
                pass.MakeReadOnly();
                SqlCredential credentials = new SqlCredential(username, pass);
                connection = new SqlConnection();
                connection.ConnectionString = $@"Data source={server}; database={database}";
                connection.Credential = credentials;
                connection.Open();
                command = new SqlCommand("", connection);
                isConnected = true;
                nfi.NumberDecimalSeparator = ".";
            }
            catch(Exception)
            {
                isConnected = false;
            }
}

        public void LogShot(double[] data)
        {
            if (isConnected && (Math.Abs(data[1] - data[3]) / (data[2] > data[0] ? data[2] : data[0])) <= testRatio)
            {
                cmd = $@"Insert into {table} values({data[0].ToString(nfi)}, {data[1].ToString(nfi)}, {data[2].ToString(nfi)}, " +
                                                 $@"{data[3].ToString(nfi)}, {data[4].ToString(nfi)}, {data[5].ToString(nfi)}, {data[6].ToString(nfi)})";
                command.CommandText = cmd;
                command.ExecuteNonQuery(); 
            }
        }

        public void EndConnection()
        {
            if (isConnected)
            {
                connection.Close(); 
            }
        }

    }
}
