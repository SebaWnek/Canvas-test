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
    public class sqlLogger
    {
        MainWindow main;
        public bool isConnected = false;
        string username = "wneku";
        string database = "SimpleTanks";
        string table = "shotLog";
        string server = "87.207.148.101";
        SqlConnection connection;
        SqlDataAdapter reader;
        SqlCommand command;
        string cmd;
        NumberFormatInfo nfi = new NumberFormatInfo();
        double testRatio = 0.25;
        double resultRatio = 0.1;


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
                main = (MainWindow)Application.Current.MainWindow;
            }
            catch(Exception)
            {
                isConnected = false;
            }
}

        public void LogShot(double[] data)
        {
            if (isConnected && data[4] < main.MaxV && (Math.Abs(data[1] - data[3]) / (data[2] > data[0] ? data[2] : data[0])) <= testRatio)
            {
                cmd = $@"Insert into {table} values({data[0].ToString(nfi)}, {data[1].ToString(nfi)}, {data[2].ToString(nfi)}, " +
                                                 $@"{data[3].ToString(nfi)}, {data[4].ToString(nfi)}, {data[5].ToString(nfi)}, {data[6].ToString(nfi)}, {data[7]})";
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

        public DataTable GetData(int wind, int minAngle, int maxAngle, int minPower, int maxPower)
        {
            DataTable records = new DataTable();
            cmd = $@"select windDivider, avg(sqrt(power((targetX-hitX),2) + power((targetY-hitY),2))/sqrt(power(targetX,2) + power(targetY,2))*100)" +
                $@"from shotLog where ABS((targetY-hitY)/targetX) <= {resultRatio.ToString(nfi)} and wind = {wind} and power >= {minPower} and power <= {maxPower} and angle >= {minAngle} and angle <= {maxAngle}" +
                $@"group by windDivider order by avg(sqrt(power((targetX-hitX),2) + power((targetY-hitY),2))/sqrt(power(targetX,2) + power(targetY,2)))";
            command.CommandText = cmd;
            reader = new SqlDataAdapter(command);
            reader.Fill(records);
            return records;
        }
    }
}
