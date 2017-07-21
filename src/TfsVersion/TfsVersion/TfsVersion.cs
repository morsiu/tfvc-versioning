using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace TfsVersion
{
    public sealed class TfsVersion : Task
    {
        [Output]
        public int TopChangesetId { get; set; }

        [Required]
        public string BaseUrl { get; set; }

        [Required]
        public string PersonalAccessToken { get; set; }

        public string ItemPath { get; set; }

        public override bool Execute()
        {
            var uri = new Uri($"{BaseUrl}/_apis/tfvc/changesets?api-version=1.0&searchCriteria.itemPath={ItemPath}&searchCriteria.orderBy=id desc&$top=1");
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($":{PersonalAccessToken}")));
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            dynamic response = new JsonSerializer().Deserialize(new JsonTextReader(new StringReader(httpClient.GetStringAsync(uri).Result)));
            TopChangesetId = (int)response.value[0].changesetId;
            return true;
        }
    }
}
