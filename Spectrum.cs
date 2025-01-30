using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeakFinder
{
    public class Spectrum
    {
        public double[] ValuesX { get; private set; }
        public double[] ValuesY { get; private set; }

        public Spectrum(int size)
        {
            if (size <= 0)
            {
                throw new ArgumentException("Size must be greater than zero.", nameof(size));
            }

            ValuesX = new double[size];
            ValuesY = new double[size];
        }

        public void AddDataPoint(double x, double y)
        {
            int index = Array.FindIndex(ValuesX, value => value == 0);
            if (index == -1)
            {
                throw new InvalidOperationException("The spectrum arrays are full. Cannot add more data points.");
            }

            ValuesX[index] = x;
            ValuesY[index] = y;
        }

        public void Resize(int newSize)
        {
            if (newSize <= ValuesX.Length)
            {
                throw new ArgumentException("New size must be greater than the current size.", nameof(newSize));
            }

            Array.Resize(ref ValuesX, newSize);
            Array.Resize(ref ValuesY, newSize);
        }

        public List<(double, double)> GetDataPoints()
        {
            var dataPoints = new List<(double, double)>();

            for (int i = 0; i < ValuesX.Length; i++)
            {
                if (ValuesX[i] != 0)
                {
                    dataPoints.Add((ValuesX[i], ValuesY[i]));
                }
            }

            return dataPoints;
        }
    }
}
