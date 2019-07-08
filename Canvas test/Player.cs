using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Canvas_test
{
    public abstract class Player
    {
        protected MainWindow main = Application.Current.MainWindow as MainWindow;
        protected double relativeX;
        protected double relativeY;
        protected double g = 9.81;
        static protected Random random = new Random();
        public enum PlayerType
        {
            Human,
            Easy,
            Medium,
            Hard
        }
        public PlayerType Type { get; set; }
        protected Tank player;
        protected Tank target;

        public abstract double[] ChooseParameters();
        protected abstract Bullet ChooseBullet();
        protected abstract Tank ChooseTarget();

        public Player(Tank player)
        {
            this.player = player;
        }

        protected void CalclulateRelativeCoordinates()
        {
            relativeX = target.PositionX - player.PositionX;
            relativeY = target.PositionY - player.PositionY;
        }
    }
}
