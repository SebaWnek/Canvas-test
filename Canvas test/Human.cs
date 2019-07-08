using System;

namespace Canvas_test
{
    class Human : Player
    {
        protected override void CheckLineOfSight()
        {
            throw new NotImplementedException();
        }

        protected override Bullet ChooseBullet()
        {
            throw new NotImplementedException();
        }

        public override int[] ChooseParameters()
        {
            throw new NotImplementedException();
        }

        protected override Tank ChooseTarget()
        {
            throw new NotImplementedException();
        }

        public Human(Tank player) : base(player)
        {
            Type = PlayerType.Human;
        }
    }
}
