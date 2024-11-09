using Microsoft.Extensions.DependencyInjection;

namespace CertificationHub.Core.Library.IoC.Library;

public interface ILibraryInitialisationService
{
    public void RegisterLibraryServicesInContainer(IServiceCollection serviceCollection);
    public Task InitialiseLibraryAsync(IServiceProvider serviceProvider);
}