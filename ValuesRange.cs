using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeakFinder
{
    public readonly struct ValuesRange
    {
        /// <summary>
        /// Min range value.
        /// </summary>
        public readonly double Min;

        /// <summary>
        /// Max range value.
        /// </summary>
        public readonly double Max;

        /// <summary>
        /// Range width :difference between <see cref="Max"/> and <see cref="Min"/>.
        /// </summary>
        public readonly double Width;

        /// <summary>
        /// Values range constructor.
        /// </summary>
        /// <param name="min">Min range value.</param>
        /// <param name="max">Max range value.</param>
        ///<remarks>
        ///if <paramref name="min"/> is greater than <paramref name="max"/>
        ///than min will be set to <see cref="Max"/> and vice versa.
        ///</remarks>
        public ValuesRange(double min, double max)
        {
            if (min <= max)
            {
                Min = min;
                Max = max;
                Width = max - min;
            }
            else
            {
                Min = max;
                Max = min;
                Width = min - max;
            }
        }
    }
}
