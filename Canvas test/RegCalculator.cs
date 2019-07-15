using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Canvas_test
{
    public class RegCalculator
    {
        DataTable records;
        sqlLogger logger;
        double a;
        double b;
        double c;
        double x0;
        double treshold = 1.05;
        int x1, x2;
        int maxX = 30;

        public RegCalculator(sqlLogger lg)
        {
            logger = lg;
        }

        private void GetData(int wind, int minAngle, int maxAngle, int minPower, int maxPower)
        {
            if (logger.isConnected)
            {
                records = logger.GetData(wind, minAngle, maxAngle, minPower, maxPower);
            }
        }

        private void CalculateRegression()
        {
            double Sx = 0;
            double Sy = 0;
            double Sx2 = 0;
            double Sx3 = 0;
            double Sx4 = 0;
            double Sxy = 0;
            double Sx2y = 0;
            double xx;
            double xy;
            double xx2;
            double x2y;
            double x2x2;
            double n = records.Rows.Count;

            foreach (DataRow row in records.Rows)
            {
                Sx += double.Parse(row[0].ToString());
                Sy += double.Parse(row[1].ToString());
                Sx2 += Math.Pow(double.Parse(row[0].ToString()), 2);
                Sx3 += Math.Pow(double.Parse(row[0].ToString()), 3);
                Sx4 += Math.Pow(double.Parse(row[0].ToString()), 4);
                Sxy += double.Parse(row[0].ToString()) * double.Parse(row[1].ToString());
                Sx2y += Math.Pow(double.Parse(row[0].ToString()), 2) * double.Parse(row[1].ToString());
            }

            xx = Sx2 - Sx * Sx / n;
            xy = Sxy - Sx * Sy / n;
            xx2 = Sx3 - Sx2 * Sx / n;
            x2y = Sx2y - Sx2 * Sy / n;
            x2x2 = Sx4 - Sx2 * Sx2 / n;

            a = (x2y * xx - xy * xx2) / (xx * x2x2 - xx2 * xx2);
            b = (xy * x2x2 - x2y * xx2) / (xx * x2x2 - xx2 * xx2);
            c = Sy / n - b * Sx / n - a * Sx2 / n;
        }

        private void CalculateMinimum()
        {
            double da = 2 * a;
            double db = b;
            x0 = -db / da;
            Console.WriteLine(x0);
        }

        private double CalcValue(double x)
        {
            return a * x * x + b * x + c;
        }

        public int[] FindMultipliers(int wind, int minAngle, int maxAngle, int minPower, int maxPower)
        {
            GetData(wind, minAngle, maxAngle, minPower, maxPower);
            CalculateRegression();
            CalculateMinimum();
            x1 = 0;
            x2 = maxX;
            while (CalcValue(x1) > CalcValue(x0) * treshold && x1 < x2)
            {
                x1++;
            }
            while (CalcValue(x2) > CalcValue(x0) * treshold && x2 > x1)
            {
                x2--;
            }
            if (x1 == 30 && x2 == 30) x1 = 0;

            Application.Current.Dispatcher.Invoke(() =>
            {
                ((MainWindow)Application.Current.MainWindow).recCalcBlock.Text = $"x0: {x0}\nx1: {x1}, x2: {x2}\n{a}x2 + {b}x + {c}";
            });

            return new int[] { x1, x2 };
        }
    }
}
