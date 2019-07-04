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
using System.Collections.ObjectModel;

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
        Bullet bullet = null;
        int terrainLength = 1000;
        int maxV = 50;
        public CanvasConvert coord;
        Ground terrain;
        int playerCount;
        List<Player> players;
        List<Player> removeList = new List<Player>();
        bool hit = false;
        bool inside;
        Player activePlayer;
        private List<System.Windows.Media.Brush> _brushes;
        ObservableCollection<Bullet> currentBullets = new ObservableCollection<Bullet>();

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
                players[i].AddBullet(new Bullet(Bullet.BulletType.SmallBullet, 99));
                players[i].AddBullet(new Bullet(Bullet.BulletType.BigBullet, 10));
                players[i].AddBullet(new Bullet(Bullet.BulletType.Nuclear, 1));
                players[i].SelectedBullet = players[i].Bullets[Bullet.BulletType.SmallBullet];
            }
            activePlayer = players[0];
            background.DataContext = activePlayer;
            bulletSelector.ItemsSource = currentBullets;
            activePlayer.activeSign.Visibility = Visibility.Visible;
            activePlayer.target.Visibility = Visibility.Visible;
            timer.Elapsed += Timer_Elapsed;
            timer.Interval = timeInterval;
            Sky.GenerateClouds(10);
            GenerateWind();
            ListCurrentBullets();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            bullet.CalculateNewPosition(wind, timeInterval);
            Dispatcher.InvokeAsync(() =>
            {
                xBlock.Text = bullet.X.ToString();
                yBlock.Text = bullet.Y.ToString();
                vxBlock.Text = bullet.SpeedX.ToString();
                vyBlock.Text = bullet.SpeedY.ToString();
                line.Points.Add(coord.ToWindowsPoint(bullet.X, bullet.Y));
                hit = terrain.CheckHit(bullet.X, bullet.Y);
                inside = bullet.Y >= 0;
                if (hit == true)
                {
                    timer.Stop();
                    GenerateWind();
                    terrain.DestroyTerrain(bullet.X, bullet.ExplosionDestroyDistance);
                    activePlayer.MoveTank();
                    activePlayer.MoveTarget();
                    ShowExplosion(bullet.ExplosionRadius);
                    foreach (Player player in players)
                    {
                        player.MoveTank();
                        player.MoveTarget();
                    }
                    CheckDamage(bullet.X);
                    bullet = null;
                    activePlayer = NextPlayer();
                    ListCurrentBullets();
                }
                if (inside == false)
                {
                    timer.Stop();
                    GenerateWind();
                    bullet = null;

                    activePlayer = NextPlayer();
                    ListCurrentBullets();
                }
            });
        }

        private void ListCurrentBullets()
        {
            Dispatcher.Invoke(() =>
            {
                currentBullets.Clear();
                foreach (KeyValuePair<Bullet.BulletType, Bullet> bullet in activePlayer.Bullets)
                {
                    currentBullets.Add(bullet.Value);
                }
            });
        }

        private void CheckDamage(double x)
        {
            bool isAlive;
            foreach (Player player in players)
            {
                isAlive = player.CheckIfAlive(x, bullet.Damage, bullet.ExplosionDestroyDistance);
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

        private Player NextPlayer()
        {
            activePlayer.activeSign.Visibility = Visibility.Hidden;
            activePlayer.target.Visibility = Visibility.Hidden;
            int currentPlayerNumber = players.IndexOf(activePlayer);
            if (currentPlayerNumber + 1 < playerCount)
            {
                players[currentPlayerNumber + 1].activeSign.Visibility = Visibility.Visible;
                players[currentPlayerNumber + 1].target.Visibility = Visibility.Visible;
                background.DataContext = players[currentPlayerNumber + 1];
                return players[currentPlayerNumber + 1];
            }
            else
            {
                players[0].activeSign.Visibility = Visibility.Visible;
                players[0].target.Visibility = Visibility.Visible;
                background.DataContext = players[0];
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
            Canvas.SetLeft(explosion, coord.ToInt(bullet.X, bullet.Y)[0] - explosion.Width / 2);
            Canvas.SetTop(explosion, coord.ToInt(bullet.X, bullet.Y)[1] - explosion.Height / 2);
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
            if (bullet == null)
            {

                if (e.Key == Key.Space || e.Key == Key.Enter)
                {
                    if (activePlayer.SelectedBullet.BulletCount > 0)
                    {
                        bullet = activePlayer.SelectedBullet;
                        activePlayer.SelectedBullet.BulletCount--;
                        ListCurrentBullets();
                        bullet.FireBullet(activePlayer.PositionX, activePlayer.PositionY + 1, activePlayer.Velocity, activePlayer.Angle, activePlayer.Direction);
                        line.Points.Clear();
                        hit = false;
                        inside = true;
                        line.Points.Add(coord.ToWindowsPoint(bullet.X, bullet.Y));
                        timer.Start();
                    }
                    else
                    {
                        MessageBox.Show("No more bullets!");
                    }
                }
                if (e.Key == Key.Up)
                {
                    if (activePlayer.Velocity < maxV)
                    {
                        activePlayer.Velocity++;
                        activePlayer.MoveTarget();
                    }
                }
                if (e.Key == Key.Down && activePlayer.Velocity > 0)
                {
                    if (activePlayer.Velocity > 0)
                    {
                        activePlayer.Velocity--;
                        activePlayer.MoveTarget();
                    }
                }
                if (e.Key == Key.Left)
                {
                    if (activePlayer.Angle > -90)
                    {
                        activePlayer.Angle -= 2;
                        activePlayer.MoveTarget();
                    }
                }
                if (e.Key == Key.Right)
                {
                    if (activePlayer.Angle < 90)
                    {
                        activePlayer.Angle += 2;
                        activePlayer.MoveTarget();
                    }
                }
                if (e.Key == Key.A)
                {
                    if (activePlayer.Direction == 'r')
                    {
                        activePlayer.Direction = 'l';
                    }
                    if (activePlayer.PositionX > 0)
                    {
                        activePlayer.PositionX--;
                        activePlayer.MoveTank();
                    }
                }
                if (e.Key == Key.D)
                {
                    if (activePlayer.Direction == 'l')
                    {
                        activePlayer.Direction = 'r';
                    }
                    if (activePlayer.PositionX < terrainLength)
                    {
                        activePlayer.PositionX++;
                        activePlayer.MoveTank();
                    }
                }
            }
        }

        public delegate void WindGeneratedEventHandler(object sender, WindEventArgs e);

        public event WindGeneratedEventHandler WindGenerated;

        private void GenerateWind()
        {
            wind = rnd.Next(-5, 6);
            Dispatcher.Invoke(() =>
            {
                windBlock.Text = wind.ToString();
            });
            OnWindGenerated(wind);
        }

        public virtual void OnWindGenerated(int wind)
        {
            if (WindGenerated != null)
            {
                WindGenerated(this, new WindEventArgs() { Wind = wind });
            }
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

        private void BulletSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            activePlayer.SelectedBullet = bulletSelector.SelectedItem as Bullet;
        }
    }

    public class WindEventArgs : EventArgs
    {
        public int Wind { get; set; }
    }
}
