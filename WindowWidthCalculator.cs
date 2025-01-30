using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeakFinder
{
    internal class WindowWidthCalculator
    {
        public static int GetFullWidth(int windowHalfWidth)
        {
            return windowHalfWidth >= 0
                ? windowHalfWidth * 2 + 1
                : throw new ArgumentException($"{nameof(windowHalfWidth)} must be positive.");
        }
    }
}
