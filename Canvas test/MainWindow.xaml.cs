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
        static int timeInterval = 30;
        int wind;
        bool isStarted = false;
        Random rnd = new Random();
        Bullet bullet = null;
        int terrainLength = 1000;
        private int maxV = 100;
        public CanvasConvert coord;
        Ground terrain;
        int playerCount;
        public List<Tank> Players => players;
        List<Tank> removeList = new List<Tank>();
        bool hit = false;
        Tank activePlayer;
        private List<System.Windows.Media.Brush> _brushes;
        List<System.Windows.Point> pointList;
        ObservableCollection<Bullet> currentBullets = new ObservableCollection<Bullet>();
        int stepsCount = 0;
        int currentStep = 0;
        int step = 50;
        List<Player.PlayerType> playerTypes = new List<Player.PlayerType>();
        private List<Tank> players;

        public int MaxV { get => maxV; }
        public int Wind { get => wind; }

        public MainWindow()
        {
            InitializeComponent();
            playerTypes.Add(Player.PlayerType.Medium);
            playerTypes.Add(Player.PlayerType.Medium);
            playerTypes.Add(Player.PlayerType.Medium);
            playerTypes.Add(Player.PlayerType.Medium);
            playerTypes.Add(Player.PlayerType.Medium);
            playerTypes.Add(Player.PlayerType.Medium);
            playerTypes.Add(Player.PlayerType.Medium);
            playerTypes.Add(Player.PlayerType.Medium);
            playerTypes.Add(Player.PlayerType.Medium);
            playerTypes.Add(Player.PlayerType.Medium);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            timer.Elapsed += Timer_Elapsed;
            timer.Interval = timeInterval;
        }

        private async void GenerateNewGame()
        {
            isStarted = true;
            InitBrushes();
            coord = new CanvasConvert(background.ActualHeight, background.ActualWidth, terrainLength);
            terrain = new Ground(terrainLength, coord, ground);
            players = new List<Tank>();
            for (int i = 0; i < playerCount; i++)
            {
                Players.Add(new Tank(rnd.Next(10, terrainLength - 10), playerTypes[i].ToString() + (i + 1).ToString(), GetRandomBrush(), coord, terrain, playerTypes[i]));
                Players[i].MoveTank();
                Players[i].MoveTarget();
                Players[i].AddBullet(new Bullet(Bullet.BulletType.SmallBullet, 99));
                Players[i].AddBullet(new Bullet(Bullet.BulletType.BigBullet, 10));
                Players[i].AddBullet(new Bullet(Bullet.BulletType.Nuclear, 1));
                Players[i].AddBullet(new Bullet(Bullet.BulletType.Sniper, 5));
                Players[i].SelectedBullet = Players[i].Bullets[Bullet.BulletType.SmallBullet];
            }
            activePlayer = Players[0];
            background.DataContext = activePlayer;
            bulletSelector.ItemsSource = currentBullets;
            activePlayer.activeSign.Visibility = Visibility.Visible;
            activePlayer.target.Visibility = Visibility.Visible;
            Sky.GenerateClouds(10);
            GenerateWind();
            ListCurrentBullets();
            await AiMoveAsync();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.InvokeAsync(() =>
            {
                line.Points.Add(pointList[currentStep]);
                currentStep += step;
                if (currentStep >= stepsCount)
                {
                    timer.Stop();
                    SimulationFinished();
                }
            });
        }

        private async void SimulationFinished()
        {
            if (hit == true)
            {
                line.Points.Add(pointList[stepsCount - 1]);
                GenerateWind();
                terrain.DestroyTerrain(bullet.X, bullet.ExplosionDestroyDistance);
                activePlayer.MoveTank();
                activePlayer.MoveTarget();
                ShowExplosion(bullet.ExplosionRadius);
                foreach (Tank player in Players)
                {
                    player.MoveTank();
                    player.MoveTarget();
                }
                CheckDamage(bullet.X);
                bullet = null;
                activePlayer = NextPlayer();
                ListCurrentBullets();
                await AiMoveAsync();
            }
            else
            {
                GenerateWind();
                bullet = null;
                activePlayer = NextPlayer();
                ListCurrentBullets();
                await AiMoveAsync();
            }
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
            foreach (Tank player in Players)
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
                foreach (Tank player in removeList)
                {
                    Players.Remove(player);
                }
            }
            removeList.Clear();
            if (Players.Count == 1)
            {
                isStarted = false;
                MessageBox.Show($"{Players.First().Name} has won!");
                mainMenu.Visibility = Visibility.Visible;
            }
            if (Players.Count == 0)
            {
                isStarted = false;
                MessageBox.Show("Tie!");
                mainMenu.Visibility = Visibility.Visible;
            }
        }

        private Tank NextPlayer()
        {
            activePlayer.activeSign.Visibility = Visibility.Hidden;
            activePlayer.target.Visibility = Visibility.Hidden;
            int currentPlayerNumber = Players.IndexOf(activePlayer);
            if (currentPlayerNumber + 1 < playerCount)
            {
                Players[currentPlayerNumber + 1].activeSign.Visibility = Visibility.Visible;
                Players[currentPlayerNumber + 1].target.Visibility = Visibility.Visible;
                background.DataContext = Players[currentPlayerNumber + 1];
                activePlayer.SelectedBullet = Players[currentPlayerNumber + 1].Bullets[Bullet.BulletType.SmallBullet];
                return Players[currentPlayerNumber + 1];
            }
            else
            {
                Players[0].activeSign.Visibility = Visibility.Visible;
                Players[0].target.Visibility = Visibility.Visible;
                background.DataContext = Players[0];
                activePlayer.SelectedBullet = Players[0].Bullets[Bullet.BulletType.SmallBullet];
                return Players[0];
            }
        }

        private async Task AiMoveAsync()
        {
            int[] aiCoords;
            if (activePlayer.player.Type != Player.PlayerType.Human)
            {
                aiCoords = activePlayer.player.ChooseParameters();
                activePlayer.Velocity = aiCoords[1];
                activePlayer.Angle = aiCoords[2];
                if (aiCoords[0] == 0)
                {
                    activePlayer.Direction = 'r';
                }
                else
                {
                    activePlayer.Direction = 'l';
                }
                activePlayer.MoveTank();
                activePlayer.MoveTarget();
                await PutTaskDelay();
                Shot();
            }
        }

        async Task PutTaskDelay()
        {
            await Task.Delay(rnd.Next(500, 1501));
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
            if (bullet == null && isStarted && activePlayer.player.Type == Player.PlayerType.Human)
            {

                if (e.Key == Key.Space || e.Key == Key.Enter)
                {
                    if (activePlayer.SelectedBullet.BulletCount > 0)
                    {
                        Shot();
                    }
                    else
                    {
                        MessageBox.Show("No more bullets!");
                    }
                }
                if (e.Key == Key.Up)
                {
                    if (activePlayer.Velocity < MaxV)
                    {
                        activePlayer.Velocity += 4;
                        activePlayer.MoveTarget();
                    }
                }
                if (e.Key == Key.Down && activePlayer.Velocity > 0)
                {
                    if (activePlayer.Velocity > 0)
                    {
                        activePlayer.Velocity -= 4;
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
                    if (activePlayer.PositionX > 0 && Math.Abs(terrain.Height[(int)activePlayer.PositionX] - terrain.Height[(int)activePlayer.PositionX - 1]) < 5)
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
                    if (activePlayer.PositionX < terrainLength && Math.Abs(terrain.Height[(int)activePlayer.PositionX] - terrain.Height[(int)activePlayer.PositionX + 1]) < 5)
                    {
                        activePlayer.PositionX++;
                        activePlayer.MoveTank();
                    }
                }
                e.Handled = true;
            }
        }

        private void Shot()
        {
            bullet = activePlayer.SelectedBullet;
            activePlayer.SelectedBullet.BulletCount--;
            ListCurrentBullets();
            bullet.FireBullet(activePlayer.PositionX, activePlayer.PositionY + 1, activePlayer.Velocity, activePlayer.Angle, activePlayer.Direction);
            line.Points.Clear();
            hit = false;
            pointList = bullet.CalulateShot(coord, terrain, Wind, out hit);
            stepsCount = pointList.Count;
            currentStep = 0;
            timer.Start();
        }

        public delegate void WindGeneratedEventHandler(object sender, WindEventArgs e);

        public event WindGeneratedEventHandler WindGenerated;

        private void GenerateWind()
        {
            wind = rnd.Next(-5, 6);
            Dispatcher.Invoke(() =>
            {
                windBlock.Text = Wind.ToString();
            });
            OnWindGenerated(Wind);
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

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                playerCount = int.Parse(playerCountBox.Text);
                if (playerCount > 10)
                {
                    throw new ArgumentException("Too many players!");
                }
                mainMenu.Visibility = Visibility.Collapsed;
                GenerateNewGame();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Wrong player count!" + ex.Message);
            }
        }
    }

    public class WindEventArgs : EventArgs
    {
        public int Wind { get; set; }
    }
}
