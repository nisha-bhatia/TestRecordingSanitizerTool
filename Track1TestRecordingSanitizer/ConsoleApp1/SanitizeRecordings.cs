using Microsoft.Azure.Test.HttpRecorder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Track1TestRecordingSanitizer
{
    class SanitizeRecordings
    {
        static void Main(string[] args)
        {
            //ProcessDirectory("C:/GitHub/azure-sdk-for-net/sdk/cosmosdb/Microsoft.Azure.Management.CosmosDB/tests");
            ProcessDirectory("C:/GitHub/azure-sdk-for-net/sdk/search/Microsoft.Azure.Search/tests/SessionRecords");
            foreach (var filename in filenames)
            {
                if (!filename.Contains(".json"))
                    continue;
                RecordEntryPack newRecording;
                //var filename2 = "C:/GitHub/azure-sdk-for-net/sdk/compute/Microsoft.Azure.Management.Compute/tests/SessionRecords/CloudServiceExtensionTests/MultiRole_CreateUpdateGetAndDeleteWithExtension_WorkerAndWebRole.json";
                using (FileStream fileStream = File.OpenRead(filename))
                {
                    try
                    {
                        var sanitizer = new RecordedTestSanitizer();
                        //using JsonDocument jsonDocument = JsonDocument.Parse(fileStream);
                        
                        newRecording = RecordEntryPack.Deserialize(filename); //replace old recording with new
                        if (!sanitizeFile(newRecording))
                            continue;
                    }
                    catch
                    {
                        continue;
                    }
                }

                //using FileStream fs = File.Create(filename);
                //var utf8JsonWriter = new Utf8JsonWriter(fs, new JsonWriterOptions()
                //{
                //    Indented = true
                //});
                newRecording.Serialize(filename);
                //utf8JsonWriter.Flush();

                Console.WriteLine(filename);
            }
        }

        private static bool sanitizeFile(RecordEntryPack newRecording)
        {
            try
            {
                foreach (var entry in newRecording.Entries)
                {
                    entry.RequestBody = RecorderUtilities.FormatString(entry.RequestBody);
                    entry.ResponseBody = RecorderUtilities.FormatString(entry.ResponseBody);
                }
                return true;
            }
            catch
            {
                return false;
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
