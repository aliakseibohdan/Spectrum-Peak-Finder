using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeakFinder
{
    public class Spectrum
    {
        private List<float> valuesX;
        public IReadOnlyList<float> ValuesX { get { return valuesX; } }

        private List<float> valuesY;
        public IReadOnlyList<float> ValuesY { get { return valuesY; } }

        public Spectrum()
        {
            valuesX = new List<float>();
            valuesY = new List<float>();
        }

        public void AddDataPoint(float x, float y)
        {
            valuesX.Add(x); 
            valuesY.Add(y);
        }
    }
}
