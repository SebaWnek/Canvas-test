using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;
using System.Reflection;

namespace Canvas_test
{
    class Player
    {
        int HP { get; set; } = 100;
        public double PositionX { get; set; }
        public double PositionY { get; set; }
        public int angle { get; set; } = 45;
        public char Direction { get; set; } = 'r';
        public Rectangle image;
        public Rectangle target;
        public Rectangle activeSign;

        public Label NameLabel;
        public Label HPLabel;
        public string Name { get; set; }
        CanvasConvert coord;
        Ground terrain;

        public int Velocity { get; set; } = 20;
        public int Damage { get; set; } = 40;
        Brush color { get; set; }


        public Player(int position, string name, System.Windows.Media.Brush col, CanvasConvert cc, Ground tr)
        {
            color = col;

            image = new Rectangle();
            image.Stroke = Brushes.Black;
            image.Fill = color;
            image.Height = 10;
            image.Width = 10;

            target = new Rectangle();
            target.Fill = Brushes.Red;
            target.Height = 4;
            target.Width = 4;
            target.Visibility = System.Windows.Visibility.Hidden;

            activeSign = new Rectangle();
            activeSign.Fill = Brushes.Red;
            activeSign.Height = 30;
            activeSign.Width = 2;
            activeSign.Visibility = System.Windows.Visibility.Hidden;

            NameLabel = new Label();
            NameLabel.Content = name;
            Name = name;

            HPLabel = new Label();
            HPLabel.Content = HP;

            ((MainWindow)App.Current.MainWindow).AddToBackgroud(image);
            ((MainWindow)App.Current.MainWindow).AddToBackgroud(target);
            ((MainWindow)App.Current.MainWindow).AddToBackgroud(NameLabel);
            ((MainWindow)App.Current.MainWindow).AddToBackgroud(activeSign);
            ((MainWindow)App.Current.MainWindow).AddToBackgroud(HPLabel);

            coord = cc;
            terrain = tr;

            PositionX = position;
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
            double x = Math.Sin(radAngle) * Velocity;
            double y = Math.Cos(radAngle) * Velocity;
            return new double[] { x, y };
        }

        public void MoveTank()
        {
            PositionY = terrain.Height[(int)PositionX];
            Canvas.SetLeft(image, coord.ToInt(PositionX, PositionY)[0] - 5);
            Canvas.SetTop(image, coord.ToInt(PositionX, PositionY)[1] - 5);
            Canvas.SetLeft(NameLabel, coord.ToInt(PositionX, PositionY)[0] - 25);
            Canvas.SetTop(NameLabel, coord.ToInt(PositionX, PositionY)[1] + 10);
            Canvas.SetLeft(HPLabel, coord.ToInt(PositionX, PositionY)[0] - 15);
            Canvas.SetTop(HPLabel, coord.ToInt(PositionX, PositionY)[1] + 30);
            Canvas.SetLeft(activeSign, coord.ToInt(PositionX, PositionY)[0] - 1);
            Canvas.SetTop(activeSign, coord.ToInt(PositionX, PositionY)[1] - 50);
            MoveTarget();
        }

        public void MoveTarget()
        {
            Canvas.SetLeft(target, coord.ToInt(PositionX + GetTargetPosition(angle)[0],
                PositionY + GetTargetPosition(angle)[1])[0] - 2);
            Canvas.SetTop(target, coord.ToInt(PositionX + GetTargetPosition(angle)[0],
                PositionY + GetTargetPosition(angle)[1])[1] - 2);
        }

        public bool CheckIfAlive(double x, int dmg)
        {
            double y = terrain.Height[(int)Math.Round(x)];
            double deltaX = Math.Abs(x - PositionX);
            double deltaY = Math.Abs(y - PositionY);
            int distance = (int)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            if(distance <= dmg)
            {
                HP -= (dmg - distance);
                HPLabel.Content = HP;
            }
            if (HP > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void RemovePlayer()
        {
            ((MainWindow)App.Current.MainWindow).RemoveFromBackground(image);
            ((MainWindow)App.Current.MainWindow).RemoveFromBackground(target);
            ((MainWindow)App.Current.MainWindow).RemoveFromBackground(NameLabel);
            ((MainWindow)App.Current.MainWindow).RemoveFromBackground(activeSign);
            ((MainWindow)App.Current.MainWindow).RemoveFromBackground(HPLabel);
        }
    }
}
