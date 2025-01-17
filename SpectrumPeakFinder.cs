using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeakFinder
{
    public class SpectrumPeakFinder
    {
        public static IReadOnlyList<int> FindPeaks(Spectrum spectrum)
        {
            int n = spectrum.ValuesX.Count;
            IReadOnlyList<int> peakIndices = new List<int>().AsReadOnly();

            if (n < 3)
            {
                Console.WriteLine("Not enough data points to find peaks.");
                return peakIndices;
            }

            float[] secondDerivative = new float[n];

            // Compute second derivative using finite differences
            for (int i = 1; i < n - 1; i++)
            {
                secondDerivative[i] = spectrum.ValuesY[i - 1] - 2 * spectrum.ValuesY[i] + spectrum.ValuesY[i + 1];
            }

            for (int i = 1; i < n - 1; i++)
            {
                if (secondDerivative[i - 1] > 0 && secondDerivative[i] < 0)
                {
                    peakIndices.Append(i);
                }
            }

            return peakIndices;
        }
    }
}
