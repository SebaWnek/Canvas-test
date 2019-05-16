using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Canvas_test
{
    class CanvasConvert
    {
        private double canvasHeigth;
        private double canvasWidth;
        private double fieldHeigth;
        private double fieldWidth;
        private double conversionRate;
        public CanvasConvert(double heigth, double width, double length)
        {
            canvasHeigth = heigth;
            canvasWidth = width;
            fieldWidth = length;
            conversionRate = canvasWidth / fieldWidth;
            fieldHeigth = canvasHeigth / conversionRate;
        }
        public Point CovertCoordinates(double x, double y)
        {
            double resultX;
            double resultY;
            resultX = y * conversionRate;
            resultY = canvasHeigth - x * conversionRate;
            return new Point(resultX, resultY);
        }
    }
}
