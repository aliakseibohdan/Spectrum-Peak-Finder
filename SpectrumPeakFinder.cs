using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeakFinder
{
    public class SpectrumPeakFinder
    {
        public static List<double> SearchPeaks(double[] positions,
                                               double[] values,
                                               int windowHalfWidth)
        {
            ValidateInputData(positions, values, windowHalfWidth);

            int totalLength = positions.Length;
            int windowWidth = WindowWidthCalculator.GetFullWidth(windowHalfWidth);

            if (totalLength < windowWidth)
                throw new ArgumentException($"{nameof(windowHalfWidth)} is too big for such input data.");

            var result = new List<double>();
            var scaledPositions = ScalePositions(positions);

            double[] secondDerivation = GetApproximatedSecondDerivation(scaledPositions, values, windowHalfWidth);
            double[] firstDerivationOfSecondDerivation = GetApproximatedFirstDerivation(scaledPositions, secondDerivation, windowHalfWidth);

            int totalLengthMinusOne = totalLength - 1;
            int i = 0;

            while (i < totalLengthMinusOne)
            {
                // Skip non-negative second derivations (descending slope)
                while (i < totalLength && secondDerivation[i] >= 0)
                    i++;

                int start = i;

                // Find the end of the peak (positive second derivation indicates ascending slope)
                while (i < totalLength && secondDerivation[i] < 0)
                    i++;

                int peakDataLength = i - start;

                // Skip small peaks
                if (peakDataLength < 3)
                    continue;

                var negativeSecondDerivationRegion = DataSection.CreateRange(start, peakDataLength);

                if (TryGetPeakPosition(
                    scaledPositions,
                    secondDerivation,
                    firstDerivationOfSecondDerivation,
                    negativeSecondDerivationRegion,
                    windowHalfWidth,
                    out double peakPosition,
                    out int minSecondDerivationIndex))
                {
                    result.Add(peakPosition);
                }
            }

            return result;
        }

        private static void ValidateInputData(double[] positions, double[] values, int windowHalfWidth)
        {
            if (positions.Length != values.Length)
                throw new ArgumentException($"{nameof(positions)} and {nameof(values)} sets must be of equal length.");

            if (positions.Length < 3)
                throw new ArgumentException($"Data must contain at least 3 points.");

            if (windowHalfWidth <= 0)
                throw new ArgumentException($"{nameof(windowHalfWidth)} must be greater than zero.");
        }

        private static double[] ScalePositions(double[] positions)
        {
            var positionsScaling = Scaling.CreateForData(positions, new ValuesRange(0, 1));
            double[] scaledPositions = positions.ToArray();
            positionsScaling.ScaleToTarget(scaledPositions);
            return scaledPositions;
        }

        private static double[] GetApproximatedFirstDerivation(double[] positions, double[] values, int windowHalfWidth)
        {
            VerifyDataAndParameters(positions, values, windowHalfWidth, out int interpolationLength, out int maxStartIndex);
            double[] result = new double[positions.Length];
            Parallel.For(0, positions.Length, i =>
            {
                int start = i - windowHalfWidth;
                start = start < 0 ? 0 : start;
                start = start > maxStartIndex ? maxStartIndex : start;
                SecondDegreePolynomialCoefficients? coeffs = SecondDegreePolynomialCoefficients.CreateApproximatedPolynomial(
                    positions, values, start, interpolationLength);
                result[i] = coeffs.HasValue ? 2 * coeffs.Value.A2 * positions[i] + coeffs.Value.A1 : double.NaN;
            });
            return result;
        }

        private static double[] GetApproximatedSecondDerivation(double[] positions, double[] values, int windowHalfWidth)
        {
            VerifyDataAndParameters(positions, values, windowHalfWidth, out int interpolationLength, out int maxStartIndex);
            double[] result = new double[positions.Length];
            Parallel.For(0, positions.Length, i =>
            {
                int start = i - windowHalfWidth;
                start = start < 0 ? 0 : start;
                start = start > maxStartIndex ? maxStartIndex : start;
                SecondDegreePolynomialCoefficients? coeffs = SecondDegreePolynomialCoefficients.CreateApproximatedPolynomial(
                    positions, values, start, interpolationLength);
                result[i] = coeffs.HasValue ? 2 * coeffs.Value.A2 : 0;
            });
            return result;
        }

        private static void VerifyDataAndParameters(
        double[] positions, double[] values, int windowHalfWidth, out int interpolationLength, out int maxStartIndex)
        {
            if (positions.Length != values.Length)
                throw new ArgumentException($"{nameof(positions)} and {nameof(values)} must be of equal length.");
            if (windowHalfWidth == 0)
                throw new ArgumentException($"{nameof(windowHalfWidth)} must be greater than 0.");
            interpolationLength = WindowWidthCalculator.GetFullWidth(windowHalfWidth);
            if (interpolationLength > positions.Length)
                throw new ArgumentException($"{nameof(windowHalfWidth)} is to big for such input data.");
            maxStartIndex = positions.Length - interpolationLength;
        }

        private static bool TryGetPeakPosition(
        double[] positions,
        double[] secondDerivation,
        double[] firstDerivationOfSecondDerivation,
        DataSection peakDataRegion,
        int windowHalfWidth,
        out double peakPosition,
        out int minSecondDerivationIndex)
        {
            peakPosition = double.NaN;
            minSecondDerivationIndex = peakDataRegion.StartInclusive;
            double minSecondDerivation = secondDerivation[minSecondDerivationIndex];
            for (int i = peakDataRegion.StartInclusive + 1; i <= peakDataRegion.EndInclusive; i++)
            {
                if (minSecondDerivation > secondDerivation[i])
                {
                    minSecondDerivation = secondDerivation[i];
                    minSecondDerivationIndex = i;
                }
            }
            int approximationHalfWidth = Math.Min((peakDataRegion.Length - 1) / 2, windowHalfWidth);
            if (SecondDegreePolynomialCoefficients.TryGetApproximationPointsAroundPivotPoint(
                peakDataRegion.StartInclusive,
                peakDataRegion.EndInclusive,
                minSecondDerivationIndex,
                approximationHalfWidth,
                out int approximationStart,
                out int approximationLength))
            {
                SecondDegreePolynomialCoefficients? coeffs = SecondDegreePolynomialCoefficients.CreateApproximatedPolynomial(
                    positions, secondDerivation, approximationStart, approximationLength);
                if (!coeffs.HasValue)
                    return false;
                peakPosition = -coeffs.Value.A1 / (2.0 * coeffs.Value.A2);
                if (peakPosition >= positions[peakDataRegion.StartInclusive] && peakPosition <= positions[peakDataRegion.EndInclusive])
                    return true;
            }

            if (CurveZeroIntersectionSearcher.TryGetCurveZeroIntersection(
                positions, firstDerivationOfSecondDerivation, peakDataRegion, windowHalfWidth, out peakPosition) &&
                peakPosition >= positions[peakDataRegion.StartInclusive] && peakPosition <= positions[peakDataRegion.EndInclusive])
                return true;
            peakPosition = (positions[peakDataRegion.StartInclusive] + positions[peakDataRegion.EndInclusive]) / 2;
            return true;
        }
    }
}
