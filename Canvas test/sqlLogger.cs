using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using System.Data.SqlClient;
using System.Globalization;
using System.Windows;
using System.Data;

namespace Canvas_test
{
    public class sqlConnector
    {
        public bool isConnected = false;
        string username = "wneku";
        string database = "SimpleTanks";
        string table = "shotLogEmpty";
        string server = "87.207.148.101";
        SqlConnection connection;
        SqlDataAdapter adapter;
        SqlDataReader reader;
        SqlCommand command;
        string cmd;
        NumberFormatInfo nfi = new NumberFormatInfo();


        public sqlConnector(SecureString pass)
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
            catch (Exception)
            {
                isConnected = false;
            }
        }

        public void LogShot(double[] data)
        {
            if (isConnected)
            {
                cmd = $@"Insert into {table} values({data[0].ToString(nfi)}, {data[1].ToString(nfi)}, {data[2].ToString(nfi)}, " +
                      $@"{data[3].ToString(nfi)}, {data[4].ToString(nfi)}, {data[5].ToString(nfi)}, {data[6].ToString(nfi)}, {data[7]}, {data[8].ToString(nfi)})";
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

        public DataTable GetData(int wind, int minX, int maxX, int minY, int maxY, int minAngle, int maxAngle)
        {
            DataTable records = new DataTable();
            cmd = $@"select hitX, hitY, angle, power " +
                  $@"from {table} where wind = {wind} and hitX >= {minX} and hitX <= {maxX} and hitY >= {minY} and hitY <= {maxY} and angle >= {minAngle} and angle <= {maxAngle}";
            command.CommandText = cmd;
            adapter = new SqlDataAdapter(command);
            adapter.Fill(records);
            return records;
        }
        public int GetCount(int wind, int minX, int maxX, int minY, int maxY, int minAngle, int maxAngle)
        {
            int count;
            DataTable records = new DataTable();
            cmd = $@"select count(hitX)" +
                  $@"from {table} where wind = {wind} and hitX >= {minX} and hitX <= {maxX} and hitY >= {minY} and hitY <= {maxY} and angle >= {minAngle} and angle <= {maxAngle}";
            command.CommandText = cmd;
            reader = command.ExecuteReader();
            reader.Read();
            count = (int)reader[0];
            reader.Close();
            return count;
        }
    }
}
