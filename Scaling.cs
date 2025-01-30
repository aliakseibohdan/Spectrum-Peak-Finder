using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeakFinder
{
    public readonly struct Scaling
    {
        private readonly ValuesRange targetRange;
        private readonly ValuesRange sourceRange;

        private Scaling(ValuesRange targetRange, ValuesRange sourceRange)
        {
            this.targetRange = targetRange;
            this.sourceRange = sourceRange;
        }

        /// <summary>
        /// Create data scaling for provided data <paramref name="sourceData"/> and target scale region.
        /// Intendent to be used with provided data only.
        /// </summary>
        /// <param name="sourceData">Input data intendent to be scale.</param>
        /// <param name="targetMin">Target scale region min.</param>
        /// <param name="targetMax">Target scale region max.</param>
        /// <returns>Scaling instance.</returns>
        /// <exception cref="ArgumentException"> If <paramref name="targetMax"/> is greater than <paramref name="targetMin"/>.
        /// </exception>
        public static Scaling CreateForData(double[] sourceData, double targetMin, double targetMax)
        {
            if (targetMin > targetMax)
                throw new ArgumentException($"{nameof(targetMax)} must be greater than {nameof(targetMin)}");

            var targetRange = new ValuesRange(targetMin, targetMax);
            return CreateForData(sourceData, targetRange);
        }

        /// <summary>
        /// Create data scaling for provided data <paramref name="sourceData"/> and target scale region.
        /// Intendent to be used with provided data only.
        /// </summary>
        /// <param name="sourceData">Input data intendent to be scale.</param>
        /// <param name="targetValuesRange">Target scale region values range.</param>
        /// <returns>Scaling instance.</returns>
        public static Scaling CreateForData(double[] sourceData, ValuesRange targetValuesRange)
        {
            double sourceMin = double.MaxValue;
            double sourceMax = double.MinValue;
            foreach (double value in sourceData)
            {
                if (sourceMin > value)
                    sourceMin = value;
                if (sourceMax < value)
                    sourceMax = value;
            }
            ValuesRange sourceRange = new ValuesRange(sourceMin, sourceMax);
            return new Scaling(targetValuesRange, sourceRange);
        }

        /// <summary>
        /// Scale input data using previously calculated coefficients.
        /// If scale source boundaries are equal result will filled with scale target min values. 
        /// </summary>
        /// <param name="sourceData">Data to be scaled.</param>
        public void ScaleToTarget(double[] sourceData)
        {
            if (sourceRange.Width == 0)
            {
                Array.Fill(sourceData, targetRange.Min);
                return;
            }
            double multiplier = targetRange.Width / sourceRange.Width;
            for (int i = 0; i < sourceData.Length; i++)
                sourceData[i] = (sourceData[i] - sourceRange.Min) * multiplier + targetRange.Min;
        }
    }
}