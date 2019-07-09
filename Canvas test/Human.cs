using System;

namespace Canvas_test
{
    class Human : Player
    {

        protected override Bullet ChooseBullet()
        {
            throw new NotImplementedException();
        }

        public override double[] ChooseParameters()
        {
            throw new NotImplementedException();
        }

        public Human(Tank player) : base(player)
        {
            Type = PlayerType.Human;
        }
    }
}
