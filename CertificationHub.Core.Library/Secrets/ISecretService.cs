namespace CertificationHub.Core.Library.Secrets;

public interface ISecretService
{ 
    string? PostgresSqlUserManagementApiConnectionString { get; }

    public Task InitAsync();
    public Task RefreshSecretsAsync();
}