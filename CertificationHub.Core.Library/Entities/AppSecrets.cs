using Newtonsoft.Json;

namespace CertificationHub.Core.Library.Entities;

internal class AppSecrets
{
    [JsonProperty] internal string? PostgresSqlUserManagementApiConnectionString { get; set; }
}