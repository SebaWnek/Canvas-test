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
        sqlConnector connector;
        double[,] X;
        double[] Y;
        public double[] B;
        int pointsCount;
        int minPointCount = 10;
        int maxAngle = 120;
        int terrainLength;
        int maxV;
        Random rnd = new Random();
        //double a;
        //double b;
        //double c;
        //double x0;
        //double treshold = 1.05;
        //int x1, x2;
        //int maxX = 30;

        //public void Test(double[,] mat, double[] vect)
        //{
        //    MatrixInput(mat, vect);
        //    double[,] XT = MatrixTranspose(X);
        //    Print(XT);
        //    double[,] result = MatrixMultiplication(XT, X);
        //    Print(result);
        //    double[,] inverse = FromNested(MatrixInverse(ToNested(result)));
        //    Print(inverse);
        //    double[,] XTXXT = MatrixMultiplication(inverse, XT);
        //    Print(XTXXT);
        //    B = MatrixVectorMultiplication(XTXXT, Y);
        //    Print(B);
        //}

        //private void Print(double[,] mat)
        //{
        //    for (int i = 0; i < mat.GetLength(0); i++)
        //    {
        //        for (int j = 0; j < mat.GetLength(1); j++)
        //        {
        //            Console.Write(mat[i, j] + ", ");
        //        }
        //        Console.WriteLine();
        //    }
        //    Console.WriteLine();
        //}
        //private void Print(double[] vect)
        //{
        //    foreach (double i in vect) Console.Write(i + ", ");
        //    Console.WriteLine();
        //}

        public RegCalculator(sqlConnector lg, int terrain, int v)
        {
            connector = lg;
            terrainLength = terrain;
            maxV = v;
        }

        public double CalculatePowerFromAngle(double X, double Y, double angle, double Wind)
        {
            double angleRad = angle * Math.PI / 180;
            double sin = Math.Sin(angleRad);
            double cos = Math.Cos(angleRad);
            double x = Math.Abs(X);
            double y = Y;
            double g = 9.81;
            double wind = Wind;
            double a = X / wind >= 0 ? Math.Abs(wind) : -Math.Abs(wind);
            double sqrt2 = Math.Sqrt(2);
            double part1 = sqrt2 * (x * sin - y * cos) * (y * a + x * g) * (a * sin + g * cos);
            double part2 = Math.Sqrt(1 / ((x * sin - y * cos) * (a * sin + g * cos)));
            double part3 = 2 * x * a * sin * sin - 2 * y * g * cos * cos - 2 * y * a * cos * sin + 2 * x * g * cos * sin;
            double power = part1 * part2 / part3;
            if (Double.IsNaN(power)) power = -1;
            return power;
        }

        public int CalculateSecondPower(int wind, int x, int y, int angle)
        {
            int power = (int)Math.Round(B[0] + B[1] * x + B[2] * y + B[3] * angle + B[4] * angle * angle);
            return power;
        }

        public int CalculatePower(int wind, int x, int y, int angle)
        {
            int power;
            int count;
            bool found = false;
            int xDelta = 50;
            int yDelta = 50;
            int aDelta = 15;
            int minX = x - xDelta;
            int maxX = x + xDelta;
            int minY = y - yDelta;
            int maxY = y + yDelta;
            int minA = angle - aDelta;
            int maxA = angle + aDelta;

            while (!found)
            {
                count = connector.GetCount(wind, minX, maxX, minY, maxY, minA, maxA);
                if (count >= minPointCount) found = true;
                else if (maxX > terrainLength || maxA > maxAngle || count == -1) break;
                else
                {
                    xDelta *= 2;
                    yDelta *= 2;
                    aDelta *= 2;
                    minX = x - xDelta;
                    maxX = x + xDelta;
                    minY = y - yDelta;
                    maxY = y + yDelta;
                }
            }
            if (found)
            {
                GetData(wind, minX, maxX, minY, maxY, minA, maxA);
                CalculateB();
                power = (int)Math.Round(B[0] + B[1] * x + B[2] * y + B[3] * angle + B[4] * angle * angle);
                return power;
            }
            else
            {
                return rnd.Next(0, maxV);
            }
        }

        private void GetData(int wind, int minX, int maxX, int minY, int maxY, int minAngle, int maxAngle)
        {
            if (connector.isConnected)
            {
                records = connector.GetData(wind, minX, maxX, minY, maxY, minAngle, maxAngle);
            }
            pointsCount = records.Rows.Count;
            DataTableToArray(records);
        }

        private void DataTableToArray(DataTable table)
        {
            DataRow row;
            X = new double[pointsCount, 5];
            Y = new double[pointsCount];
            Parallel.For(0, pointsCount, i =>
            {
                row = table.Rows[i];
                X[i, 0] = 1;
                X[i, 1] = Convert.ToDouble(row[0]);
                X[i, 2] = Convert.ToDouble(row[1]);
                X[i, 3] = Convert.ToDouble(row[2]);
                // 4th row is squares of angle
                X[i, 4] = X[i, 3] * X[i, 3];
                Y[i] = Convert.ToDouble(row[3]);
            });
        }

        //public void MatrixInput(double[,] mat, double[] vect)
        //{
        //    X = mat;
        //    Y = vect;
        //    pointsCount = X.GetLength(0);
        //}

        public void CalculateB()
        {
            double[,] XT = MatrixTranspose(X);
            double[,] result = MatrixMultiplication(XT, X);
            double[,] inverse = FromNested(MatrixInverse(ToNested(result)));
            double[,] XTXXT = MatrixMultiplication(inverse, XT);
            B = MatrixVectorMultiplication(XTXXT, Y);
        }

        private double[,] MatrixTranspose(double[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            double[,] tmp = new double[cols, rows];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    tmp[j, i] = matrix[i, j];
                }
            }
            return tmp;
        }

        private double[] MatrixVectorMultiplication(double[,] mat, double[] vect)
        {
            int rows = mat.GetLength(0);
            int cols = mat.GetLength(1);
            double[] result = new double[rows];
            Parallel.For(0, rows, i =>
            {
                for (int j = 0; j < cols; j++)
                {
                    result[i] += mat[i, j] * vect[j];
                }
            });
            return result;
        }

        private double[,] MatrixMultiplication(double[,] mat1, double[,] mat2)
        {
            int rows = mat1.GetLength(0);
            int cols = mat1.GetLength(1);
            int colsRes = mat2.GetLength(1);
            double[,] result = new double[rows, colsRes];
            Parallel.For(0, rows, i =>
            {
                for (int j = 0; j < colsRes; j++)
                {
                    for (int k = 0; k < cols; k++)
                    {
                        result[i, j] += mat1[i, k] * mat2[k, j];
                    }
                }
            });
            return result;
        }

        private double[][] ToNested(double[,] array)
        {
            int rows = array.GetLength(0);
            int cols = array.GetLength(1);
            double[][] result = new double[rows][];
            Parallel.For(0, rows, i =>
            {
                result[i] = new double[cols];
                for (int j = 0; j < cols; j++)
                {
                    result[i][j] = array[i, j];
                }
            });
            return result;
        }

        private double[,] FromNested(double[][] array)
        {
            int rows = array.Length;
            int cols = array[0].Length;
            double[,] result = new double[rows, cols];
            Parallel.For(0, rows, i =>
            {
                for (int j = 0; j < cols; j++)
                {
                    result[i, j] = array[i][j];
                }
            });
            return result;
        }

        // based on https://msdn.microsoft.com/en-us/magazine/mt736457.aspx
        static double[][] MatrixInverse(double[][] matrix)
        {
            // assumes determinant is not 0
            // that is, the matrix does have an inverse
            int n = matrix.Length;
            double[][] result = MatrixCreate(n, n); // make a copy of matrix
            for (int i = 0; i < n; ++i)
                for (int j = 0; j < n; ++j)
                    result[i][j] = matrix[i][j];

            double[][] lum; // combined lower & upper
            int[] perm;
            int toggle;
            toggle = MatrixDecompose(matrix, out lum, out perm);

            double[] b = new double[n];
            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                    if (i == perm[j])
                        b[j] = 1.0;
                    else
                        b[j] = 0.0;

                double[] x = Helper(lum, b); // 
                for (int j = 0; j < n; ++j)
                    result[j][i] = x[j];
            }
            return result;
        } // MatrixInverse

        static double[][] MatrixCreate(int rows, int cols)
        {
            double[][] result = new double[rows][];
            for (int i = 0; i < rows; ++i)
                result[i] = new double[cols];
            return result;
        }

        static int MatrixDecompose(double[][] m, out double[][] lum, out int[] perm)
        {
            // Crout's LU decomposition for matrix determinant and inverse
            // stores combined lower & upper in lum[][]
            // stores row permuations into perm[]
            // returns +1 or -1 according to even or odd number of row permutations
            // lower gets dummy 1.0s on diagonal (0.0s above)
            // upper gets lum values on diagonal (0.0s below)

            int toggle = +1; // even (+1) or odd (-1) row permutatuions
            int n = m.Length;

            // make a copy of m[][] into result lu[][]
            lum = MatrixCreate(n, n);
            for (int i = 0; i < n; ++i)
                for (int j = 0; j < n; ++j)
                    lum[i][j] = m[i][j];


            // make perm[]
            perm = new int[n];
            for (int i = 0; i < n; ++i)
                perm[i] = i;

            for (int j = 0; j < n - 1; ++j) // process by column. note n-1 
            {
                double max = Math.Abs(lum[j][j]);
                int piv = j;

                for (int i = j + 1; i < n; ++i) // find pivot index
                {
                    double xij = Math.Abs(lum[i][j]);
                    if (xij > max)
                    {
                        max = xij;
                        piv = i;
                    }
                } // i

                if (piv != j)
                {
                    double[] tmp = lum[piv]; // swap rows j, piv
                    lum[piv] = lum[j];
                    lum[j] = tmp;

                    int t = perm[piv]; // swap perm elements
                    perm[piv] = perm[j];
                    perm[j] = t;

                    toggle = -toggle;
                }

                double xjj = lum[j][j];
                if (xjj != 0.0)
                {
                    for (int i = j + 1; i < n; ++i)
                    {
                        double xij = lum[i][j] / xjj;
                        lum[i][j] = xij;
                        for (int k = j + 1; k < n; ++k)
                            lum[i][k] -= xij * lum[j][k];
                    }
                }

            } // j

            return toggle;
        } // MatrixDecompose

        static double[] Helper(double[][] luMatrix, double[] b) // helper
        {
            int n = luMatrix.Length;
            double[] x = new double[n];
            b.CopyTo(x, 0);

            for (int i = 1; i < n; ++i)
            {
                double sum = x[i];
                for (int j = 0; j < i; ++j)
                    sum -= luMatrix[i][j] * x[j];
                x[i] = sum;
            }

            x[n - 1] /= luMatrix[n - 1][n - 1];
            for (int i = n - 2; i >= 0; --i)
            {
                double sum = x[i];
                for (int j = i + 1; j < n; ++j)
                    sum -= luMatrix[i][j] * x[j];
                x[i] = sum / luMatrix[i][i];
            }

            return x;
        } // Helper

        //internal void PrintData()
        //{
        //    foreach (DataRow row in records.Rows)
        //    {
        //        Console.WriteLine(row[0] + " " + row[1]);
        //    }
        //}

        //public void CalculateRegression()
        //{
        //    double Sx = 0;
        //    double Sy = 0;
        //    double Sx2 = 0;
        //    double Sx3 = 0;
        //    double Sx4 = 0;
        //    double Sxy = 0;
        //    double Sx2y = 0;
        //    double xx;
        //    double xy;
        //    double xx2;
        //    double x2y;
        //    double x2x2;
        //    double n = records.Rows.Count;

        //    foreach (DataRow row in records.Rows)
        //    {
        //        Sx += double.Parse(row[0].ToString());
        //        Sy += double.Parse(row[1].ToString());
        //        Sx2 += Math.Pow(double.Parse(row[0].ToString()), 2);
        //        Sx3 += Math.Pow(double.Parse(row[0].ToString()), 3);
        //        Sx4 += Math.Pow(double.Parse(row[0].ToString()), 4);
        //        Sxy += double.Parse(row[0].ToString()) * double.Parse(row[1].ToString());
        //        Sx2y += Math.Pow(double.Parse(row[0].ToString()), 2) * double.Parse(row[1].ToString());
        //    }

        //    xx = Sx2 - Sx * Sx / n;
        //    xy = Sxy - Sx * Sy / n;
        //    xx2 = Sx3 - Sx2 * Sx / n;
        //    x2y = Sx2y - Sx2 * Sy / n;
        //    x2x2 = Sx4 - Sx2 * Sx2 / n;

        //    a = (x2y * xx - xy * xx2) / (xx * x2x2 - xx2 * xx2);
        //    b = (xy * x2x2 - x2y * xx2) / (xx * x2x2 - xx2 * xx2);
        //    c = Sy / n - b * Sx / n - a * Sx2 / n;
        //}

        //public void CalculateMinimum()
        //{
        //    double da = 2 * a;
        //    double db = b;
        //    x0 = -db / da;
        //    Console.WriteLine(x0);
        //}

        //public double CalcValue(double x)
        //{
        //    return a * x * x + b * x + c;
        //}

        //public int[] FindMultipliers()
        //{
        //    x1 = 0;
        //    x2 = maxX;
        //    while(CalcValue(x1) > CalcValue(x0) * treshold)
        //    {
        //        x1++;
        //    }
        //    while(CalcValue(x2) > CalcValue(x0) * treshold)
        //    {
        //        x2--;
        //    }

        //    return new int[] { x1, x2 };
        //}
    }
}
