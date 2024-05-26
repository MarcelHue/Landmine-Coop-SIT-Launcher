using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Landmine_Coop_SIT_Launcher
{
    public class GoogleDownloader
    {
        public readonly List<Task> Downloads = [];
        
        public string GetContentAsString(string id)
        {
            var wb = new HttpClient();
            var response = wb.GetAsync($"https://drive.google.com/uc?export=download&id={id}").Result;
            using var fs = new MemoryStream();
            response.Content.CopyToAsync(fs);
            return StreamToString(fs);
        }
        
        private static Task DownloadFile(string id, string path)
        {
            var wb = new HttpClient();
            var uri = new Uri($"https://drive.google.com/uc?export=download&id={id}");
            return wb.DownloadFileTaskAsync(uri, path);
        }
        
        private static string StreamToString(Stream stream)
        {
            stream.Position = 0;
            using var reader = new StreamReader(stream, Encoding.UTF8);
            return reader.ReadToEnd();
        }
        
        public void GetGoogleFolderFolder(string publicFolderId, string path)
        {
            var httpClient = new HttpClient();
            var nextPageToken = "";
            do
            {
                var folderContentsUri = $"https://www.googleapis.com/drive/v3/files?q='{publicFolderId}'+in+parents&key={HelperUtils.GoogleDriveApiKey}";
                if (!string.IsNullOrEmpty(nextPageToken))
                {
                    folderContentsUri += $"&pageToken={nextPageToken}";
                }
                var contentsJson = httpClient.GetStringAsync(folderContentsUri).Result;
                var contents = (JObject)JsonConvert.DeserializeObject(contentsJson);
                if(contents != null)
                {
                    nextPageToken = (string)contents["nextPageToken"];
                    foreach(var file in((JArray)contents["files"])!)
                    {
                        var id = (string)file["id"];
                        var name = (string)file["name"];
                        var mimeType = (string)file["mimeType"];
                        if(name is null or"1. Baby Bots" or"2. Less Difficult" or"3. Default" or"4. I Like Pain" or"5. Death Wish")
                        {
                            continue;
                        }
                        var combine = Path.Combine(path, name);
                        Console.WriteLine($"Downloading ... :{name} To [{combine}]");
                        if(mimeType != null && mimeType.Contains("folder"))
                        {
                            Directory.CreateDirectory(combine);
                            this.GetGoogleFolderFolder(id, combine);
                        }
                        else
                        {
                            this.Downloads.Add(DownloadFile(id, combine));
                        }
                    }
                }
            } while (!string.IsNullOrEmpty(nextPageToken));
        }
    }
}