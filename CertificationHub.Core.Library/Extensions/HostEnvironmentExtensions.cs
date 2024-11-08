using Microsoft.Extensions.Hosting;

namespace CertificationHub.Core.Library.Extensions;

public static class HostEnvironmentExtensions
{
    public static bool IsLocalDevelopmentEnvironment(this IHostEnvironment hostEnvironment)
    {
        return hostEnvironment.EnvironmentName.EndsWith("Local", StringComparison.OrdinalIgnoreCase);
    }
}