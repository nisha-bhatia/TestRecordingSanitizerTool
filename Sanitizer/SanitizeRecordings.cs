using Azure.Core.TestFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Sanitizer
{
    class SanitizeRecordings
    {
        static void Main(string[] args)
        {
            ProcessDirectory("C:/GitHub/azure-sdk-for-net/sdk/appconfiguration/Azure.ResourceManager.AppConfiguration/tests");
            foreach (var filename in filenames)
            {
                if (!filename.Contains(".json"))
                    continue;
                RecordSession newRecording;

                using (FileStream fileStream = File.OpenRead(filename))
                {   
                    try 
                    {
                        using JsonDocument jsonDocument = JsonDocument.Parse(fileStream);
                        newRecording = RecordSession.Deserialize(jsonDocument.RootElement); //replace old recording with new

                        newRecording.Sanitize(new RecordedTestSanitizer());
                    }
                    catch
                    {
                        continue;
                    }
                }

                using FileStream fs = File.Create(filename);
                var utf8JsonWriter = new Utf8JsonWriter(fs, new JsonWriterOptions()
                {
                    Indented = true
                });
                newRecording.Serialize(utf8JsonWriter);
                utf8JsonWriter.Flush();

                Console.WriteLine(filename);
            }
        }

        public static List<string> filenames = new List<string>();
        // Process all files in the directory passed in, recurse on any directories
        // that are found, and process the files they contain.
        public static void ProcessDirectory(string targetDirectory)
        {
            if (targetDirectory == null || targetDirectory == "")
                return;

            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string filename in fileEntries)
                filenames.Add(filename);

            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
                ProcessDirectory(subdirectory);
        }
    }
}
