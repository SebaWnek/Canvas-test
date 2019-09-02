using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Canvas_test
{
    class Medium : Player
    {
        List<Tank> targets = new List<Tank>();
        int targetCount;
        double windDivider;
        int minAngle;
        public Medium(Tank player) : base(player)
        {
            Type = PlayerType.Medium;
        }
        public override double[] ChooseParameters()
        {
            windDivider = random.Next(8, 12);
            target = ChooseTarget();
            CalclulateRelativeCoordinates();
            minAngle = FindMinAngle(target);
            int angle = random.Next(minAngle,minAngle + 10);
            if (angle > 85) angle = 80;
            player.SelectedBullet = ChooseBullet();
            double windCoefficient = Math.Abs(main.Wind) * Math.Abs(main.Wind) * ((double)angle / 61) * ((double)angle / 61) / windDivider;
            int windDirection = main.Wind/relativeX >= 0 ? 1 : -1;

            int direction = relativeX >= 0 ? 0 : 1;
            relativeX = Math.Abs(relativeX);
            double radAngle = (angle) * Math.PI / 180;
            int power = (int)Math.Sqrt((g * relativeX * relativeX) / 
                                  (relativeX * Math.Sin(2 * radAngle) - 2 * relativeY * Math.Cos(radAngle) * Math.Cos(radAngle)));
            power = power - windDirection * (int)(windCoefficient * power);
            if (power > 100) power = 100;
            if (power <= 0) power = 10;
            return new double[] { direction, power, angle, relativeX, relativeY, windDirection * Math.Abs(main.Wind), windDivider, player.SelectedBullet.SpeedMultiplier };
        }

        private int FindMinAngle(Tank tar)
        {
            double[] terrain = main.Terrain.Height;
            double targetX;
            if (tar != null)
            {
                targetX = tar.PositionX;
            }
            else
            {
                targetX = target.PositionX;
            }
            double playerY = player.PositionY;
            double playerX = player.PositionX;
            double x;
            double y;
            double tmpMinAngle = 0;

            double tmpAngle;
            int minX = targetX < playerX ? 0 : (int)playerX;
            int maxX = targetX < playerX ? (int)playerX : terrain.Length - 1;
            for (int i = minX; i < maxX; i++)
            {
                x = Math.Abs(playerX - i);
                y = terrain[i] - playerY;
                tmpAngle = Math.Atan(y / x) * 180 / Math.PI;
                if (tmpAngle > tmpMinAngle && x > 0)
                {
                    tmpMinAngle = tmpAngle;
                }
            }
            tmpMinAngle += 10;
            if (tmpMinAngle > 70) tmpMinAngle = 70;
            return (int)tmpMinAngle;
        }

        protected override Bullet ChooseBullet()
        {
            if (player.Bullets[Bullet.BulletType.BigBullet].BulletCount > 0)
            {
                return player.Bullets[Bullet.BulletType.BigBullet];

            }
            else
            {
                return player.Bullets[Bullet.BulletType.SmallBullet];
            }
        }

        protected Tank ChooseTarget()
        {
            targets.Clear();
            foreach(Tank tank in main.AlivePlayers)
            {
                if(tank != player)
                {
                    targets.Add(tank);
                }
            }
            targetCount = targets.Count;
            return targets[random.Next(targetCount)];
        }
    }
}
