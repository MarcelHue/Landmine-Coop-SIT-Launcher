namespace Landmine_Coop_SIT_Launcher
{
    public static class HttpClientUtils
    {
        public static async Task DownloadFileTaskAsync(this HttpClient client, Uri uri, string FileName)
        {
            await using var s = await client.GetStreamAsync(uri);
            await using var fs = new FileStream(FileName, FileMode.CreateNew);
            await s.CopyToAsync(fs);
        }
    }
}