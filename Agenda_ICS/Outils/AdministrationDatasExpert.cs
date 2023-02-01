using System.IO;

namespace NOutils
{
    public class AdministrationDatasExpert
    {
        public const int ID_LINE_LASTDATEWHENLICENCEWASOK = 2;

        public static string[] Read(string pathToFile)
        {
            var lines = File.ReadAllLines(pathToFile);
            return lines;
        }

        public static string ReadLine(string pathToFile, int i)
        {
            var lines = File.ReadAllLines(pathToFile);
            return lines[i];
        }

        public static void ModifyLine(string pathToFile, int i, string newLine)
        {
            var lines = File.ReadAllLines(pathToFile);
            lines[i] = newLine;
            File.WriteAllLines(pathToFile, lines);
        }
    }
}
