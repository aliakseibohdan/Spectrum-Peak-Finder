using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeakFinder
{
    public struct SecondDegreePolynomialCoefficients
    {
        private const int MinPointsNumberForSecondDegreeInterpolation = 3;
        private const double DetValueZeroPrecision = 1e-35;

        private readonly double xSum;
        private readonly double ySum;
        private readonly double yxSum;
        private readonly double xSPSum;
        private readonly double yxSPSum;
        private readonly double xTPSum;
        private readonly double xFPSum;
        private readonly double det;
        private readonly int length;

        private double? a0;
        /// <summary>
        /// Zero degree variable coefficient A0.
        /// </summary>
        public double A0
        {
            get
            {
                if (a0.HasValue)
                    return a0.Value;

                a0 = xTPSum.Pow2() * ySum + xSPSum.Pow2() * yxSPSum - xTPSum * (xSum * yxSPSum + xSPSum * yxSum) +
                    xFPSum * (-xSPSum * ySum + xSum * yxSum);
                a0 /= det;
                return a0.Value;
            }
        }

        private double? a1;
        /// <summary>
        /// First degree variable coefficient A1 * x.
        /// </summary>
        public double A1
        {
            get
            {
                if (a1.HasValue)
                    return a1.Value;

                a1 = length * xTPSum * yxSPSum - xSPSum * (xTPSum * ySum + xSum * yxSPSum) + xSPSum.Pow2() * yxSum +
                    xFPSum * (xSum * ySum - length * yxSum);
                a1 /= det;
                return a1.Value;
            }
        }

        private double? a2;
        /// <summary>
        /// Second degree variable coefficient A2 * x ^ 2.
        /// </summary>
        public double A2
        {
            get
            {
                if (a2.HasValue)
                    return a2.Value;

                a2 = xSPSum.Pow2() * ySum - xSum * xTPSum * ySum + xSum.Pow2() * yxSPSum + length * xTPSum * yxSum -
                    xSPSum * (length * yxSPSum + xSum * yxSum);
                a2 /= det;
                return a2.Value;
            }
        }

        public SecondDegreePolynomialCoefficients(
            double xSum, double ySum, double yxSum, double xSPSum, double yxSPSum, double xTPSum, double xFPSum, double det, int length)
        {
            this.xSum = xSum;
            this.ySum = ySum;
            this.yxSum = yxSum;
            this.xSPSum = xSPSum;
            this.yxSPSum = yxSPSum;
            this.xTPSum = xTPSum;
            this.xFPSum = xFPSum;
            this.det = det;
            this.length = length;
        }

        /// <summary>
        /// Get second degree polynomial coefficients for provided data.
        /// </summary>
        /// <param name="x">Provided data values positions.</param>
        /// <param name="y">Provided data values.</param>
        /// <param name="startIndex">Zero based start data used for interpolation index.</param>
        /// <param name="length">Data points count used for interpolation.</param>
        /// <returns>Second degree </returns>
        /// <exception cref="ArgumentException">If data values and data positions sets are of unequal length or
        /// if points number is not enough (lesser than 3) to fit second degree polynomial.</exception>
        public static SecondDegreePolynomialCoefficients? CreateApproximatedPolynomial(
            double[] x, double[] y, int startIndex, int length)
        {
            if (x.Length != y.Length)
                throw new ArgumentException("X and Y values sets must be of equal length;");
            if (x.Length - startIndex < length)
                throw new ArgumentException("Start index is to big for such length. Index will be outside of the range.");
            if (length < MinPointsNumberForSecondDegreeInterpolation)
                throw new ArgumentException("Minimal points number for second degree polynomial interpolation is 3.");
            double xSum = 0;
            double ySum = 0;
            double yxSum = 0;
            double xSPSum = 0;
            double yxSPSum = 0;
            double xTPSum = 0;
            double xFPSum = 0;

            int m = startIndex + length;
            for (int i = startIndex; i < m; i++)
            {
                double tempX = x[i];
                double tempY = y[i];
                xSum += tempX;
                ySum += tempY;
                yxSum += tempY * tempX;
                double tempXSP = tempX.Pow2();
                xSPSum += tempXSP;
                yxSPSum += tempY * tempXSP;
                xTPSum += tempXSP * tempX;
                xFPSum += tempXSP.Pow2();
            }
            double det = xSPSum.Pow3() + xFPSum * xSum.Pow2() - 2 * xSPSum * xSum * xTPSum + length * (-xFPSum * xSPSum + xTPSum.Pow2());
            if (!det.IsCloseToZero(DetValueZeroPrecision))
                return new SecondDegreePolynomialCoefficients(xSum, ySum, yxSum, xSPSum, yxSPSum, xTPSum, xFPSum, det, length);
            //Points can not be approximated as second degree polynomial.
            return null;
        }

        /// <summary>
        /// Get interpolation data region start index and length around pivot point index.
        /// </summary>
        /// <param name="start">Data region of interest start index.</param>
        /// <param name="end">Data region of interest end index.</param>
        /// <param name="pivotPointIndex">Pivot data point index.</param>
        /// <param name="windowHalfWidth">Interpolation region half width.</param>
        /// <param name="approximationStart">Interpolation region start index.</param>
        /// <param name="approximationLength">Interpolation region length.</param>
        /// <returns>If possible to select points for interpolation around pivot point.</returns>
        public static bool TryGetApproximationPointsAroundPivotPoint(
            int start, int end, int pivotPointIndex, int windowHalfWidth, out int approximationStart, out int approximationLength)
        {
            approximationStart = 0;
            approximationLength = 0;
            if (windowHalfWidth < 1)
                return false;
            approximationLength = WindowWidthCalculator.GetFullWidth(windowHalfWidth);
            if (end - start + 1 < approximationLength)
                return false;
            approximationStart = pivotPointIndex - windowHalfWidth;
            approximationStart = approximationStart < start ? start : approximationStart;
            if (approximationStart + approximationLength - 1 > end)
                approximationStart = end - approximationLength + 1;
            return true;
        }
    }
}
