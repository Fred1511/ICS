using NDatasModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateDatasApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var pathToFolder = args[0];

            var pathToChantiersFile = pathToFolder + @"\Chantiers.fic";
            var chantiers = ReadChantiersFromFile(pathToChantiersFile);
            WriteChantiersToFile(chantiers, pathToChantiersFile);
            //for (int i = 0; i < chantiers.Length; i++)
            //{
            //    Console.WriteLine(chantiers[i].Name);
            //}
        }

        private static void WriteChantiersToFile(CChantier[] chantiers, string pathToChantiersFile)
        {
            using (var fileStream = new FileStream(pathToChantiersFile, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (var writer = new BinaryWriter(fileStream))
                {
                    writer.Write(chantiers.Length);

                    foreach (var chantier in chantiers)
                    {
                        NDatasBaseOnFile.DatasBase.WriteChantierToFile(writer, chantier);
                    }
                }
            }
        }

        private static CChantier[] ReadChantiersFromFile(string pathToChantiersFile)
        {
            var output = new List<CChantier>();

            using (var fileStream = new FileStream(pathToChantiersFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var reader = new BinaryReader(fileStream))
                {
                    var nbChantiers = reader.ReadInt32();

                    for (var i = 0; i < nbChantiers; i++)
                    {
                        var chantier = NDatasBaseOnFile.DatasBase.ReadChantierFromOldFile(reader);
                        output.Add(chantier);
                    }
                }
            }

            return output.ToArray();

        }
    }
}
