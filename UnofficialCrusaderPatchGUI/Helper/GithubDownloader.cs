
//using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UCP.Helper
{
    //public class GithubDownloader
    //{
    //    public enum UpdateType
    //    {
    //        None,
    //        Major,
    //        Minor,
    //        Patch
    //    }

    //    private IReleasesClient _releaseClient;
    //    internal GitHubClient Github;

    //    internal String CurrentVersion;
    //    internal string RepositoryOwner;
    //    internal string RepostoryName;
    //    internal Release LatestRelease;
    //    public GithubDownloader(string owner, string name, string version = null)
    //    {
    //        Github = new GitHubClient(new ProductHeaderValue(name + @"-UpdateCheck"));
    //        _releaseClient = Github.Release;

    //        RepositoryOwner = owner;
    //        RepostoryName = name;
    //        CurrentVersion = version;
    //    }

    //    public async Task<UpdateType> CheckUpdate(UpdateType locked = UpdateType.None)
    //    {
    //        var releases = await _releaseClient.GetAll(RepositoryOwner, RepostoryName);
    //        if (releases[0].TagName.Equals(CurrentVersion)) return UpdateType.None;
    //        LatestRelease = releases[0];

    //        return UpdateType.Major;
    //    }

    //    internal async void DownloadAsset(string assetname)
    //    {
    //        // asset.Url is some api wizardry that we'll maybe use later
    //        var assets = await _releaseClient.GetAssets(RepositoryOwner, RepostoryName, LatestRelease.Id);

    //        Dictionary<String, String> dummy;

    //        var client = new GitHubClient(new ProductHeaderValue("UCP-New-GUI"));
    //        var dummy1 = client.User.Email;
    //        var createIssue = new NewIssue("this thing doesn't work");
    //        try
    //        {
    //            var issue = await client.Issue.Create("PodeCaradox", "PodeCaradox", createIssue);
    //        }
    //        catch (Exception e)
    //        {

            
    //        }
           
    //    }
    //}
}
