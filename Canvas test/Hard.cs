using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Canvas_test
{
    class Hard : Player
    {
        List<Tank> targets = new List<Tank>();
        int targetCount;
        bool lineOfSight = false;

        public Hard(Tank player) : base(player)
        {
            Type = PlayerType.Easy;
        }

        public override double[] ChooseParameters()
        {
            int angle = 45;
            double power = 100;
            int direction;
            double[] results;
            int minAngle;
            double[] choosenParams;

            target = ChooseTarget();
            minAngle = FindMinAngle(out lineOfSight);
            CalclulateRelativeCoordinates();
            direction = relativeX > 0 ? 0 : 1;
            player.SelectedBullet = ChooseBullet();
            results = FindPowerFromAngle();
            choosenParams = FindParameters(results, minAngle, lineOfSight);
            power = choosenParams[0];
            angle = (int)choosenParams[1];
            return new double[] { direction, power, angle };
        }
        // to be changed later
        protected override Bullet ChooseBullet()
        {
            if(player.Bullets[Bullet.BulletType.Sniper].BulletCount > 0 & lineOfSight)
            {
                return player.Bullets[Bullet.BulletType.Sniper];
            }
            if (player.Bullets[Bullet.BulletType.Nuclear].BulletCount > 0 & target.HP > player.Bullets[Bullet.BulletType.Nuclear].Damage - 10)
            {
                return player.Bullets[Bullet.BulletType.Nuclear];
            }
            else if (player.Bullets[Bullet.BulletType.BigBullet].BulletCount > 0 & target.HP > player.Bullets[Bullet.BulletType.BigBullet].Damage - 10)
            {
                return player.Bullets[Bullet.BulletType.BigBullet];

            }
            else
            {
                return player.Bullets[Bullet.BulletType.SmallBullet];
            }
        }

        protected override Tank ChooseTarget()
        {
            targets.Clear();
            foreach (Tank tank in main.Players)
            {
                if (tank != player)
                {
                    targets.Add(tank);
                }
            }
            targetCount = targets.Count;
            return targets[random.Next(targetCount)];
        }

        private int FindMinAngle(out bool los)
        {
            double[] terrain = main.Terrain.Height;
            double targetX = target.PositionX;
            double targetY = target.PositionY;
            double playerY = player.PositionY;
            double playerX = player.PositionX;
            double x;
            double y;
            double tmpMinAngle = Math.Asin((targetY - playerY) / Math.Abs(playerX - targetX)) * 180 / Math.PI;
            los = true;

            double tmpAngle;
            int minX = targetX < playerX ? (int)targetX : (int)playerX;
            int maxX = targetX < playerX ? (int)playerX : (int)targetX;
            for (int i = minX; i < maxX; i++)
            {
                if (terrain[i] > playerY)
                {
                    x = Math.Abs(playerX - i);
                    y = terrain[i] - playerY;
                    if (x > 2)
                    {
                        tmpAngle = Math.Asin(y / x) * 180 / Math.PI;
                        if (tmpAngle > tmpMinAngle)
                        {
                            tmpMinAngle = tmpAngle;
                            los = false;
                        }
                    }
                }
            }
            return (int)tmpMinAngle;
        }

        private double CalculatePowerFromAngle(double angle)
        {
            double angleRad = angle * Math.PI / 180;
            double sin = Math.Sin(angleRad);
            double cos = Math.Cos(angleRad);
            double x = Math.Abs(relativeX);
            double y = relativeY;
            double g = 9.81;
            double wind = main.Wind;
            double a = relativeX / wind >= 0 ? Math.Abs(wind) : -Math.Abs(wind);
            double sqrt2 = Math.Sqrt(2);
            double part1 = sqrt2 * (x * sin - y * cos) * (y * a + x * g) * (a * sin + g * cos);
            double part2 = Math.Sqrt(1 / ((x * sin - y * cos) * (a * sin + g * cos)));
            double part3 = 2 * x * a * sin * sin - 2 * y * g * cos * cos - 2 * y * a * cos * sin + 2 * x * g * cos * sin;
            double power = part1 * part2 / part3;
            return power;
        }

        private double[] FindPowerFromAngle()
        {
            double[] results = new double[181];
            for (int i = 0; i < 181; i++)
            {
                results[i] = (int)Math.Round(CalculatePowerFromAngle(i - 90));
            }
            return results;
        }

        private double[] FindParameters(double[] powers, int minAngle, bool los)
        {
            double choosenPower = main.MaxV * player.SelectedBullet.SpeedMultiplier;
            int choosenAngle = minAngle;
            double[] result = new double[] { 100, 80 };

            if (los)
            {
                for (int i = minAngle +90; i < powers.Length - 1; i++)
                {
                    if (powers[i] > 0 && powers[i] < choosenPower)
                    {
                        choosenPower = powers[i];
                        choosenAngle = i - 90;
                        result = new double[] { choosenPower / player.SelectedBullet.SpeedMultiplier, choosenAngle };
                    }
                }
            }
            else if (minAngle <= 70)
            {
                for (int i = minAngle + 100; i < powers.Length; i++)
                {
                    if (powers[i] > 0 && powers[i] < choosenPower)
                    {
                        choosenPower = powers[i];
                        choosenAngle = i - 90;
                        result = new double[] { choosenPower / player.SelectedBullet.SpeedMultiplier, choosenAngle };
                    }
                }
            }
            else
            {
                for (int i = powers.Length - 1; i > 0; i--)
                {
                    if (powers[i] > 0 && powers[i] < choosenPower)
                    {
                        choosenPower = powers[i];
                        choosenAngle = i - 90;
                        result = new double[] { choosenPower / player.SelectedBullet.SpeedMultiplier, choosenAngle };
                    }
                }
            }
            if (result[0] > 100)
            {
                result[0] = 100;
            }

            return result;
        }
    }
}
