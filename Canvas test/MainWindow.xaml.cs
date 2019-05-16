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

namespace Canvas_test
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Timer timer = new Timer();
        int timeInterval = 1000 / 200;
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
        int velocity = 10;
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
                    terrain.DestroyTerrain(ball.X, 100);
                    Canvas.SetLeft(player1image, coord.ToInt(player1.Position, terrain.Height[player1.Position])[0] - 5);
                    Canvas.SetTop(player1image, coord.ToInt(player1.Position, terrain.Height[player1.Position])[1] - 5);
                }
                if (inside == false)
                {
                    timer.Stop();
                    GenerateWind();
                }
            });
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
            Canvas.SetLeft(player1image, coord.ToInt(player1.Position, terrain.Height[player1.Position])[0] - 5);
            Canvas.SetTop(player1image, coord.ToInt(player1.Position, terrain.Height[player1.Position])[1] - 5);
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
                ball = new Ball(player1.Position, terrain.Height[player1.Position], velocity, angle, player1.Direction);
                line.Points.Add(coord.ToWindowsPoint(ball.X, ball.Y));
                timer.Start();
            }
            if (e.Key == Key.Up)
            {
                if (velocity < maxV)
                {
                    velocity++;
                    vblock.Text = velocity.ToString(); 
                }
            }
            if (e.Key == Key.Down && velocity > 0)
            {
                if (velocity > 0)
                {
                    velocity--;
                    vblock.Text = velocity.ToString(); 
                }
            }
            if (e.Key == Key.Left)
            {
                angle--;
                Canvas.SetLeft(player1target, coord.ToInt(player1.Position + player1.GetTargetPosition(angle)[0],
                    terrain.Height[player1.Position] + player1.GetTargetPosition(angle)[1])[0] - 2);
                Canvas.SetTop(player1target, coord.ToInt(player1.Position + player1.GetTargetPosition(angle)[0],
                    terrain.Height[player1.Position] + player1.GetTargetPosition(angle)[1])[1] - 2);
                angleblock.Text = angle.ToString();
            }
            if (e.Key == Key.Right)
            {
                angle++;
                Canvas.SetLeft(player1target, coord.ToInt(player1.Position + player1.GetTargetPosition(angle)[0],
                    terrain.Height[player1.Position] + player1.GetTargetPosition(angle)[1])[0] - 2);
                Canvas.SetTop(player1target, coord.ToInt(player1.Position + player1.GetTargetPosition(angle)[0],
                    terrain.Height[player1.Position] + player1.GetTargetPosition(angle)[1])[1] - 2);
                angleblock.Text = angle.ToString();
            }
            if (e.Key == Key.A)
            {
                if (player1.Position > 0)
                {
                    player1.Position--;
                    Canvas.SetLeft(player1image, coord.ToInt(player1.Position, terrain.Height[player1.Position])[0] - 5);
                    Canvas.SetTop(player1image, coord.ToInt(player1.Position, terrain.Height[player1.Position])[1] - 5);
                    Canvas.SetLeft(player1target, coord.ToInt(player1.Position + player1.GetTargetPosition(angle)[0],
                        terrain.Height[player1.Position] + player1.GetTargetPosition(angle)[1])[0] - 2);
                    Canvas.SetTop(player1target, coord.ToInt(player1.Position + player1.GetTargetPosition(angle)[0],
                        terrain.Height[player1.Position] + player1.GetTargetPosition(angle)[1])[1] - 2);
                    angleblock.Text = angle.ToString();
                }
            }
            if (e.Key == Key.D)
            {
                if (player1.Position < terrainLength)
                {
                    player1.Position++;
                    Canvas.SetLeft(player1image, coord.ToInt(player1.Position, terrain.Height[player1.Position])[0] - 5);
                    Canvas.SetTop(player1image, coord.ToInt(player1.Position, terrain.Height[player1.Position])[1] - 5);
                    Canvas.SetLeft(player1target, coord.ToInt(player1.Position + player1.GetTargetPosition(angle)[0],
                        terrain.Height[player1.Position] + player1.GetTargetPosition(angle)[1])[0] - 2);
                    Canvas.SetTop(player1target, coord.ToInt(player1.Position + player1.GetTargetPosition(angle)[0],
                        terrain.Height[player1.Position] + player1.GetTargetPosition(angle)[1])[1] - 2);
                    angleblock.Text = angle.ToString();
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
