using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Octokit;


namespace Rig
{
    class VersionManager
    {
        private int releaseId = 0;
        private Version gitVersion = new Version();
        private string zipPath;
        private static GitHubClient Client => new GitHubClient(new Octokit.ProductHeaderValue(Header));
        private static ProcessStartInfo startInfo = new ProcessStartInfo();
        private string newVersionFolderPath;
        public VersionManager()
        {
            RigEx.WriteLineColors($"Check for Update".AddTimeStamp(), ConsoleColor.DarkGray);
            var release = GetLatestRelease();
            release.Wait();
            Version.TryParse(release.Result.TagName, out gitVersion);
            RigEx.WriteLineColors($"curent version:\t{RigEx.MyVersion}".AddTimeStamp(),ConsoleColor.Gray);
            RigEx.WriteLineColors($"last  version:\t{gitVersion}".AddTimeStamp(),ConsoleColor.Gray);
            if (gitVersion <= RigEx.MyVersion)
            {
                RigEx.WriteLineColors($"you are using the latest version".AddTimeStamp(), ConsoleColor.Green);
                return;
            }
            RigEx.WriteLineColors($"Begin update version".AddTimeStamp(), ConsoleColor.Green);
            releaseId = release.Result.Assets[0].Id;
            var a = assetsid();
            var resp = Download(a);
            SafeToZip(resp);
            Unzip(zipPath);
            StartProgram();
        }

        private ReleaseAsset assetsid()
        {
            if (releaseId == 0)
            {
                Console.WriteLine("error");
                return null;
            }
            Task<ReleaseAsset> assets = Client.Repository.Release.GetAsset(Owner, RepositoryName, releaseId);
            assets.Wait();
            return assets.Result;
        }

        public const string Header = "some-stupid-text";

        public const string Owner = "Serghii";

        public const string RepositoryName = "Rig";

        public IApiResponse<byte[]> Download(ReleaseAsset assets)
        {
            RigEx.WriteLineColors($"start download".AddTimeStamp(), ConsoleColor.DarkGray);
            var resp = Client.Connection.Get<byte[]>(new Uri(assets.BrowserDownloadUrl),
                new Dictionary<string, string>(),
                null);
            resp.Wait();
            RigEx.WriteLineColors($"download done".AddTimeStamp(), ConsoleColor.DarkGray);
            return resp.Result;
        }

        public void SafeToZip(IApiResponse<byte[]> resp)
        {

            using (var zipstream = new MemoryStream((byte[])resp.Body))
            using (var archive = new ZipArchive(zipstream))
            {
                var entry = archive.Entries[0];
                var dat = new byte[entry.Length];
                entry.Open().ReadAsync(dat, 0, dat.Length);
            }
            var responseData = resp.HttpResponse.Body;
            zipPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(RigEx.MainFolderPath, $"{gitVersion.ToString()}.zip"));
            System.IO.File.WriteAllBytes(zipPath, (byte[])responseData);
        }

        private void Unzip(string zipPath)
        {
            try
            {
                Console.WriteLine("start UnZip");
                newVersionFolderPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(RigEx.MainFolderPath, $"{gitVersion.ToString()}"));
                ZipFile.ExtractToDirectory(zipPath,newVersionFolderPath);
                Console.WriteLine("UnZipDir - DONE");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unzip Error: {e.Message}");
                if (!e.Message.Contains("already exists"))
                {
                    RigEx.QuitApp(30000);
                }
            }
        }
        private void StartProgram()
        {
            try
            {
                Console.WriteLine($"launch application {gitVersion.ToString()}");
                startInfo.CreateNoWindow = true;
                startInfo.UseShellExecute = true;
                startInfo.FileName = $"{newVersionFolderPath}\\Rig.exe";
                startInfo.WindowStyle = ProcessWindowStyle.Normal;
                Process.Start(startInfo);
            }
            catch (Exception e)
            {
                RigEx.WriteLineColors($"Start Program Error: {e.Message}", ConsoleColor.Red);
                Console.ReadLine();
            }
        }
        public async Task<Release> GetLatestRelease()
        {
            return await Client.Repository.Release.GetLatest(Owner, RepositoryName);
        }

    }
}
