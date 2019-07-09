using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Canvas_test
{
    class Hard : Player
    {
        static Dictionary<Tank, int> targets = new Dictionary<Tank, int>();
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
            minAngle = FindMinAngle(null, out lineOfSight);
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
            if(main.Players.Count == 2)
            {
                foreach(Tank tank in main.Players)
                {
                    if (tank != player) return tank;
                }
            }
            targets.Clear();
            int score;
            foreach (Tank tank in main.Players)
            {
                if (tank != player)
                {
                    score = EvaluateTarget(tank);
                    targets.Add(tank, score);
                }
            }
            var sortList = targets.ToList();
            sortList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
            return sortList.Last().Key;
        }

        private int EvaluateTarget(Tank tank)
        {
            int score = 0;
            CalclulateRelativeCoordinates(tank);
            double distance = Math.Sqrt(relativeX * relativeX + relativeY * relativeY);
            int HP = tank.HP;
            bool los;
            FindMinAngle(tank, out los);
            if (distance < 30) score -= 10;
            if (distance < 100) score -= 5;
            if (distance > 600) score -= 3;
            if (HP < 20) score += 5;
            if (HP >= 20 && HP < 50) score += 3;
            if (HP >= 50 && HP < 70) score += 1;
            if (los && player.Bullets[Bullet.BulletType.Sniper].BulletCount > 0) score += 6;
            if (los && player.Bullets[Bullet.BulletType.Sniper].BulletCount == 0) score += 3;

            return score;
        }

        private int FindMinAngle(Tank tar, out bool los)
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
            double targetY = main.Terrain.Height[(int)Math.Round(targetX)];
            double playerY = player.PositionY;
            double playerX = player.PositionX;
            double x;
            double y;
            double tmpMinAngle = Math.Asin((targetY - playerY) / Math.Abs(playerX - targetX)) * 180 / Math.PI + 1;
            los = true;

            double tmpAngle;
            int minX = targetX < playerX ? (int)targetX : (int)playerX;
            int maxX = targetX < playerX ? (int)playerX : (int)targetX;
            for (int i = minX; i < maxX; i++)
            {
                x = Math.Abs(playerX - i);
                y = terrain[i] - playerY;
                tmpAngle = Math.Asin(y / x) * 180 / Math.PI;
                if (tmpAngle > tmpMinAngle && x > 0)
                {
                    tmpMinAngle = tmpAngle;
                    los = false;
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
                results[i] = CalculatePowerFromAngle(i - 90);
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
                for (int i = 0; i < powers.Length - 1; i++)
                {
                    if (powers[i] > 0 && powers[i] < choosenPower)
                    {
                        choosenPower = powers[i];
                        choosenAngle = i - 90;
                        result = new double[] { choosenPower / player.SelectedBullet.SpeedMultiplier, choosenAngle };
                        break;
                    }
                }
            }
            else if (minAngle <= 60)
            {
                for (int i = minAngle + 110; i < powers.Length - 1; i++)
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
                choosenPower = int.MaxValue;
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
