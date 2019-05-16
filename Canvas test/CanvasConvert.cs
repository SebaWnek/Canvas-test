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
        public Point ToWindowsPoint(double x, double y)
        {
            double resultX;
            double resultY;
            resultX = x * conversionRate;
            resultY = canvasHeigth - y * conversionRate;
            return new Point(resultX, resultY);
        }
        public int[] ToInt(double x, double y)
        {
            int resultX;
            int resultY;
            resultX = (int)(x * conversionRate);
            resultY = (int)(canvasHeigth - y * conversionRate);
            return new int[] { resultX, resultY };
        }
        public System.Drawing.Point ToDrawingPoint(double x, double y)
        {
            int resultX;
            int resultY;
            resultX = (int)(x * conversionRate);
            resultY = (int)(canvasHeigth - y * conversionRate);
            return new System.Drawing.Point(resultX, resultY);
        }
    }
}
