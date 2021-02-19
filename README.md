## GitDownloader
GitHub CLI utility that lets you download repositories' release assets without the need of opening a browser and searching

**Features:**
* Lists all you might need to know when choosing the repository and release asset.
* Allows for instant zip file extraction (other zip files like tar.gz should work).
* Cross platform support for Windows and unix systems

**How to use:**
1. Download and extract the latest release and unzip the contents.
2. You can either make a `token.txt` file or run `GihubtDownloader.exe` for the first time and let it make one for you.
3. Place your github token into the `token.txt` file.
4. Re-run `GithubDownloader.exe` and enjoy!.

## Building

**Prerequisites to build**:
* 64bit dotnet 5.0

**How to build:**
* Enter the downloaded folder and run `dotnet run` in the current directory.
* Build files will be in `~/bin/Debug/net5.0/`.

## Packages used:

[**octokit.net**](https://github.com/octokit/octokit.net) - used for interation with the GitHub API

[**nfd-sharp**](https://github.com/benklett/nfd-sharp) - used for the folder browser as a wrapper for the [nativefiledialog](https://github.com/mlabbe/nativefiledialog) library

## Why do you need a token?

The need for a GitHub token is there for the webclient to provide the token for downloading files from GitHub, as well as for increasing the API request limit, allowing you to make more requests if that is what you might want.

If you don't know how to obtain your token, you can recieve it [here](https://github.com/settings/tokens).
