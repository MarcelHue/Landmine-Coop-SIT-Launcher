using System.Diagnostics;
using static System.Int32;

namespace Landmine_Coop_SIT_Launcher
{
    public class Launcher
    {
        public void Run()
        {
            if(!HelperUtils.ValidateStartup())
            {
                return;
            }
            
            var installPath = HelperUtils.GetInstallPath();
            if(ValidateAndUpdate())
            {
                ManageConfig(installPath);
                ManagePlugins(installPath);
            }
            
            Process.Start(Path.Combine(HelperUtils.BaseDirectoryPath, "SIT.Manager.exe"));
        }
        
        private static void ManageConfig(string installPath)
        {
            foreach (var file in Directory.GetFiles(Path.Combine(HelperUtils.CoopModsPath, "config")))
            {
                File.Copy(file, Path.Combine(Path.Combine(installPath, "BepInEx", "config"), Path.GetFileName(file)), true);
            }
        }
        
        private static void ManagePlugins(string installPath)
        {
            HelperUtils.RecursiveDelete(Path.Combine(installPath, "BepInEx", "plugins"));
            
            HelperUtils.CopyFilesRecursively(Path.Combine(HelperUtils.CoopModsPath, "plugins"), Path.Combine(installPath, "BepInEx", "plugins"));
            HelperUtils.CopyFilesRecursively(HelperUtils.CustomModsPath, Path.Combine(installPath, "BepInEx" , "plugins"));
        }
        
        private static bool ValidateAndUpdate()
        {
            var googleDownloader = new GoogleDownloader();
            
            var localVersion = HelperUtils.GetLocalVersion();
            _ = TryParse(googleDownloader.GetContentAsString(HelperUtils.VersionId).Replace(".", string.Empty), out var onlineVersion);
            if(localVersion < onlineVersion)
            {
                HelperUtils.RecursiveDelete(HelperUtils.CoopModsPath);
                Directory.CreateDirectory(HelperUtils.CoopModsPath);
                googleDownloader.GetGoogleFolderFolder(HelperUtils.FolderId, HelperUtils.CoopModsPath);
                Task.WaitAll(googleDownloader.Downloads.ToArray());
                return true;
            }
            
            return false;
        }
    }
}