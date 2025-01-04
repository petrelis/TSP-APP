namespace TSPLIB
{
    public class TSPParser
    {
        public static (string name, List<Point> points) ParseTspFile(string fileName)
        {
            if (!fileName.EndsWith(".tsp", StringComparison.OrdinalIgnoreCase))
            {
                fileName += ".tsp";
            }

            string currentAssemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string currentDirectory = Path.GetDirectoryName(currentAssemblyPath);

            DirectoryInfo directory = new DirectoryInfo(currentDirectory);
            while (directory != null && !directory.GetDirectories("TSPLIB").Any())
            {
                directory = directory.Parent;
            }

            if (directory == null)
            {
                throw new DirectoryNotFoundException("Could not find TSPLIB directory in parent folders.");
            }

            string tspDataPath = Path.Combine(directory.FullName, "TSPLIB", "TSPData", fileName);

            string name = "";
            var points = new List<Point>();

            try
            {
                if (!File.Exists(tspDataPath))
                {
                    throw new FileNotFoundException($"The TSP file '{fileName}' was not found in the TSPData folder.");
                }

                string[] lines = File.ReadAllLines(tspDataPath);

                foreach (var line in lines)
                {
                    if (line.StartsWith("NAME"))
                    {
                        name = line.Split(':')[1].Trim();
                        break;
                    }
                }

                bool inCoordSection = false;

                var numberFormat = new System.Globalization.NumberFormatInfo
                {
                    NumberDecimalSeparator = ".",
                    NumberGroupSeparator = string.Empty
                };

                foreach (var line in lines)
                {
                    if (line.Contains("NODE_COORD_SECTION"))
                    {
                        inCoordSection = true;
                        continue;
                    }

                    if (line.Contains("EOF"))
                        break;

                    if (inCoordSection)
                    {
                        var parts = line.Trim().Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        if (double.TryParse(parts[1].Replace(',', '.'),
                                System.Globalization.NumberStyles.Any,
                                numberFormat,
                                out double x) &&
                            double.TryParse(parts[2].Replace(',', '.'),
                                System.Globalization.NumberStyles.Any,
                                numberFormat,
                                out double y))
                        {
                            points.Add(new Point(x, y));
                        }
                    }
                }

                return (name, points);
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException($"The TSP file '{fileName}' was not found at path: {tspDataPath}");
            }
            catch (IOException ex)
            {
                throw new IOException($"Error reading the TSP file '{fileName}': {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while parsing the TSP file '{fileName}': {ex.Message}");
            }
        }

        public static List<string?> GetTspFileNames()
        {
            string currentAssemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string currentDirectory = Path.GetDirectoryName(currentAssemblyPath);

            DirectoryInfo directory = new DirectoryInfo(currentDirectory);
            while (directory != null && !directory.GetDirectories("TSPLIB").Any())
            {
                directory = directory.Parent;
            }

            if (directory == null)
            {
                throw new DirectoryNotFoundException("Could not find TSPLIB directory in parent folders.");
            }

            string tspDataPath = Path.Combine(directory.FullName, "TSPLIB", "TSPData");

            return Directory.GetFiles(tspDataPath, "*.tsp")
                .Select(Path.GetFileName)
                .ToList();
        }
    }
}