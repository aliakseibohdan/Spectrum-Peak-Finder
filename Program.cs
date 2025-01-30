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

            var fileName = "GaAs004.dat";
            var projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            var inputFilePath = Path.Combine(projectDirectory, @"Data\", fileName);

            var spectrum = SpectrumReader.ReadSpectrumDataFromFile(inputFilePath);

            #endregion

            #region Smooth data

            var windowSize = 5;
            var polyOrder = 2;

            var filter = new SavitzkyGolayFilter(windowSize, polyOrder);
            var smoothedValuesY = filter.Process(spectrum.ValuesY);

            #endregion

            #region Find peaks

            var windowHalfWidth = 2;
            var peaks = SpectrumPeakFinder.SearchPeaks(spectrum.ValuesX, smoothedValuesY, windowHalfWidth);

            #endregion

            #region Export data

            var outputFilePath = $"Smoothed_{Path.GetFileNameWithoutExtension(inputFilePath)}.dat";

            //DataExporter.ExportData(smoothedValuesY, outputFilePath);

            #endregion
        }
    }
}
