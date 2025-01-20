using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeakFinder
{
    public class SpectrumPeakFinder
    {
        /// <summary>
        /// Finds peaks in a spectrum by calculating the second derivative using finite differences
        /// </summary>
        /// <param name="spectrum"> Smoothing should be applied beforehand </param>
        /// <returns> A list of x-axis values corresponding to the peaks; an empty list if there are not enough points to find the peaks </returns>
        public static IReadOnlyList<int> FindPeaks(Spectrum spectrum)
        {
            int n = spectrum.ValuesX.Count;
            List<int> peakIndices = new List<int>();

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

            // Identify peaks based on the second derivative
            for (int i = 1; i < n - 1; i++)
            {
                if (secondDerivative[i - 1] > 0 && secondDerivative[i] < 0)
                {
                    peakIndices.Add(i);
                }
            }

            return peakIndices.AsReadOnly();
        }
    }
}
