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
        double windDivider = 8;
        public Medium(Tank player) : base(player)
        {
            Type = PlayerType.Medium;
        }
        public override double[] ChooseParameters()
        {
            target = ChooseTarget();
            CalclulateRelativeCoordinates();
            int angle = random.Next(45,61);
            player.SelectedBullet = ChooseBullet();
            double windCoefficient = Math.Abs(main.Wind) * Math.Abs(main.Wind) * ((double)angle / 61) * ((double)angle / 61) / windDivider;
            int windDirection = main.Wind/relativeX >= 0 ? -1 : 1;

            int direction = relativeX >= 0 ? 0 : 1;
            relativeX = Math.Abs(relativeX);
            double radAngle = (angle) * Math.PI / 180;
            int power = (int)Math.Sqrt((g * relativeX * relativeX) / 
                                  (relativeX * Math.Sin(2 * radAngle) - 2 * relativeY * Math.Cos(radAngle) * Math.Cos(radAngle)));
            power = power + windDirection * (int)(windCoefficient * power);
            if (power > 100) power = 100;
            if (power <= 0) power = 10;
            return new double[] { direction, power, angle, relativeX, relativeY, windDirection * Math.Abs(main.Wind) };
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
            foreach(Tank tank in main.Players)
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
