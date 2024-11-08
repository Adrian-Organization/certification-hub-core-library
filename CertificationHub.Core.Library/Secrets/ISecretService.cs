namespace CertificationHub.Core.Library.Secrets;

public interface ISecretService
{ 
    string? PostgresSqlUserManagementApiConnectionString { get; }

    internal Task InitAsync();
}