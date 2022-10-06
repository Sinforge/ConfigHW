using AngleSharp.Html.Parser;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace visualizer
{
    internal class Program
    {
        //Commented code is deprecated)
        // private const string MainGitHubURL = "https://github.com/";
        // private const string MainGitHubURL1 = "https://github.com";
        private const string PypiURL = "https://pypi.org/pypi/";
        private const string MetadataPath = "C:\\Users\\vladv\\TarDir\\METADATA.txt";
        private const string PathToLoad = "C:\\Users\\vladv\\TarDir\\my.tar";



        static async Task<int> Main(string[] args)
        {
            if (File.Exists(MetadataPath))
            {
                File.Delete(MetadataPath);
            }
            string NameOfLibrary = args[0].ToLower();
            string MetaDataURL = await GetUrlToMetadata(NameOfLibrary); //Получаем ссылка на метаданные
            GetMetadata(MetaDataURL);//Получаем метаданные пакета
            List<string> dependenciesList = await GetDependenciesList();//получаем зависимости пакета
            
            PrintGraphvizCode(dependenciesList); //Выводит код графа

            return 0;

        }

        private static void PrintGraphvizCode(List<string> dependenciesList)
        {
            Console.WriteLine("digraph G {");
            foreach (var dependency in dependenciesList)
            {
                Console.WriteLine("\t" + dependency);
            }
            Console.WriteLine("}");
        }

        private static async Task<List<string>> GetDependenciesList()
        {
            var dependenciesList = new List<string>();
            using (StreamReader reader = new StreamReader(MetadataPath))
            {
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    if (line.Contains("Requires-Dist:"))
                    {
                        var separated_line = line.Split(' ');
                        dependenciesList.Add(separated_line[1] + separated_line[2]);
                    }
                }
            }

            return dependenciesList;
        }

        private static void GetMetadata(string MetaDataURL)
        {
            using (WebClient wc = new WebClient())
            {
                wc.DownloadFile(MetaDataURL, PathToLoad);
                ZipArchive archive = ZipFile.OpenRead(PathToLoad);
                foreach (var entry in archive.Entries)
                {
                    if (entry.FullName.EndsWith("METADATA"))
                    {
                        entry.ExtractToFile(MetadataPath);

                    }
                }
            }
        }

        private static async Task<string> GetUrlToMetadata(string NameOfLibrary)
        {
            var httpClient = new HttpClient();

            var json = await httpClient.GetStringAsync(PypiURL + NameOfLibrary + "/json");
            JObject Json = JObject.Parse(json);
            var version = Json["info"]["version"].ToString();
            var releases = Json["releases"];
            var lastRelease = releases[version][0];
            var MetadataURL = lastRelease["url"].ToString();
            return MetadataURL;
        }

        /*private static async Task<int> GetDependenciesTree(string NameOfLibrary, int level)
        {
            string GitHubURL = await GetGitHubOfLibrary(NameOfLibrary);
            if (GitHubURL == null)
            {
                Console.WriteLine("Sorry, but we cant found this library");
                return -1;
            }
            string SetupURL = GetSetupFileAsync(GitHubURL).Result;
            if (SetupURL == null)
            {
                Console.WriteLine("Sorry, but we cant find setup file");
                return -1;
            }

            var dependencies = GetDependenciesFromSetupFile(MainGitHubURL1 + SetupURL).Result;

            for (int i =1; i < dependencies.Count - 1; i++)
            {
                for(int j =0; j < level; j++)
                {
                    Console.Write("\t");
                }
                var dep = new String(dependencies[i].Where(c => Char.IsLetter(c) || Char.IsDigit(c) || ">=<~.^*".Contains(c) && !c.Equals("\"")).ToArray());
                Console.WriteLine(dep);
            }

            return 0;
        }

        static async Task<List<string>> GetDependenciesFromSetupFile(string SetupURL)
        {
            var responeBody = client.GetStringAsync(SetupURL).Result;
            var document = await parser.ParseDocumentAsync(responeBody);
            var CodeLines = document.QuerySelectorAll("tbody > tr");
            var dependenciesList = new List<string>();
            bool find_begin_of_dependencies = false;
            foreach(var line in CodeLines) {
                if (line.TextContent.Replace(" ", "").Contains("install_requires"))
                {
                    find_begin_of_dependencies = true;
                }
                
                if (find_begin_of_dependencies)
                {
                    dependenciesList.Add(line.TextContent);
                }
                if (find_begin_of_dependencies && line.TextContent.Contains("]"))
                {
                    break;
                }
            }
            return dependenciesList;


        }
        static async Task<string> GetGitHubOfLibrary(string LibraryName) {
            string URL = "https://pypi.org/pypi/" + LibraryName;
            var responeBody = client.GetStringAsync(URL).Result;
            var document = await parser.ParseDocumentAsync(responeBody);

            foreach(var link in document.GetElementsByTagName("a"))
            {
                if(link.GetAttribute("href").Contains(MainGitHubURL + LibraryName + "/") && link.TextContent.Contains("Source Code")) return link.GetAttribute("href");
            }
            return null;
        }
        

        static async Task<string> GetSetupFileAsync(string GitHubURL)
        {
            var responseBody = client.GetStringAsync(GitHubURL).Result;
            var document = await parser.ParseDocumentAsync(responseBody);
            string SetupPageURL = document.QuerySelectorAll("div > span > a").Where(a => a.GetAttribute("href").Contains("setup.py")).FirstOrDefault().GetAttribute("href");
            return SetupPageURL;
             
                // .Where(a => a.GetAttribute("title") .Equals("setup.py")).FirstOrDefault().GetAttribute("href");
            //Console.WriteLine(SetupURL);

        }
    }*/





    }
}
