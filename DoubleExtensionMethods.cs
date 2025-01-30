using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PeakFinder
{
    public static class DoubleExtensionMethods
    {
        /// <summary>
        /// Get second power of <paramref name="value"/>.
        /// Used for performance reasons.
        /// </summary>
        /// <param name="value">Value which will be raised to the second power.</param>
        /// <returns>Second power of <paramref name="value"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Pow2(this double value) => value * value;

        /// <summary>
        /// Get third power of <paramref name="value"/>.
        /// Used for performance reasons.
        /// </summary>
        /// <param name="value">Value which will be raised to the third power.</param>
        /// <returns>Third power of <paramref name="value"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Pow3(this double value) => value * value * value;

        /// <summary>
        /// Get fourth power of <paramref name="value"/>.
        /// Used for performance reasons.
        /// </summary>
        /// <param name="value">Value which will be raised to the fourth power.</param>
        /// <returns>Fourth power of <paramref name="value"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Pow4(this double value) => Pow2(Pow2(value));
        /// <summary>
        /// Determine whether the value can be treated as zero with precision specified by <paramref name="epsilon"/> value.
        /// </summary>
        /// <param name="value">The value begin explored.</param>
        /// <param name="epsilon">Precision value.</param>
        /// <returns><code>True</code> if abs <paramref name="value"/> is lesser than <paramref name="epsilon"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsCloseToZero(this double value, double epsilon) => Math.Abs(value) < epsilon;
    }
}
