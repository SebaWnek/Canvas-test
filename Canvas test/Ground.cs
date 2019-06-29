using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows;
using System.Windows.Shapes;

namespace Canvas_test
{
    class Ground
    {
        public double[] Height { get; set; }
        CanvasConvert coord;
        int terrainLength;
        Polygon ground;
        Random rnd = new Random();
        public Ground(int length, CanvasConvert cd, Polygon gr)
        {
            coord = cd;
            ground = gr;
            terrainLength = length;
            Height = new double[terrainLength + 1];
            GenerateTerrain(0.1);
            HeigthToTerrain();
        }

        private void HeigthToTerrain()
        {
            ground.Points.Clear();
            ground.Points.Add(coord.ToWindowsPoint(0, 0));
            for (int i = 0; i <= terrainLength; i++)
            {
                ground.Points.Add(coord.ToWindowsPoint(i, Height[i]));
            }
            ground.Points.Add(coord.ToWindowsPoint(terrainLength, 0));
        }

        private void GenerateTerrain(double variations)
        {
            int signCounter = rnd.Next(0, terrainLength / 10);
            int sign = rnd.Next(2);
            if (sign == 0) sign = -1;
            int angleCounter = rnd.Next(0, terrainLength / 10);
            int angle = rnd.Next(50);
            Height[0] = rnd.Next(100, 200);
            for (int i = 1; i <= terrainLength; i++)
            {
                if (i > signCounter)
                {
                    sign = -sign;
                    signCounter = rnd.Next(signCounter + 1, signCounter + terrainLength / 10);
                }
                if (i > angleCounter)
                {
                    angle = rnd.Next(50);
                    angleCounter = rnd.Next(angleCounter + 1, angleCounter + terrainLength / 10);
                }
                Height[i] = Height[i - 1] + rnd.Next(angle) * variations * sign;
                if (Height[i] < 0)
                {
                    Height[i] = 0;
                }
            }
        }

        public void DestroyTerrain(double hitX, int size)
        {
            int hitXint = (int)Math.Round(hitX) + 1;
            double medium = Height[hitXint];
            double tmp;
            for (int i = -size; i < size; i++)
            {
                if (hitXint - i >= 0 && hitXint - i <= terrainLength)
                {
                    tmp = Height[hitXint - i] - medium + (size - i * i / size) + rnd.Next(-1, 2);
                    if (tmp > 0)
                    {
                        Height[hitXint - i] -= tmp;
                    } 
                }
            }
            HeigthToTerrain();
        }

        public bool CheckHit(double x, double y)
        {
            int roundX = (int)Math.Round(x);
            if (roundX > 0 && roundX <= terrainLength && y <= Height[roundX])
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
