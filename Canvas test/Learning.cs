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
        sqlConnector connector;
        RegCalculator calc;
        int minAngle;
        int aDelta = 5;
        int counterMax = 5;

        public Learning(Tank player) : base(player)
        {
            Type = PlayerType.Learning;
            connector = main.logger;
            calc = new RegCalculator(connector, main.Terrain.terrainLength, main.MaxV);
        }

        public override double[] ChooseParameters()
        {
            target = ChooseTarget();
            double[] result = CalculateParameters();
            if(result[1] > main.MaxV && main.AlivePlayers.Count > 2)
            {
                Tank targetOld = target;
                target = ChooseTarget();
                if (target != targetOld)
                {
                    result = CalculateParameters(); 
                }
            }
            if (result[1] > main.MaxV) result[1] = main.MaxV;
            return result;
        }

        private double[] CalculateParameters()
        {
            CalclulateRelativeCoordinates();
            minAngle = FindMinAngle(target);
            int angle = random.Next(minAngle, minAngle + 10);
            player.SelectedBullet = ChooseBullet();
            int windDirection = main.Wind / relativeX >= 0 ? 1 : -1;
            int relativeWind = windDirection * Math.Abs(main.Wind);
            int direction = relativeX >= 0 ? 0 : 1;
            relativeX = Math.Abs(relativeX);
            int power = FindPower(ref angle, relativeWind);
            double calcPower = calc.CalculatePowerFromAngle(relativeX, relativeY, angle, windDirection * Math.Abs(main.Wind));

            if (power <= 5) power = 5;
            return new double[] { direction, power, angle, relativeX, relativeY, windDirection * Math.Abs(main.Wind), 0, player.SelectedBullet.SpeedMultiplier, calcPower };
        }

        private int FindPower(ref int angle, int relativeWind)
        {
            int power;
            if (connector.isConnected)
            {
                power = calc.CalculatePower(relativeWind, (int)Math.Round(relativeX), (int)Math.Round(relativeY), angle);
                int powerOld;
                int counter = 0;
                while (power > main.MaxV)
                {
                    angle += aDelta;
                    powerOld = power;
                    if (counter < counterMax)
                    {
                        power = calc.CalculateSecondPower(relativeWind, (int)Math.Round(relativeX), (int)Math.Round(relativeY), angle);
                        counter++;
                    }
                    else
                    {
                        power = calc.CalculatePower(relativeWind, (int)Math.Round(relativeX), (int)Math.Round(relativeY), angle);
                        counter = 0;
                    }
                    if (power > powerOld)
                    {
                        power = powerOld;
                        angle -= aDelta;
                        break;
                    }
                } 
            }
            else
            {
                power = random.Next(0, main.MaxV + 1);
            }

            return power;
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
            tmpMinAngle += 20;
            if (tmpMinAngle > 70) tmpMinAngle = 70;
            return (int)tmpMinAngle;
        }
    }
}
