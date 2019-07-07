using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Canvas_test
{
    abstract class Player
    {
        int relativeX;
        int relativeY;
        static protected Random random = new Random();
        public enum PlayerType
        {
            Human,
            Easy,
            Medium,
            Hard
        }
        public PlayerType Type { get; set; }
        private Tank player;
        private Tank target;

        public abstract int[] ChooseParameters();
        protected abstract Bullet.BulletType ChooseBullet();
        protected abstract Tank ChooseTarget();
        protected abstract void CheckLineOfSight();

        public Player(Tank player)
        {
            this.player = player;
        }

        private void CalclulateRelativeCoordinates()
        {

        }
    }
}
