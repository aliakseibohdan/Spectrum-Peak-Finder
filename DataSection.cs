using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeakFinder
{
    public readonly struct DataSection
    {
        /// <summary>
        /// Zero based start data section index inclusive.</param>
        /// </summary>
        public readonly int StartInclusive;

        /// <summary>
        /// Zero based end data section index inclusive.</param>
        /// </summary>
        public readonly int EndInclusive;

        /// <summary>
        /// Data section points number.
        /// </summary>
        /// <remarks>
        /// Always greater than 0.
        /// </remarks>
        public readonly int Length;

        private DataSection(int startInclusive, int endInclusive, int length)
        {
            StartInclusive = startInclusive;
            EndInclusive = endInclusive;
            Length = length;
        }

        /// <summary>
        /// Create <see cref="DataSection"/> instance using start index and section length.
        /// </summary>
        /// <param name="startInclusive">Zero based start data section index inclusive.</param>
        /// <param name="length">Data section length.</param>
        /// <returns><see cref="DataSection"/> instance.</returns>
        /// <exception cref="ArgumentException">Throw if length is not a positive integer number.</exception>
        public static DataSection CreateRange(int startInclusive, int length)
        {
            if (startInclusive < 0)
                throw new ArgumentException($"{nameof(startInclusive)} must be non negative value.");
            if (length <= 0)
                throw new ArgumentException("Section must have positive length.");
            int endInclusive = startInclusive + length - 1;
            return new DataSection(startInclusive, endInclusive, length);
        }

        /// <summary>
        /// Create <see cref="DataSection"/> instance using start index and end index.
        /// </summary>
        /// <param name="startInclusive">Zero based start data section index inclusive.</param>
        /// <param name="endInclusive">Zero based end data section index inclusive.</param>
        /// <returns><see cref="DataSection"/> instance.</returns>
        /// <exception cref="ArgumentException">Throw if <paramref name="endInclusive"/> index is lesser than <paramref name="startInclusive"/> index.</exception>
        public static DataSection CreateStartEnd(int startInclusive, int endInclusive)
        {
            if (startInclusive < 0)
                throw new ArgumentException($"{nameof(startInclusive)} must be non negative value.");
            if (endInclusive < startInclusive)
                throw new ArgumentException("Section end must be greater than start.");
            int length = endInclusive - startInclusive + 1;
            return new DataSection(startInclusive, endInclusive, length);
        }

        /// <summary>
        /// Create <see cref="DataSection"/> instance that cover all data indexes.
        /// </summary>
        /// <param name="array">Array to cover.</param>
        /// <returns>Data section for whole array<paramref name="array"/>.</returns>
        public static DataSection CreateAllData<T>(T[] array) => CreateRange(0, array.Length);
    }
}
