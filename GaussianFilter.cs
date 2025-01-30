using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeakFinder
{
    public class GaussianFilter
    {
        public static double[] ApplyGaussianFilter(double[] valuesX, double[] valuesY, double sigma)
        {
            int length = valuesY.Length;
            double[] smoothedY = new double[length];

            // Define the Gaussian kernel
            int kernelSize = (int)(6 * sigma);
            kernelSize = Math.Max(kernelSize, 3);
            double[] gaussianKernel = new double[kernelSize];

            // Compute the Gaussian kernel
            double sum = 0;
            for (int i = 0; i < kernelSize; i++)
            {
                double x = i - kernelSize / 2;
                gaussianKernel[i] = Math.Exp(-0.5 * (x * x) / (sigma * sigma));
                sum += gaussianKernel[i];
            }

            // Normalize the Gaussian kernel
            for (int i = 0; i < kernelSize; i++)
            {
                gaussianKernel[i] /= sum;
            }

            // Apply the filter by convolving with the Gaussian kernel
            for (int i = 0; i < length; i++)
            {
                double smoothedValue = 0;

                // Convolution: Sum the weighted values from the neighborhood
                for (int j = 0; j < kernelSize; j++)
                {
                    int index = i + j - kernelSize / 2;

                    // Ensure we are within the bounds of the array
                    if (index >= 0 && index < length)
                    {
                        smoothedValue += valuesY[index] * gaussianKernel[j];
                    }
                }

                smoothedY[i] = smoothedValue;
            }

            return smoothedY;
        }
    }
}
