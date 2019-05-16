using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Canvas_test
{
    class Ball
    {
        private readonly double gravity = 9.81;
        public double X { get; set; }
        public double Y { get; set; }
        public double SpeedX { get; set; }
        public double SpeedY { get; set; }
        public Ball(double x, double y, double speedx, double speedy)
        {
            X = x;
            Y = y;
            SpeedX = speedx;
            SpeedY = speedy;
        }
        public Ball(double x, double y, double speed, int angle, char direction)
        {
            double radAngle;
            X = x;
            Y = y;
            if (direction == 'r')
            {
                radAngle = -(angle - 90) * Math.PI / 180; 
            }
            else
            {
                radAngle = -(90 - angle) * Math.PI / 180;
            }
            SpeedX = Math.Cos(radAngle) * speed;
            SpeedY = Math.Sin(radAngle) * speed;
        }
        public void CalculateNewPosition(double wind, int timeInterval)
        {
            X += SpeedX * timeInterval / 1000;
            Y += SpeedY * timeInterval / 1000;
            SpeedX -= gravity * timeInterval / 1000;
            SpeedY += wind * timeInterval / 1000;
        }
    }
}
