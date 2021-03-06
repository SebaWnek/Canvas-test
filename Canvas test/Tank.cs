﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;
using System.Reflection;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Canvas_test
{
    public class Tank : INotifyPropertyChanged
    {
        public Dictionary<Bullet.BulletType, Bullet> Bullets { get; }
        private static MainWindow main = (MainWindow)Application.Current.MainWindow;
        public static int Size {get;} = 3;

        public void AddBullet(Bullet bullet)
        {
            if (Bullets.ContainsKey(bullet.Type))
            {
                Bullets[bullet.Type].BulletCount += bullet.BulletCount;
            }
            else
            {
                Bullets.Add(bullet.Type, bullet);
            }
        }

        public Bullet SelectedBullet { get; set; }

        public Player player;

        public int HP { get; set; } = 100;
        public double PositionX { get; set; }
        public double PositionY { get; set; }
        public int Angle
        {
            get => angle;
            set
            {
                angle = value;
                NotifyPropertyChanged();
            }
        }
        public char Direction { get; set; } = 'r';
        public Rectangle image;
        public Rectangle target;
        public Rectangle activeSign;

        public Label NameLabel;
        public Label HPLabel;
        public string Name { get; set; }
        CanvasConvert coord;
        Ground terrain;
        private int angle = 45;
        private double velocity = 50;

        public event PropertyChangedEventHandler PropertyChanged;

        public double Velocity
        {
            get => velocity;
            set
            {
                velocity = value;
                NotifyPropertyChanged();
            }
        }

        Brush color { get; set; }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Tank(int position, string name, System.Windows.Media.Brush col, CanvasConvert cc, Ground tr, Player.PlayerType playerType)
        {
            switch (playerType)
            {
                case Player.PlayerType.Human:
                    player = new Human(this);
                    break;
                case Player.PlayerType.Easy:
                    player = new Easy(this);
                    break;
                case Player.PlayerType.Medium:
                    player = new Medium(this);
                    break;
                case Player.PlayerType.Hard:
                    player = new Hard(this);
                    break;
                case Player.PlayerType.Learning:
                    player = new Learning(this);
                    break;
                default:
                    throw new ArgumentException("wrong player type!");
            }

            Bullets = new Dictionary<Bullet.BulletType, Bullet>();

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
            AddPlayerToMap();

            coord = cc;
            terrain = tr;

            PositionX = position;
        }


        internal static bool CheckHit(double x, double y)
        {
            bool hit = false;
            foreach(Tank tank in main.AlivePlayers)
            {
                double dx = Math.Abs(tank.PositionX - x);
                double dy = Math.Abs(tank.PositionY - y);
                if(dx < Size && dy < Size && tank != main.activePlayer)
                {
                    hit = true;
                }
            }
            return hit;
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
            double x = Math.Sin(radAngle) * Velocity / 2;
            double y = Math.Cos(radAngle) * Velocity / 2;
            return new double[] { x, y };
        }

        public void MoveTank()
        {
            PositionY = terrain.Height[(int)PositionX] + Size;
            Canvas.SetLeft(image, coord.ToInt(PositionX, PositionY)[0] - 5);
            Canvas.SetTop(image, coord.ToInt(PositionX, PositionY)[1] - 5);
            Canvas.SetLeft(NameLabel, coord.ToInt(PositionX, PositionY)[0] - 25);
            Canvas.SetTop(NameLabel, coord.ToInt(PositionX, PositionY)[1] - 30);
            Canvas.SetLeft(HPLabel, coord.ToInt(PositionX, PositionY)[0] - 15);
            Canvas.SetTop(HPLabel, coord.ToInt(PositionX, PositionY)[1] - 50);
            Canvas.SetLeft(activeSign, coord.ToInt(PositionX, PositionY)[0] - 1);
            Canvas.SetTop(activeSign, coord.ToInt(PositionX, PositionY)[1] - 50);
            MoveTarget();
        }

        public void MoveTarget()
        {
            Canvas.SetLeft(target, coord.ToInt(PositionX + GetTargetPosition(Angle)[0],
                PositionY + GetTargetPosition(Angle)[1])[0] - 2);
            Canvas.SetTop(target, coord.ToInt(PositionX + GetTargetPosition(Angle)[0],
                PositionY + GetTargetPosition(Angle)[1])[1] - 2);
        }

        public bool CheckIfAlive(double x, int dmg, int dmgRadius)
        {
            double y = terrain.Height[(int)Math.Round(x)];
            double deltaX = Math.Abs(x - PositionX);
            double deltaY = Math.Abs(y - PositionY);
            int distance = (int)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            if (distance <= dmgRadius)
            {
                HP -= dmg * (dmgRadius - distance)/dmgRadius;
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

        public void RemovePlayerFromMap()
        {
            main.RemoveFromBackground(image);
            main.RemoveFromBackground(target);
            main.RemoveFromBackground(NameLabel);
            main.RemoveFromBackground(activeSign);
            main.RemoveFromBackground(HPLabel);
        }

        public void AddPlayerToMap()
        {
            HPLabel.Content = HP;
            main.AddToBackgroud(image);
            main.AddToBackgroud(target);
            main.AddToBackgroud(NameLabel);
            main.AddToBackgroud(activeSign);
            main.AddToBackgroud(HPLabel);
        }
    }
}
