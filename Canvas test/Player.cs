using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace Canvas_test
{
    class Player
    {
        private Rectangle Body;
        public int Position { get; set; }
        public int targetDistance { get; set; } = 50;
        public char Direction { get; set; } = 'r';


        public Player(int position, Rectangle rect)
        {
            Position = position;
            Body = rect;
        }

        public double[] GetTargetPosition(int angle)
        {
            double radAngle;
            if (Direction == 'r')
            {
                radAngle = (90 - angle) * Math.PI / 180;
            }
            else
            {
                radAngle = -(90 - angle) * Math.PI / 180;
            }
            double x = Math.Sin(radAngle) * targetDistance;
            double y = Math.Cos(radAngle) * targetDistance;
            return new double[] { x, y };
        }
    }
}
