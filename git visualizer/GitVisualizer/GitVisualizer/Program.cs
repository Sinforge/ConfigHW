using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace GitVisualizer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {

            List<string> pointersList = new List<string>();
            string PathToGitRepo = args[0] + @"\.git\logs\HEAD";
            var textFromFile = await TextFromFile(PathToGitRepo);
            string[] objects = textFromFile.Split('\n');
            PrintGraph(objects, pointersList);
        }

        private static void PrintGraph(string[] objects, List<string> list)
        {
            Console.WriteLine("digraph D {");
            for (int i = 0; i < objects.Length - 1; i++)
            {
                string[] objectInfo = objects[i].Split(' ');
                if (objectInfo != null && objectInfo.Length > 0 && objects[i].Contains("\tcommit"))
                {
                    list.Add("\"" + objectInfo[0] + "\"" + " -> " + "\"" + objectInfo[1] + "\"");
                }
            }

            foreach (string s in list)
            {
                Console.WriteLine("\t" + s);
            }

            Console.WriteLine("}");
        }

        private static async Task<string> TextFromFile(string PathToGitRepo)
        {
            FileStream fstream = File.OpenRead(PathToGitRepo);
            // Get bytes from stream
            byte[] buffer = new byte[fstream.Length];
            // Get data
            await fstream.ReadAsync(buffer, 0, buffer.Length);
            // Decode data
            string textFromFile = Encoding.Default.GetString(buffer);
            return textFromFile;
        }
    }
}
