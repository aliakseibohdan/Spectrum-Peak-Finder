using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeakFinder
{
    public class DataExporter
    {
        public static void WriteDataToFile(string filePath, IReadOnlyList<float> valuesX, IReadOnlyList<float> valuesY)
        {
            try
            {
                if (valuesX.Count != valuesY.Count || valuesX.Count == 0)
                {
                    Console.WriteLine("Error: Mismatch in the number of X and Y values or data is empty.");
                    return;
                }

                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    for (int i = 0; i < valuesX.Count; i++)
                    {
                        writer.WriteLine($"{valuesX[i]} {valuesY[i]}");
                    }

                    writer.Dispose();
                }

                Console.WriteLine($"Data has been successfully written to {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to file: {ex.Message}");
            }
        }

        public static void ExportData(Spectrum spectrum, string outputFilePath)
        {
            WriteDataToFile(outputFilePath, spectrum.ValuesX, spectrum.ValuesY);
        }
    }
}
