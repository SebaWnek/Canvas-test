using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Windows.Media.Animation;

namespace Canvas_test
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Timers.Timer timer = new System.Timers.Timer();
        int timeInterval = 10;
        int wind;
        Random rnd = new Random();
        Ball ball;
        int terrainLength = 1000;
        int maxV = 50; 
        CanvasConvert coord;
        Ground terrain;
        Player player1;
        bool hit = false;
        bool inside;
        int angle = 45;
        int velocity = 20;
        int damage = 40;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ball.CalculateNewPosition(wind, timeInterval);
            Dispatcher.InvokeAsync(() =>
            {
                xBlock.Text = ball.X.ToString();
                yBlock.Text = ball.Y.ToString();
                vxBlock.Text = ball.SpeedX.ToString();
                vyBlock.Text = ball.SpeedY.ToString();
                line.Points.Add(coord.ToWindowsPoint(ball.X, ball.Y));
                hit = terrain.CheckHit(ball.X,ball.Y);
                inside = ball.Y >= 0;
                if (hit == true)
                {
                    timer.Stop();
                    GenerateWind();
                    terrain.DestroyTerrain(ball.X, damage);
                    MoveTank();
                    MoveTarget();
                    ShowExplosion();
                }
                if (inside == false)
                {
                    timer.Stop();
                    GenerateWind();
                }
            });
        }

        private void ShowExplosion()
        {
            Canvas.SetLeft(explosion, coord.ToInt(ball.X, ball.Y)[0] - explosion.Width / 2);
            Canvas.SetTop(explosion, coord.ToInt(ball.X, ball.Y)[1] - explosion.Height / 2);
            explosion.Opacity = 1;
            var animation = new DoubleAnimation
            {
                To = 0,
                BeginTime = TimeSpan.FromSeconds(0),
                Duration = TimeSpan.FromSeconds(2),
                FillBehavior = FillBehavior.Stop
            };
            animation.Completed += (s, a) => explosion.Opacity = 0;
            explosion.BeginAnimation(UIElement.OpacityProperty, animation);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            coord = new CanvasConvert(background.ActualHeight, background.ActualWidth, terrainLength);
            terrain = new Ground(terrainLength, coord, ground);
            timer.Elapsed += Timer_Elapsed;
            timer.Interval = timeInterval;
            angleblock.Text = angle.ToString();
            vblock.Text = velocity.ToString();
            GenerateWind();
            player1 = new Player(10, player1image);
            player1.targetDistance = velocity;
            MoveTank();
            MoveTarget();
            explosion.Opacity = 0;
        }

        private void MoveTank()
        {
            Canvas.SetLeft(player1image, coord.ToInt(player1.Position, terrain.Height[player1.Position])[0] - 5);
            Canvas.SetTop(player1image, coord.ToInt(player1.Position, terrain.Height[player1.Position])[1] - 5);
        }

        private void MoveTarget()
        {
            Canvas.SetLeft(player1target, coord.ToInt(player1.Position + player1.GetTargetPosition(angle)[0],
                terrain.Height[player1.Position] + player1.GetTargetPosition(angle)[1])[0] - 2);
            Canvas.SetTop(player1target, coord.ToInt(player1.Position + player1.GetTargetPosition(angle)[0],
                terrain.Height[player1.Position] + player1.GetTargetPosition(angle)[1])[1] - 2);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                line.Points.Clear();
                hit = false;
                inside = true;
                ball = new Ball(player1.Position, terrain.Height[player1.Position] + 1, velocity, angle, player1.Direction);
                line.Points.Add(coord.ToWindowsPoint(ball.X, ball.Y));
                timer.Start();
            }
            if (e.Key == Key.Up)
            {
                if (velocity < maxV)
                {
                    velocity++;
                    player1.targetDistance = velocity;
                    vblock.Text = velocity.ToString();
                    MoveTarget();
                }
            }
            if (e.Key == Key.Down && velocity > 0)
            {
                if (velocity > 0)
                {
                    velocity--;
                    player1.targetDistance = velocity;
                    vblock.Text = velocity.ToString();
                    MoveTarget();
                }
            }
            if (e.Key == Key.Left)
            {
                angle--;
                MoveTarget();
            }
            if (e.Key == Key.Right)
            {
                angle++;
                MoveTarget();
            }
            if (e.Key == Key.A)
            {
                if (player1.Position > 0)
                {
                    player1.Position--;
                    MoveTarget();
                    MoveTank();
                }
            }
            if (e.Key == Key.D)
            {
                if (player1.Position < terrainLength)
                {
                    player1.Position++;
                    MoveTarget();
                    MoveTank();
                }
            }
        }

        private void GenerateWind()
        {
            wind = rnd.Next(-5, 6);
            Dispatcher.Invoke(() =>
            {
                windBlock.Text = wind.ToString();
            });
        }
    }
}
