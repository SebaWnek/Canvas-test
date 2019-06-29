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
using System.Reflection;

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
        Ball ball = null;
        int terrainLength = 1000;
        int maxV = 50; 
        CanvasConvert coord;
        Ground terrain;
        int playerCount;
        List<Player> players;
        List<Player> removeList = new List<Player>();
        bool hit = false;
        bool inside;
        Player activePlayer;
        private List<System.Windows.Media.Brush> _brushes;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitBrushes();
            coord = new CanvasConvert(background.ActualHeight, background.ActualWidth, terrainLength);
            terrain = new Ground(terrainLength, coord, ground);
            playerCount = 2;
            players = new List<Player>();
            for (int i = 0; i < playerCount; i++)
            {
                players.Add(new Player(rnd.Next(10, terrainLength - 10), "player" + (i + 1).ToString(), GetRandomBrush(), coord, terrain));
                players[i].MoveTank();
                players[i].MoveTarget();
            }
            activePlayer = players[0];
            activePlayer.activeSign.Visibility = Visibility.Visible;
            activePlayer.target.Visibility = Visibility.Visible;
            timer.Elapsed += Timer_Elapsed;
            timer.Interval = timeInterval;
            angleblock.Text = activePlayer.angle.ToString();
            vblock.Text = activePlayer.Velocity.ToString();
            GenerateWind();
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
                    terrain.DestroyTerrain(ball.X, activePlayer.Damage);
                    activePlayer.MoveTank();
                    activePlayer.MoveTarget();
                    ShowExplosion(150);
                    foreach(Player player in players)
                    {
                        player.MoveTank();
                        player.MoveTarget();
                    }
                    CheckDamage(ball.X);
                    ball = null;
                    activePlayer = nextPlayer();
                }
                if (inside == false)
                {
                    timer.Stop();
                    GenerateWind();
                    ball = null;

                    activePlayer = nextPlayer();
                }
            });
        }

        private void CheckDamage(double x)
        {
            bool isAlive;
            foreach(Player player in players)
            {
                isAlive = player.CheckIfAlive(x, activePlayer.Damage);
                if (!isAlive)
                {
                    player.RemovePlayer();
                    playerCount--;
                    removeList.Add(player);
                }
            }
            if (removeList.Count > 0)
            {
                foreach (Player player in removeList)
                {
                    players.Remove(player);
                }
            }
            removeList.Clear();
            if (players.Count == 1)
            {
                MessageBox.Show($"{players.First().Name} has won!");
                Environment.Exit(1);
            }
            if (players.Count == 0)
            {
                MessageBox.Show("Tie!");
                Environment.Exit(1);
            }
        }

        private Player nextPlayer()
        {
            activePlayer.activeSign.Visibility = Visibility.Hidden;
            activePlayer.target.Visibility = Visibility.Hidden;
            int currentPlayerNumber = players.IndexOf(activePlayer);
            if (currentPlayerNumber + 1 < playerCount)
            {
                players[currentPlayerNumber + 1].activeSign.Visibility = Visibility.Visible;
                players[currentPlayerNumber + 1].target.Visibility = Visibility.Visible;
                return players[currentPlayerNumber + 1];
            }
            else
            {
                players[0].activeSign.Visibility = Visibility.Visible;
                players[0].target.Visibility = Visibility.Visible;
                return players[0];
            }
        }

        private void ShowExplosion(int radius)
        {
            Ellipse explosion = new Ellipse();
            explosion.Height = radius;
            explosion.Width = radius;
            explosion.Stroke = System.Windows.Media.Brushes.Orange;
            explosion.Fill = System.Windows.Media.Brushes.Red;
            AddToBackgroud(explosion);
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
            animation.Completed += (s, a) => { background.Children.Remove(explosion); };
            explosion.BeginAnimation(UIElement.OpacityProperty, animation);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (ball == null)
            {

                if (e.Key == Key.Space)
                {
                    ball = new Ball(activePlayer.PositionX, activePlayer.PositionY + 1, activePlayer.Velocity, activePlayer.angle, activePlayer.Direction);
                    line.Points.Clear();
                    hit = false;
                    inside = true;
                    line.Points.Add(coord.ToWindowsPoint(ball.X, ball.Y));
                    timer.Start();
                }
                if (e.Key == Key.Up)
                {
                    if (activePlayer.Velocity < maxV)
                    {
                        activePlayer.Velocity++;
                        vblock.Text = activePlayer.Velocity.ToString();
                        activePlayer.MoveTarget();
                    }
                }
                if (e.Key == Key.Down && activePlayer.Velocity > 0)
                {
                    if (activePlayer.Velocity > 0)
                    {
                        activePlayer.Velocity--;
                        vblock.Text = activePlayer.Velocity.ToString();
                        activePlayer.MoveTarget();
                    }
                }
                if (e.Key == Key.Left)
                {
                    activePlayer.angle--;
                    activePlayer.MoveTarget();
                }
                if (e.Key == Key.Right)
                {
                    activePlayer.angle++;
                    activePlayer.MoveTarget();
                }
                if (e.Key == Key.A)
                {
                    if (activePlayer.PositionX > 0)
                    {
                        activePlayer.PositionX--;
                        activePlayer.MoveTank();
                    }
                }
                if (e.Key == Key.D)
                {
                    if (activePlayer.PositionX < terrainLength)
                    {
                        activePlayer.PositionX++;
                        activePlayer.MoveTank();
                    }
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

        public void AddToBackgroud(UIElement element)
        {
            background.Children.Add(element);
        }
        public void RemoveFromBackground(UIElement element)
        {
            background.Children.Remove(element);
        }

        private void InitBrushes()
        {
            _brushes = new List<System.Windows.Media.Brush>();
            var props = typeof(System.Windows.Media.Brushes).GetProperties(BindingFlags.Public | BindingFlags.Static);
            foreach (var propInfo in props)
            {
                _brushes.Add((System.Windows.Media.Brush)propInfo.GetValue(null, null));
            }
        }
        private System.Windows.Media.Brush GetRandomBrush()
        {
            return _brushes[rnd.Next(_brushes.Count)];
        }
    }
}
