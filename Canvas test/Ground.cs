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
            Height[0] = rnd.Next(100, 200);
            for (int i = 1; i <= terrainLength; i++)
            {
                Height[i] = Height[i - 1] + rnd.Next(-10, 11) * variations;
                if (Height[i] < 0)
                {
                    Height[i] = 0;
                }
            }
        }

        public void DestroyTerrain(double hitX, int size)
        {
            int hitXint = (int)Math.Round(hitX) + 1;
            Height[hitXint] -= size;
            for (int i = 1; i < size; i++)
            {
                if (hitXint - i >= 0)
                {
                    Height[hitXint - i] -= (size - i * i / size);
                }
                if (hitXint + i <= terrainLength)
                {
                    Height[hitXint + i] -= (size - i * i / size);
                }
            }
            HeigthToTerrain();
        }

        public bool CheckHit(double x, double y)
        {
            int roundX = (int)Math.Round(x);
            if (roundX > 0 && roundX <= terrainLength + 1 && y <= Height[roundX])
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
