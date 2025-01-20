using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeakFinder
{
    public class SavitzkyGolayFilter
    {
        // Bibliographic Reference:
        // Savitzky, A., & Golay, M. J. E. (1964). "Smoothing and Differentiation of Data by Simplified Least Squares Procedures." Analytical Chemistry, 36(8), 1627-1639
        // Golub, G. H., & Van Loan, C. F. (1996). Matrix Computations (3rd Edition). Johns Hopkins University Press.

        /// <summary>
        /// Smooths the spectrum by fitting a polynomial of a given order to a window of data points and then computing a weighted sum of the data points using the filter coefficients
        /// </summary>
        /// <param name="spectrum"> The input spectrum to be smoothed </param>
        /// <param name="windowSize"> The size of the window used for smoothing. It must be odd and greater than polynomial order; otherwise exception is thrown </param>
        /// <param name="polyOrder"> The order of the polynomial used for the smoothing. It defines the degree of the polynomial used in the local fit </param>
        /// <returns> A new <see cref="Spectrum"/> object where the smoothed data is stored </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public static Spectrum ApplySavitzkyGolayFilter(Spectrum spectrum, int windowSize, int polynomialOrder)
        {
            double[] x = spectrum.ValuesX.Select(v => (double)v).ToArray();
            double[] y = spectrum.ValuesY.Select(v => (double)v).ToArray();

            if (windowSize % 2 == 0 || windowSize <= polynomialOrder)
            {
                throw new ArgumentException("Window size must be odd and greater than polynomial order.");
            }

            int halfWindow = (windowSize - 1) / 2;
            int n = y.Length;
            double[] smoothedY = new double[n];

            // Generate the matrix for the least squares polynomial fitting (A matrix)
            double[,] A = new double[windowSize, polynomialOrder + 1];
            for (int i = -halfWindow; i <= halfWindow; i++)
            {
                for (int j = 0; j <= polynomialOrder; j++)
                {
                    A[i + halfWindow, j] = Math.Pow(i, j);
                }
            }

            double[,] ATA = MultiplyMatrices(TransposeMatrix(A), A);
            double[,] ATAInverse = Inverse(ATA);

            // Apply Savitzky-Golay filter
            for (int i = halfWindow; i < n - halfWindow; i++)
            {
                double[] windowY = new double[windowSize];
                for (int j = -halfWindow; j <= halfWindow; j++)
                {
                    windowY[j + halfWindow] = y[i + j];
                }

                double smoothedValue = 0;

                // Matrix-vector multiplication
                for (int row = 0; row < windowSize; row++)
                {
                    double dotProduct = 0;
                    for (int col = 0; col < windowSize; col++)
                    {
                        dotProduct += ATAInverse[row, col] * windowY[col]; // <- IndexOutOfRangeException
                    }
                    smoothedValue += dotProduct;
                }

                smoothedY[i] = smoothedValue;
            }

            Spectrum smoothedSpectrum = new Spectrum();
            for (int i = 0; i < n; i++)
            {
                smoothedSpectrum.AddDataPoint(spectrum.ValuesX[i], (float)smoothedY[i]);
            }

            return smoothedSpectrum;
        }


        private static double[,] MultiplyMatrices(double[,] A, double[,] B)
        {
            int rowsA = A.GetLength(0);
            int colsA = A.GetLength(1);
            int colsB = B.GetLength(1);

            double[,] result = new double[rowsA, colsB];

            for (int i = 0; i < rowsA; i++)
            {
                for (int j = 0; j < colsB; j++)
                {
                    result[i, j] = 0;
                    for (int k = 0; k < colsA; k++)
                    {
                        result[i, j] += A[i, k] * B[k, j];
                    }
                }
            }

            return result;
        }

        private static double[,] TransposeMatrix(double[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            double[,] result = new double[cols, rows];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    result[j, i] = matrix[i, j];
                }
            }

            return result;
        }

        private static double[,] Inverse(double[,] matrix)
        {
            int n = matrix.GetLength(0);
            var LU = LU_Decompose(matrix);
            double[,] L = LU.Item1;
            double[,] U = LU.Item2;

            double[,] inverse = new double[n, n];

            for (int i = 0; i < n; i++)
            {
                double[] b = new double[n];
                b[i] = 1;  // Identity matrix column
                double[] column = SolveLU(L, U, b);

                for (int j = 0; j < n; j++)
                {
                    inverse[j, i] = column[j];
                }
            }

            return inverse;
        }

        // LU decomposition using Gaussian elimination
        private static Tuple<double[,], double[,]> LU_Decompose(double[,] matrix)
        {
            int n = matrix.GetLength(0);
            double[,] L = new double[n, n];
            double[,] U = (double[,])matrix.Clone();

            for (int i = 0; i < n; i++)
            {
                L[i, i] = 1;
                for (int j = i + 1; j < n; j++)
                {
                    double multiplier = U[j, i] / U[i, i];
                    L[j, i] = multiplier;
                    for (int k = i; k < n; k++)
                    {
                        U[j, k] -= multiplier * U[i, k];
                    }
                }
            }

            return Tuple.Create(L, U);
        }

        // Forward and backward substitution for solving linear systems
        private static double[] SolveLU(double[,] L, double[,] U, double[] b)
        {
            int n = b.Length;
            double[] y = new double[n];

            // Forward substitution: solve Ly = b
            for (int i = 0; i < n; i++)
            {
                y[i] = b[i];
                for (int j = 0; j < i; j++)
                {
                    y[i] -= L[i, j] * y[j];
                }
            }

            double[] x = new double[n];

            // Backward substitution: solve Ux = y
            for (int i = n - 1; i >= 0; i--)
            {
                x[i] = y[i];
                for (int j = i + 1; j < n; j++)
                {
                    x[i] -= U[i, j] * x[j];
                }
                x[i] /= U[i, i];
            }

            return x;
        }
    }
}
