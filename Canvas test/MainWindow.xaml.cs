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

namespace Canvas_test
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Timer timer = new Timer();
        int timeInterval = 1000 / 30;
        int wind = -3;
        Ball ball = new Ball(10, 10, 10,10);
        CanvasConvert convertCoordinates;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ball.CalculateNewPosition(wind, timeInterval);
            Dispatcher.Invoke(() =>
            {
                xBlock.Text = ball.X.ToString();
                yBlock.Text = ball.Y.ToString();
                vxBlock.Text = ball.SpeedX.ToString();
                vyBlock.Text = ball.SpeedY.ToString();
                line.Points.Add(convertCoordinates.CovertCoordinates(ball.X, ball.Y));
            });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            convertCoordinates = new CanvasConvert(background.ActualHeight, background.ActualWidth, 50);
            line.Points.Add(convertCoordinates.CovertCoordinates(ball.X, ball.Y));
            timer.Elapsed += Timer_Elapsed;
            timer.Interval = timeInterval;
            timer.Start();
        }
    }
}
