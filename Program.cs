using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Octokit;
using NfdSharp;

namespace GithubDownloader
{
    class Program 
    {
        private static async Task Main(string[] args)
        {
            var client = new GitHubClient(new ProductHeaderValue("GithubDownloader"));
            var osInfo = Environment.OSVersion;
            
            var token = string.Empty;
            var path = string.Empty;
            
            var repoIdsDictionary = new Dictionary<int, Repository>();
            var assetIdsDictionary = new Dictionary<int, ReleaseAsset>();
            
            try
            {
                token = File.ReadAllText("token.txt");
            }
            catch (FileNotFoundException)
            {
                File.Create("token.txt");
                Console.WriteLine("Made a token.txt file as a previous one has not been found, please put your github token inside of it.");
                Environment.Exit(126);
            }

            try
            {
                var tokenAuth = new Credentials(token);
                client.Credentials = tokenAuth;
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Please place your github token in the token.txt file to proceed.");
                Environment.Exit(126);
            }

            try
            {
                await client.User.Current();
            }
            catch (AuthorizationException)
            {
                Console.WriteLine("Invalid token!");
                Environment.Exit(126);
            }
            
            Console.Write("Type the repo owner's username: ");
            var owner = Console.ReadLine();
            
            var repositories = client.Repository.GetAllForUser(owner);

            for (var i = 0; i < repositories.Result.Count; i++)
            {
                Console.WriteLine($"ID: {i + 1}, Repo: {repositories.Result[i].Url.Split('/').Last()}");
                repoIdsDictionary.Add(i + 1, repositories.Result[i]);
            }

            Console.Write($"\nSelect the id associated with the repo you'd like to use: ");
            var repoId = Convert.ToInt32(Console.ReadLine());

            if (repoIdsDictionary.ContainsKey(repoId))
            {
                Console.WriteLine("Valid repo!, Listing all downloads from latest release...");
                var releases = client.Repository.Release.GetAll(repoIdsDictionary[repoId].Id);
                var releaseAssets = client.Repository.Release.GetAllAssets(repoIdsDictionary[repoId].Id, releases.Result[0].Id);
                
                for (var i = 0; i < releaseAssets.Result.Count; i++)
                {
                    Console.WriteLine($"ID: {i + 1}, Asset: {releaseAssets.Result[i].Name}");
                    assetIdsDictionary.Add(i + 1, releaseAssets.Result[i]);
                }

                Console.Write($"\nSelect the id associated with the asset you'd like to download: ");
                var assetId = Convert.ToInt32(Console.ReadLine());

                if (assetIdsDictionary.ContainsKey(assetId))
                {
                    Console.WriteLine("Valid asset!, please select a folder to download the asset into\n");

                    switch (osInfo.Platform)
                    {
                        case PlatformID.Unix:
                            Nfd.PickFolder("", out path);
                            break;
                        case PlatformID.Win32NT:
                            Console.Write("Windows folder browsing not supported! Please specify path instead (eg: D:/Repo1/src): ");
                            path = Console.ReadLine();
                            break;
                        case PlatformID.MacOSX:
                            Console.Write("MacOS folder browsing not supported! Please specify path instead (eg: D:/Repo1/src): ");
                            path = Console.ReadLine();
                            break;
                    }

                    Console.Write("Would you like the file to be automatically extracted? (Y/n) ");
                    var extract = Console.ReadLine();
                    
                    WebClient webClient = new WebClient();
                    webClient.Headers.Add("authorization", $"token {token}");

                    await webClient.DownloadFileTaskAsync(new Uri(assetIdsDictionary[assetId].BrowserDownloadUrl), $"{path}/{assetIdsDictionary[assetId].Name}");

                    Console.WriteLine($"\nSuccessfully downloaded {assetIdsDictionary[assetId].Name}!");
                    if (extract == "Y")
                    {
                        Console.WriteLine($"Extracting {assetIdsDictionary[assetId].Name}...");
                        ZipFile.ExtractToDirectory($"{path}/{assetIdsDictionary[assetId].Name}", path ?? string.Empty, true);
                        File.Delete($"{path}/{assetIdsDictionary[assetId].Name}");
                    }

                    Console.WriteLine("Done!");
                }
            }

            Console.ReadLine();
        }
    }
}
