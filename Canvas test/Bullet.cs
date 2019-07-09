using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Canvas_test
{
    public class Bullet
    {
        public enum BulletType
        {
            SmallBullet,
            BigBullet,
            Nuclear,
            Sniper
        }
        public BulletType Type { get; set; }
        public int Damage { get; set; }
        public int ExplosionRadius { get; set; }
        public int ExplosionDestroyDistance { get; set; }
        public int BulletCount { get; set; }

        private readonly double gravity = 9.81;
        public double X { get; set; }
        public double Y { get; set; }
        public double SpeedX { get; set; }
        public double SpeedY { get; set; }
        public double SpeedMultiplier { get; set; }
        public double MaxVMultiplier { get; set; } = 1;

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
                    SpeedMultiplier = 1;
                    MaxVMultiplier = 1;
                    break;
                case BulletType.BigBullet:
                    Damage = 40;
                    ExplosionRadius = 160;
                    ExplosionDestroyDistance = 50;
                    SpeedMultiplier = 1;
                    MaxVMultiplier = 1;
                    break;
                case BulletType.Nuclear:
                    Damage = 80;
                    ExplosionRadius = 200;
                    ExplosionDestroyDistance = 100;
                    SpeedMultiplier = 1;
                    MaxVMultiplier = 0.5;
                    break;
                case BulletType.Sniper:
                    Damage = 80;
                    ExplosionRadius = 30;
                    ExplosionDestroyDistance = 10;
                    SpeedMultiplier = 10;
                    MaxVMultiplier = 1;
                    break;
                default:
                    throw new Exception("Wrong bullet type");
            }
        }
        public void FireBullet(double x, double y, double speed, int angle, char direction)
        {
            double radAngle;
            X = x;
            Y = y + 2;
            if (direction == 'r')
            {
                radAngle = (angle) * Math.PI / 180;
            }
            else
            {
                radAngle = (-180 - angle) * Math.PI / 180;
            }
            SpeedX = Math.Cos(radAngle) * speed * SpeedMultiplier;
            SpeedY = Math.Sin(radAngle) * speed * SpeedMultiplier;
        }
        public void CalculateNewPosition(double wind, int timeInterval)
        {
            X += SpeedX * timeInterval / 1000;
            Y += SpeedY * timeInterval / 1000;
            SpeedX += wind * timeInterval / 1000;
            SpeedY -= gravity * timeInterval / 1000;
        }

        public List<System.Windows.Point> CalulateShot(CanvasConvert coord, Ground terrain, int wind, out bool hit)
        {
            bool terrainHit = false;
            bool tankHit = false;
            bool overGround = true;
            bool inside = true;
            int timeInterval = 2;
            List<System.Windows.Point> result = new List<System.Windows.Point>();
            result.Add(coord.ToWindowsPoint(X, Y));
            while (!terrainHit && !tankHit && overGround)
            {
                inside = X >= -1 && X <= terrain.terrainLength + 1;
                overGround = Y >= 0;
                CalculateNewPosition(wind, timeInterval);
                if (inside)
                {
                    result.Add(coord.ToWindowsPoint(X, Y)); 
                }
                terrainHit = terrain.CheckHit(X, Y);
                tankHit = Tank.CheckHit(X, Y);
            }
            hit = terrainHit || tankHit;
            return result;
        }
    }
}
