using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace visualizer
{
    internal class Program
    {
        private const string MainGitHubURL = "https://github.com/";
        private const string MainGitHubURL1 = "https://github.com";
        private static readonly HttpClient client = new HttpClient();
        private static readonly HtmlParser parser = new HtmlParser();


        static async Task<int> Main(string[] args)
        {
            string NameOfLibrary = args[0].ToLower();
            return await GetDependenciesTree(NameOfLibrary, 0);
        }

        private static async Task<int> GetDependenciesTree(string NameOfLibrary, int level)
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
    }




   
}
