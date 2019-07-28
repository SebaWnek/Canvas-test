using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;


namespace Canvas_test
{
    class Easy : Player
    {
        public Easy(Tank player) : base(player)
        {
            Type = PlayerType.Easy;
        }

        public override double[] ChooseParameters()
        {
            int direction = random.Next(2);
            int power = random.Next(20, main.MaxV + 1);
            int angle = random.Next(20, 91);
            int windDirection = (main.Wind > 0 && direction == 0) || (main.Wind < 0 && direction == 1) ? 1 : -1;
            return new double[] { direction, power, angle, 0, 0, windDirection * Math.Abs(main.Wind), -1, player.SelectedBullet.SpeedMultiplier };
        }

        protected override Bullet ChooseBullet()
        {
            throw new NotImplementedException();
        }
    }
}
