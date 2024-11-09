using System.Reflection;
using CertificationHub.Core.Library.IoC.DependencyInjectionHelpers;
using CertificationHub.Core.Library.Secrets;
using Microsoft.Extensions.DependencyInjection;

namespace CertificationHub.Core.Library.IoC.Library;

public class CoreLibrary : ILibraryInitialisationService
{
    public void RegisterLibraryServicesInContainer(IServiceCollection serviceCollection)
    {
        serviceCollection.RegisterDependenciesFromAssembly(Assembly.GetExecutingAssembly());
        serviceCollection.AddSingleton<ISecretService, SecretService>();
    }
    
    public async Task InitialiseLibraryAsync(IServiceProvider serviceProvider)
    {
        var secretService = serviceProvider.GetRequiredService<ISecretService>();
        await secretService.InitAsync();
    }
}