using Octokit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Wpf.Ui.Controls;

namespace EZAutoclickerWPF.VersionControl
{
    internal static class Versions
    {
        public static async Task CheckGitHubNewerVersion()
        {
            //Get all releases from GitHub
            //Source: https://octokitnet.readthedocs.io/en/latest/getting-started/
            GitHubClient client = new GitHubClient(new Octokit.ProductHeaderValue("EZAC"));
            IReadOnlyList<Release> releases = await client.Repository.Release.GetAll("Glumboi", "EZAutoclicker-Remake");

            //Setup the versions
            Version latestGitHubVersion = new Version(releases[0].TagName);
            Assembly Reference = typeof(MainWindow).Assembly;
            Version Version = Reference.GetName().Version;
            Version localVersion = new Version(Version.ToString());

            //Compare the Versions
            //Source: https://stackoverflow.com/questions/7568147/compare-version-numbers-without-using-split-function
            int versionComparison = localVersion.CompareTo(latestGitHubVersion);
            if (versionComparison < 0)
            {
                MessageBoxResult result = System.Windows.MessageBox.Show("You got an outdated version of EZAC installed would you like to " +
                    "open the repo page?",
                    "Info",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Do this
                    Web.WebBrowser.OpenUrl("https://github.com/Glumboi/EZAutoclicker/releases");
                }
                else
                {
                    return;
                }
                //The version on GitHub is more up to date than this local release.
            }
            else if (versionComparison > 0)
            {
                //This local version is greater than the release version on GitHub.
            }
            else
            {
                //This local Version and the Version on GitHub are equal.
            }

            Debug.WriteLine(versionComparison);
        }
    }
}