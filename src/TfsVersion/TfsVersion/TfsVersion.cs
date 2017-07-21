using System;
using Microsoft.Build.Utilities;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.Build.Framework;

namespace TfsVersion
{
    public sealed class TfsVersion : Task
    {
        public override bool Execute()
        {
            new RedirectionOfNewtonsoftJsonBetween0And8To8(); // Ensure its static constructor runs
            TopChangesetId = 
                new TfvcHttpClient(
                        new Uri(BaseUrl),
                        new VssBasicCredential(string.Empty, PersonalAccessToken))
                    .GetChangesetsAsync(
                        project: Project,
                        orderby: "ChangesetId",
                        searchCriteria: new TfvcChangesetSearchCriteria { ItemPath = ItemPath },
                        top: 1)
                    .Result[0]
                    .ChangesetId;
            return true;
        }

        [Output]
        public int TopChangesetId { get; set; }

        [Required]
        public string Project { get; set; }

        [Required]
        public string BaseUrl { get; set; }

        [Required]
        public string PersonalAccessToken { get; set; }

        public string ItemPath { get; set; }
    }
}
