﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace PeakFinder
{
    public class SpectrumReader
    {
        /// <summary>
        /// Assumes that there is one entry per line of two values separated by a space
        /// </summary>
        /// <returns> <see cref="Spectrum"/> object; invalid entries will be skipped when creating the spectrum </returns>
        public static Spectrum ReadSpectrumDataFromFile(string filePath)
        {
            Spectrum spectrum = new Spectrum();

            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] values = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        if (values.Length == 2)
                        {
                            try
                            {
                                float x = Single.Parse(values[0], System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture);
                                float y = Single.Parse(values[1], System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture);

                                if (y <= 0)
                                {
                                    Console.WriteLine($"Skipping invalid Y value (non-positive): {y}");
                                    continue;
                                }

                                spectrum.AddDataPoint(x, MathF.Log10(y));
                            }
                            catch (FormatException)
                            {
                                Console.WriteLine($"Skipping invalid line: {line}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Skipping invalid line (not exactly two values): {line}");
                        }
                    }

                    reader.Dispose();
                }

                Console.WriteLine("File read successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading file: {ex.Message}");
            }

            return spectrum;
        }
    }
}
