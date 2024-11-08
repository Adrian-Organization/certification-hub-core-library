using CertificationHub.Core.Library.Entities;
using CertificationHub.Core.Library.Extensions;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace CertificationHub.Core.Library.Secrets;

public class SecretService(IHostEnvironment hostEnvironment) : ISecretService
{
    private const string LocalSecretsFileNameSuffix = "secrets.json";
    private AppSecrets? _appSecrets;

    public async Task InitAsync()
    {
        _appSecrets = await GetSecretsAsync();
    }
    
    public string? PostgresSqlUserManagementApiConnectionString => _appSecrets?.PostgresSqlUserManagementApiConnectionString;

    private async Task<AppSecrets> GetSecretsAsync()
    {
        //Configure for local development environment
        var serializedSecrets = string.Empty;
        
        if (hostEnvironment.IsLocalDevelopmentEnvironment())
        {
            var localEnvironment = hostEnvironment.EnvironmentName.Remove(hostEnvironment.EnvironmentName.LastIndexOf("Local", StringComparison.OrdinalIgnoreCase));
            var secretFile = FindSecretFile(hostEnvironment.ContentRootPath, localEnvironment);
            serializedSecrets = await File.ReadAllTextAsync(secretFile);
        }
        
        //Configure for docker -- TODO
        //----------------------------
        
        var secrets = JsonConvert.DeserializeObject<AppSecrets>(serializedSecrets);

        if (secrets is null)
        {
            throw new Exception("Could not retrieve secrets from current file");
        }
        
        return secrets;
    }

    private string FindSecretFile(string? startPath, string localEnvironment)
    {
        var fileName = $"{localEnvironment}-{LocalSecretsFileNameSuffix}";
        
        //navigate through parent directories to search for the secrets file
        while (startPath is not null)
        {
            var secretsFilePath = Path.Combine(startPath, fileName);
            if (File.Exists(secretsFilePath))
            {
                return secretsFilePath;
            }
            
            startPath = Directory.GetParent(startPath)?.FullName;
        }
        
        throw new Exception($"Specified file {fileName} could not be found");
    }
}