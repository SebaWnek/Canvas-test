﻿using System;
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
    public class Ground
    {
        public double[] Height { get; set; }
        CanvasConvert coord;
        public int terrainLength;
        Polygon ground;
        Random rnd = new Random();
        int maxHeight;
        public enum GroundType
        {
            normal,
            flat,
            zero,
            vShaped
        }
        public Ground(int length, int maxHeight, CanvasConvert cd, Polygon gr, GroundType type)
        {
            coord = cd;
            ground = gr;
            terrainLength = length;
            this.maxHeight = maxHeight;
            Height = new double[terrainLength + 1];
            GenerateTerrain(type, 0.1);
            HeigthToTerrain();
        }

        public void ResetGround(GroundType type)
        {
            Height = new double[terrainLength + 1];
            GenerateTerrain(type, 0.1);
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

        private void GenerateTerrain(GroundType type, double variations)
        {
            int signCounter = rnd.Next(0, terrainLength / 10);
            int sign = rnd.Next(2);
            if (sign == 0) sign = -1;
            int angleCounter = rnd.Next(0, terrainLength / 10);
            int angle = rnd.Next(60);
            Height[0] = rnd.Next(200, 300);
            if (type == GroundType.normal)
            {
                for (int i = 1; i <= terrainLength; i++)
                {
                    if (i > signCounter || Height[i - 1] > maxHeight)
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

            if (type == GroundType.vShaped)
            {
                for (int i = 0; i < terrainLength; i++)
                {
                    Height[i] = Math.Abs(i - terrainLength / 2);
                }
            }

            if (type == GroundType.flat)
            {
                int h = rnd.Next(100, 401);
                for (int i = 0; i < terrainLength; i++)
                {
                    Height[i] = h;
                }
            }
            if (type == GroundType.zero)
            {
                for (int i = 0; i < terrainLength; i++)
                {
                    Height[i] = 1;
                }
            }
        }

        public void DestroyTerrain(double hitX, int size)
        {
            int hitXint;
            if (hitX >= 0 && hitX <= terrainLength)
            {
                hitXint = (int)Math.Round(hitX);
            }
            else if (hitX > terrainLength) hitXint = terrainLength;
            else hitXint = 0;
            double medium = Height[hitXint];
            double tmp;
            for (int i = -size; i < size; i++)
            {
                if (hitXint - i >= 0 && hitXint - i <= terrainLength)
                {
                    tmp = (size - i * i / size) + rnd.Next(-1, 2);
                    if (tmp > 0 && Height[hitXint - i] > medium + tmp)
                    {
                        Height[hitXint - i] -= tmp * 2;
                    } 
                    else if (tmp > 0 && Height[hitXint - i] > medium - tmp)
                    {
                        Height[hitXint - i] = medium - tmp;
                    }
                }
            }
            LevelTerrain();
            HeigthToTerrain();
        }

        private void LevelTerrain()
        {
            for (int i = 0; i < terrainLength; i++)
            {
                if (Height[i] <= 0) Height[i] = 1;
            }
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
