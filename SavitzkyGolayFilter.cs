using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeakFinder
{
    public class SavitzkyGolayFilter
    {
        public static Spectrum ApplySavitzkyGolayFilter(Spectrum spectrum, int windowSize, int polyOrder)
        {
            if (windowSize % 2 == 0 || windowSize <= polyOrder)
            {
                throw new ArgumentException("Window size must be odd and greater than polynomial order.");
            }

            int halfWindow = windowSize / 2;
            int dataLength = spectrum.ValuesX.Count;
            Spectrum smoothedSpectrum = new Spectrum();

            float[] coefficients = GetSavitzkyGolayCoefficients(windowSize, polyOrder);

            for (int i = 0; i < dataLength; i++)
            {
                float sum = 0;
                for (int j = -halfWindow; j <= halfWindow; j++)
                {
                    int index = i + j;
                    if (index >= 0 && index < dataLength)
                    {
                        sum += spectrum.ValuesY[index] * coefficients[j + halfWindow];
                    }
                }
                smoothedSpectrum.AddDataPoint(spectrum.ValuesX[i], sum);
            }

            return smoothedSpectrum;
        }

        private static float[] GetSavitzkyGolayCoefficients(int windowSize, int polyOrder)
        {
            int halfWindow = windowSize / 2;
            int order = polyOrder + 1;
            float[] coefficients = new float[windowSize];

            float[,] mat = new float[order, order];
            float[] rhs = new float[order];

            for (int i = 0; i < order; i++)
            {
                for (int j = 0; j < order; j++)
                {
                    mat[i, j] = MathF.Pow(halfWindow, i + j);
                }
                rhs[i] = 0.0f;
            }

            for (int i = 0; i < order; i++)
            {
                coefficients[i] = 0.0f;
                for (int j = 0; j < order; j++)
                {
                    coefficients[i] += mat[i, j];
                }
            }

            return coefficients;
        }
    }
}
