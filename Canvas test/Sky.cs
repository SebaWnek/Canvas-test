using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;

namespace Canvas_test
{
    static class Sky
    {
        static List<Cloud> clouds = new List<Cloud>();

        public static Timer timer;

        public static void GenerateClouds(int count)
        {
            timer = new Timer();
            timer.AutoReset = true;
            timer.Interval = 1000 / 30;
            timer.Start();
            clouds.Clear();
            for (int i = 0; i < count; i++)
            {
                clouds.Add(new Cloud());
            }
        }
        public static void AddCloud()
        {
            clouds.Add(new Cloud());
        }
    }
}
