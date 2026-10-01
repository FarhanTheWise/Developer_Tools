using UnityEngine;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using UnityEditor;

namespace FarhanAfzal
{
    public static class PackageLoader
    {
        private const string gistUser = "FarhanTheWise";
        private const string gistId = "d5df2be052b3266065fba62b5f12f193";


        [MenuItem("MyTools/Load Package Manifest")]
        static async Task LoadPackageManifestAsync()
        {
            var gistUrl = GetGistUrl();
            var contentTask = await GetGistContent(gistUrl);
            ReplaceManifestFile(contentTask);
        }

        static string GetGistUrl()
        {
            return $"https://gist.githubusercontent.com/{gistUser}/{gistId}/raw";
        }

        static async Task<string> GetGistContent(string url)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                var responseContent = await response.Content.ReadAsStringAsync();
                return responseContent;
            }
        }

        static void ReplaceManifestFile(string content)
        {
            var existingManifest = Path.Combine(Application.dataPath, "../Packages/manifest.json");
            File.WriteAllText(existingManifest, content);
            UnityEditor.PackageManager.Client.Resolve();
        }
    }
}
