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
            TopChangesetId =
                new TopChangesetRequest(
                        new TopChangesetUri(BaseUrl, ItemPath),
                        new TfsPersonalAccessTokenAuthorization(PersonalAccessToken))
                    .Response()
                    .TopChangesetId();
            return true;
        }

        private sealed class TopChangesetUri
        {
            private readonly string _itemPath;
            private readonly string _baseUrl;

            public TopChangesetUri(string baseUrl, string itemPath)
            {
                _baseUrl = baseUrl;
                _itemPath = itemPath;
            }

            public static implicit operator Uri(TopChangesetUri uri)
            {
                return new Uri(
                    $"{uri._baseUrl}/_apis/tfvc/changesets?api-version=1.0&searchCriteria.itemPath={uri._itemPath}&searchCriteria.orderBy=id desc&$top=1");
            }
        }

        private sealed class TfsPersonalAccessTokenAuthorization
        {
            private readonly string _personalAccessToken;

            public TfsPersonalAccessTokenAuthorization(string personalAccessToken)
            {
                _personalAccessToken = personalAccessToken;
            }

            public static implicit operator AuthenticationHeaderValue(TfsPersonalAccessTokenAuthorization authorization)
            {
                return 
                    new AuthenticationHeaderValue(
                        "Basic",
                        Convert.ToBase64String(
                            Encoding.ASCII.GetBytes(
                                $":{authorization._personalAccessToken}")));
            }
        }

        private sealed class TopChangesetRequest
        {
            private readonly Uri _uri;
            private readonly AuthenticationHeaderValue _authorization;

            public TopChangesetRequest(Uri uri, AuthenticationHeaderValue authorization)
            {
                _uri = uri;
                _authorization = authorization;
            }

            public TopChangesetResponse Response()
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = _authorization;
                    httpClient.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                    return
                        new TopChangesetResponse(
                            httpClient.GetStringAsync(_uri).Result);
                }
            }
        }

        private sealed class TopChangesetResponse
        {
            private readonly string _response;

            public TopChangesetResponse(string response)
            {
                _response = response;
            }

            public int TopChangesetId()
            {
                using (var stringReader = new StringReader(_response))
                using (var jsonTextReader = new JsonTextReader(stringReader))
                {
                    dynamic responseAsJson =
                        new JsonSerializer()
                            .Deserialize(jsonTextReader);
                    return responseAsJson.value[0].changesetId;
                }
            }
        }
    }
}
