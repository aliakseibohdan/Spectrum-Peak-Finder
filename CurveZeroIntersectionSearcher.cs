using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeakFinder
{
    internal static class CurveZeroIntersectionSearcher
    {
        /// <summary>
        /// Try search curve zero intersection.
        /// </summary>
        /// <param name="positions">Data points positions.</param>
        /// <param name="values">Data points values.</param>
        /// <param name="dataRegion">Curve region of interest.</param>
        /// <param name="windowHalfWidth">Smoothing data region half window.</param>
        /// <param name="intersectionPoint"></param>
        /// <returns>If curve has zero intersection in region of interest.</returns>
        /// <exception cref="ArgumentException">If <paramref name="positions"/> and <paramref name="values"/> are of unequal length.
        /// If <paramref name="dataLength"/> have lesser than 3 points.
        /// If <paramref name="startIndex"/> and <paramref name="dataLength"/> lead to out of range exception 
        /// for <paramref name="positions"/> of <paramref name="values"/>.</exception>
        internal static bool TryGetCurveZeroIntersection(
            double[] positions, double[] values, DataSection dataRegion, int windowHalfWidth, out double intersectionPoint)
        {
            if (positions.Length != values.Length)
                throw new ArgumentException($"{nameof(positions)} and {nameof(values)} must be of equal length.");
            if (dataRegion.Length < 3)
                throw new ArgumentException($"{nameof(dataRegion)} length must be greater or equal than 3 because data interpolation will be used.");
            if (dataRegion.EndInclusive >= positions.Length)
                throw new ArgumentException($"{nameof(dataRegion)} must not lead index out of the {nameof(positions)} or {nameof(values)} range.");
            if (windowHalfWidth < 0)
                throw new ArgumentException($"{nameof(windowHalfWidth)} must be greater than 0.");

            intersectionPoint = double.NaN;
            int leftIntersectionSegmentIndex = -1;
            for (int i = dataRegion.StartInclusive; i < dataRegion.EndInclusive; i++)
            {
                if (values[i] * values[i + 1] <= 0)
                {
                    leftIntersectionSegmentIndex = i;
                    break;
                }
            }
            if (leftIntersectionSegmentIndex < 0)
                return false;
            if (TryGetIntersectionUsingSecondDegreePolyInterpolation(
                positions, values, leftIntersectionSegmentIndex, windowHalfWidth, dataRegion, out intersectionPoint))
                return true;
            return TryGetIntersectionTwoPointsLinearInterpolation(
                positions, values, leftIntersectionSegmentIndex, dataRegion, out intersectionPoint);
        }

        /// <summary>
        /// Try get curve zero intersection using second degree polynomial data interpolation.
        /// </summary>
        /// <param name="positions">Data points positions.</param>
        /// <param name="values">Data points values.</param>
        /// <param name="leftIntersectionSegmentIndex">Intersection curve segment left point index.</param>
        /// <param name="windowHalfWidth">Smoothing data half window size.</param>
        /// <param name="dataRegion">Region of interest.</param>
        /// <param name="intersectionPoint">Zero intersection point position.</param>
        /// <returns>If curve zero intersection was found.</returns>
        private static bool TryGetIntersectionUsingSecondDegreePolyInterpolation(
            double[] positions,
            double[] values,
            int leftIntersectionSegmentIndex,
            int windowHalfWidth,
            DataSection dataRegion,
            out double intersectionPoint)
        {
            intersectionPoint = double.NaN;
            if (!SecondDegreePolynomialCoefficients.TryGetApproximationPointsAroundPivotPoint(
                dataRegion.StartInclusive,
                dataRegion.EndInclusive,
                leftIntersectionSegmentIndex,
                windowHalfWidth,
                out int approximationStart,
                out int approximationLength))
                return false;
            SecondDegreePolynomialCoefficients? coeffs = SecondDegreePolynomialCoefficients.CreateApproximatedPolynomial(
                positions, values, approximationStart, approximationLength);
            if (!coeffs.HasValue)
                return false;
            double xMin = positions[dataRegion.StartInclusive];
            double xMax = positions[dataRegion.EndInclusive];
            double root;
            if (coeffs.Value.A2 == 0)
            {
                if (coeffs.Value.A1 == 0)
                    return false;
                root = -coeffs.Value.A0 / coeffs.Value.A1;
                if (xMin <= root && root <= xMax)
                {
                    intersectionPoint = root;
                    return true;
                }
                return false;
            }
            double discriminant = coeffs.Value.A1.Pow2() - 4 * coeffs.Value.A0 * coeffs.Value.A2;
            if (discriminant < 0)
                return false;
            double sqrtDiscriminant = Math.Sqrt(discriminant);
            root = (-coeffs.Value.A1 - sqrtDiscriminant) / (2 * coeffs.Value.A2);
            if (xMin <= root && root <= xMax)
            {
                intersectionPoint = root;
                return true;
            }
            root = (-coeffs.Value.A1 + sqrtDiscriminant) / (2 * coeffs.Value.A2);
            if (xMin <= root && root <= xMax)
            {
                intersectionPoint = root;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get zero intersection coordinate using linear interpolation between two points which values have different signs.
        /// </summary>
        /// <param name="positions">Data points positions.</param>
        /// <param name="values">Data points values.</param>
        /// <param name="leftIntersectionSegmentIndex">Intersection curve segment left data index.</param>
        /// <param name="dataRegion">Region of interest.</param>
        /// <param name="intersectionPoint">Zero intersection point position.</param>
        /// <returns>If curve zero intersection was found.</returns>
        private static bool TryGetIntersectionTwoPointsLinearInterpolation(
            double[] positions, double[] values, int leftIntersectionSegmentIndex, DataSection dataRegion, out double intersectionPoint)
        {
            intersectionPoint = double.NaN;
            int rightIntersectionSegmentIndex = leftIntersectionSegmentIndex + 1;
            double x = (values[rightIntersectionSegmentIndex] * positions[leftIntersectionSegmentIndex]
                - values[leftIntersectionSegmentIndex] * positions[rightIntersectionSegmentIndex])
                / (values[rightIntersectionSegmentIndex] - values[leftIntersectionSegmentIndex]);
            if (positions[dataRegion.StartInclusive] <= x && x <= positions[dataRegion.EndInclusive])
            {
                intersectionPoint = x;
                return true;
            }
            return false;
        }
    }
}
