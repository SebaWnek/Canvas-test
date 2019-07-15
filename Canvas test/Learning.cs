using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Canvas_test
{
    class Learning : Medium
    {
        int windDivider;
        int[] windMinMax;
        sqlLogger logger;
        RegCalculator calc;
        int minAngle;

        public Learning(Tank player) : base(player)
        {
            Type = PlayerType.Learning;
            logger = main.logger;
            calc = new RegCalculator(logger);
        }

        public override double[] ChooseParameters()
        {
            target = ChooseTarget();
            CalclulateRelativeCoordinates();
            minAngle = FindMinAngle(target);
            int angle = random.Next(minAngle, minAngle + 30);
            if (angle > 85) angle = 80;
            player.SelectedBullet = ChooseBullet();
            double windCoefficient = Math.Abs(main.Wind) * Math.Abs(main.Wind) * ((double)angle / 61) * ((double)angle / 61) / windDivider;
            int windDirection = main.Wind / relativeX >= 0 ? -1 : 1;

            int direction = relativeX >= 0 ? 0 : 1;
            relativeX = Math.Abs(relativeX);
            double radAngle = (angle) * Math.PI / 180;
            int power = (int)Math.Sqrt((g * relativeX * relativeX) /
                                  (relativeX * Math.Sin(2 * radAngle) - 2 * relativeY * Math.Cos(radAngle) * Math.Cos(radAngle)));

            windMinMax = calc.FindMultipliers(main.Wind, angle - 30, angle + 30, power - 30, power + 30);
            windCoefficient = random.Next(windMinMax[0], windMinMax[1] + 1);

            power = power + windDirection * (int)(windCoefficient * power);

            if (power > 100) power = 100;
            if (power <= 0) power = 10;
            return new double[] { direction, power, angle, relativeX, relativeY, windDirection * Math.Abs(main.Wind), windDivider };
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
            double targetY = main.Terrain.Height[(int)Math.Round(targetX)];
            double playerY = player.PositionY;
            double playerX = player.PositionX;
            double x;
            double y;
            double tmpMinAngle = Math.Atan((targetY - playerY) / Math.Abs(playerX - targetX)) * 180 / Math.PI + 1;

            double tmpAngle;
            int minX = targetX < playerX ? (int)targetX : (int)playerX;
            int maxX = targetX < playerX ? (int)playerX : (int)targetX;
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
            tmpMinAngle += 40;
            if (tmpMinAngle > 70) tmpMinAngle = 70;
            return (int)tmpMinAngle;
        }
    }
}
