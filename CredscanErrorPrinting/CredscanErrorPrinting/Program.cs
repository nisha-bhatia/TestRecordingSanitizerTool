using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace CredscanErrorPrinting
{
    class Program
    {
        static void Main(string[] args)
        {
            string logFilePath = "C:/Users/nibhati/Downloads/credscan-matches.sarif";

            // For a file in the deprecated, pre-standardization SARIF v1.0 format:
            string logContents = File.ReadAllText(logFilePath);

            dynamic array = JsonConvert.DeserializeObject(logContents);

            var pairs = new List<KeyValuePair<string, int[]>>();
            string filepath;
            int linenumber;
            int startColumn;
            int endColumn;

            foreach (var result in array.runs[0].results)
            {
                filepath = result.locations[0].physicalLocation.artifactLocation.uri;
                // For more filtering, can pass in a string into the following if statement, or comment it out to get all results
                if (filepath.Contains("Azure.Analytics.Synapse.Artifacts"))
                {
                    foreach (var location in result.locations)
                    {
                        var identifier = new int[3];
                        linenumber = result.locations[0].physicalLocation.region.startLine;
                        startColumn = result.locations[0].physicalLocation.region.startColumn;
                        endColumn = result.locations[0].physicalLocation.region.endColumn;
                        identifier[0] = linenumber;
                        identifier[1] = startColumn;
                        identifier[2] = endColumn;
                        pairs.Add(new KeyValuePair<string, int[]>(filepath, identifier));
                    }
                }
            }

            Console.WriteLine("NUMBER OF LEAKED SECRETS : " + pairs.Count);

            foreach (var pair in pairs)
            {
                string filename = pair.Key.Replace("file:///D:/a/1/s", "C:/GitHub/Azure/azure-sdk-for-net");
                string fileContents = File.ReadAllText(filename);
                string[] contents = fileContents.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                var loc = pair.Value;
                var toParse = contents[loc[0] - 1];
                toParse.Replace("\\n", "");
                toParse.Replace("\\r", "");
                var secret = toParse.Substring(loc[1], loc[2] - loc[1]);
                var beforesecret = toParse.Substring(loc[1] - 30, 150);
                Console.WriteLine("FILENAME : " + filename);
                Console.WriteLine("SECRET : " + secret + "\n");
                Console.WriteLine("TEXT BEFORE SECRET : " + beforesecret + "\n");
            }
        }
    }
}
