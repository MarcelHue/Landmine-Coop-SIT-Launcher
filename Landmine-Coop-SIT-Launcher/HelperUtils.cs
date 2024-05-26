using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static System.Int32;

namespace Landmine_Coop_SIT_Launcher
{
    public class HelperUtils
    {
        public static string BaseDirectoryPath;
        public static string CoopModsPath;
        public static string CustomModsPath;
        public const string GoogleDriveApiKey = "AIzaSyD4cdKjIoC1FKDVsrgZCFPBcYl8yLNsrD8";
        public const string VersionId = "1HRr3ldQYSEqlg4LhFtCapkL1VDeNRunc";
        public const string FolderId = "1BNxQdxNUPUmeqsw3sXTqpnNYZN0jnpJV";
        
        public static void RecursiveDelete(DirectoryInfo baseDir)
        {
            if (!baseDir.Exists)
            {
                return;
            }
            
            foreach (var dir in baseDir.EnumerateDirectories())
            {
                RecursiveDelete(dir);
            }
            baseDir.Delete(true);
        } 
        
        public static void RecursiveDelete(string baseDir) => RecursiveDelete(new DirectoryInfo(baseDir));
        
        public static void CopyFilesRecursively(string sourcePath, string targetPath)
        {
            //Now Create all of the directories
            foreach (var dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
            }
            
            //Copy all the files & Replaces any files with the same name
            foreach (var newPath in Directory.GetFiles(sourcePath, "*.*",SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
            }
        }
        
        public static int GetLocalVersion()
        {
            var path = Path.Combine(CoopModsPath,"Version.txt");
            if(!File.Exists(path))
            {
                return 0;
            }
            
            var readAllLines = File.ReadAllLines(path);
            var firstline = readAllLines[0].Replace(".", string.Empty);
            
            _ = TryParse(firstline, out var number);
            return number;
        }
        
        public static bool ValidateStartup()
        {
            BaseDirectoryPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            Console.WriteLine(BaseDirectoryPath);
            if(BaseDirectoryPath != null)
            {
                CoopModsPath = Path.Combine(BaseDirectoryPath, "CoopMods");
                CustomModsPath = Path.Combine(BaseDirectoryPath, "CustomMods");
                if(string.IsNullOrWhiteSpace(BaseDirectoryPath))
                {
                    return false;
                }
                
                var files = Directory.GetFiles(BaseDirectoryPath);
                if(files.All(x => Path.GetFileName(x) != "SIT.Manager.exe"))
                {
                    return false;
                }
            }
            
            if(!Directory.Exists(CoopModsPath))
            {
                Directory.CreateDirectory(CoopModsPath);
            }
            if(!Directory.Exists(CustomModsPath))
            {
                Directory.CreateDirectory(CustomModsPath);
            }
            
            return true;
        }
        
        public static string GetInstallPath()
        {
            using var streamReader = new StreamReader(Path.Combine(BaseDirectoryPath, "ManagerConfig.json"), Encoding.UTF8);
            var readContents = streamReader.ReadToEnd();
            var contents = (JObject)JsonConvert.DeserializeObject(readContents);
            return contents["SitEftInstallPath"].ToString();
        }
        
    }
}