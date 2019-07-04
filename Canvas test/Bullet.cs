using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Canvas_test
{
    class Bullet
    {
        public enum BulletType
        {
            SmallBullet,
            BigBullet,
            Nuclear
        }
        public BulletType Type { get; set; }
        public int Damage { get; set; }
        public int ExplosionRadius { get; set; }
        public int ExplosionDestroyDistance { get; set; }
        public int BulletCount { get; set; }

        int animationSpeed { get; set; } = 5;
        private readonly double gravity = 9.81;
        public double X { get; set; }
        public double Y { get; set; }
        public double SpeedX { get; set; }
        public double SpeedY { get; set; }

        public override string ToString()
        {
            return BulletCount.ToString() + " - " + Type.ToString();
        }

        public Bullet(BulletType bullet, int count)
        {
            Type = bullet;
            BulletCount = count;
            switch (bullet)
            {
                case BulletType.SmallBullet:
                    Damage = 10;
                    ExplosionRadius = 80;
                    ExplosionDestroyDistance = 20;
                    break;
                case BulletType.BigBullet:
                    Damage = 40;
                    ExplosionRadius = 160;
                    ExplosionDestroyDistance = 50;
                    break;
                case BulletType.Nuclear:
                    Damage = 100;
                    ExplosionRadius = 200;
                    ExplosionDestroyDistance = 150;
                    break;
                default:
                    throw new Exception("Wrong bullet type");
            }
        }
        public void FireBullet(double x, double y, double speed, int angle, char direction)
        {
            double radAngle;
            X = x;
            Y = y;
            if (direction == 'r')
            {
                radAngle = (angle) * Math.PI / 180;
            }
            else
            {
                radAngle = (-180 - angle) * Math.PI / 180;
            }
            SpeedX = Math.Cos(radAngle) * speed;
            SpeedY = Math.Sin(radAngle) * speed;
        }
        public void CalculateNewPosition(double wind, int timeInterval)
        {
            X += animationSpeed * SpeedX * timeInterval / 1000;
            Y += animationSpeed * SpeedY * timeInterval / 1000;
            SpeedX += wind * timeInterval / 1000;
            SpeedY -= gravity * timeInterval / 1000;
        }
    }
}
