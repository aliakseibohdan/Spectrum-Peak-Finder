using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace PeakFinder
{
    class Program
    {
        static void Main()
        {
            #region Read spectrum

            string fileName = "GaAs004.dat";
            string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            string inputFilePath = Path.Combine(projectDirectory, @"Data\", fileName);

            Spectrum spectrum = SpectrumReader.ReadSpectrumDataFromFile(inputFilePath);

            #endregion

            #region Smooth data

            int windowSize = 5;
            int polyOrder = 2;

            Spectrum smoothedSpectrum = SavitzkyGolayFilter.ApplySavitzkyGolayFilter(spectrum, windowSize, polyOrder);

            #endregion

            #region Find peaks

            IReadOnlyList<int> peaks = SpectrumPeakFinder.FindPeaks(smoothedSpectrum);

            #endregion

            #region Export data

            string outputFilePath = $"Smoothed_{Path.GetFileNameWithoutExtension(inputFilePath)}.dat";

            DataExporter.ExportData(smoothedSpectrum, outputFilePath);

            #endregion
        }
    }
}
