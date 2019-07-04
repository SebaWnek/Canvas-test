using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Controls;

namespace Canvas_test
{
    class Cloud
    {
        int animationSpeed = 5;
        private int size;
        private int scale = 50;
        private double wind = 0;
        private int positionY;
        private double positionX;
        private double baseSpeed;

        CanvasConvert coord;
        Ground terrain;

        static Random rnd = new Random();

        Ellipse elipse;

        MainWindow window = Application.Current.MainWindow as MainWindow;

        public Cloud()
        {
            size = rnd.Next(8);
            baseSpeed = 20 - 2 * size;
            window.WindGenerated += OnWindGenerated;
            coord = window.coord;
            positionY = rnd.Next(200, (int)coord.FieldHeigth + 100);
            positionX = rnd.Next((int)coord.FieldWidth);

            elipse = new Ellipse();
            elipse.Height = scale * size;
            elipse.Width = scale * size;
            elipse.Fill = System.Windows.Media.Brushes.White;
            Canvas.SetZIndex(elipse, -1);
            Canvas.SetLeft(elipse, coord.ToInt(positionX, positionY)[0]);
            Canvas.SetTop(elipse, coord.ToInt(positionX, positionY)[1]);

            Sky.timer.Elapsed += Timer_Elapsed;
            window.background.Children.Add(elipse);
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            positionX += wind * animationSpeed * baseSpeed * 30 / 1000;
            if(positionX < -200)
            {
                positionX = (int)coord.FieldWidth + 100;
            }
            if(positionX > (int)coord.FieldWidth + 100)
            {
                positionX = -200;
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                Canvas.SetLeft(elipse, coord.ToInt(positionX, positionY)[0]);
            });
        }

        public void OnWindGenerated(object sender, WindEventArgs e)
        {
            wind = e.Wind;
        }
    }
}
