using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace CertificationHub.Core.Library.IoC.DependencyInjectionHelpers;

public static class ServiceCollectionUtilities
{
    private static IServiceCollection? Services { get; set; }
    private static List<Assembly>? Assemblies { get; set; }

    public static void InitializeAssemblies(string solutionName, List<Assembly> assembliesToLoad)
    {
        assembliesToLoad.ForEach(a => Assembly.Load(a.GetName()));
        
        Assemblies = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(t => t.FullName != null && t.FullName.Contains(solutionName))
            .ToList();
    }

    public static void InjectServicesForNamespace(this IServiceCollection services, string nameSpace)
    {
        Services = services;
        InjectDependencyForAssembly(
            Assemblies!.FirstOrDefault(t => nameSpace.Contains(t.GetName().Name ?? string.Empty)), nameSpace);
    }

    public static void RegisterDependenciesFromAssembly(this IServiceCollection services, Assembly? assembly, string? nameSpace = null)
    {
        Services = services;
        InjectDependencyForAssembly(assembly, nameSpace);
    }

    private static void InjectDependencyForAssembly(Assembly? assembly, string? nameSpace)
    {
        if (assembly is null)
        {
            return;
        }
        
        var registrationTypes = assembly.GetTypes()
            .Where(type => nameSpace == null || type.Namespace == nameSpace && type.Namespace.Contains(nameSpace, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
        
        registrationTypes.ForEach(RegisterType);
    }

    private static void RegisterType(Type registrationType)
    {
        var registrationKind = registrationType.GetCustomAttribute<RegistrationKindAttribute>();

        if (registrationKind is null)
        {
            return;
        }

        Type? registrationBaseType = null;

        if (!registrationKind.AsSelf)
        {
            if (string.IsNullOrEmpty(registrationKind.InjectionInterface))
            {
                registrationBaseType = registrationType.GetInterfaces().FirstOrDefault();

                if (registrationBaseType is null)
                {
                    throw new Exception($"No interface for type '{registrationType.FullName}' has been found");
                }
            }
            else
            {
                registrationBaseType = registrationType.GetInterfaces().FirstOrDefault(type => type.Name == registrationKind.InjectionInterface);

                if (registrationBaseType is null)
                {
                    throw new Exception($"Interface not found '{registrationKind.InjectionInterface}'");
                }
            }
        }
        
        switch (registrationKind.Type)
        {
            case RegistrationType.Scoped:
            {
                if (registrationBaseType is null)
                {
                    Services?.AddScoped(registrationType);
                }
                else
                {
                    Services?.AddScoped(registrationBaseType, registrationType);
                }
            } break;
            
            case RegistrationType.Singleton:
            {
                if (registrationBaseType is null)
                {
                    Services?.AddSingleton(registrationType);
                }
                else
                {
                    Services?.AddSingleton(registrationBaseType, registrationType);
                }
            } break;
                
            case RegistrationType.Transient:
            {
                if (registrationBaseType is null)
                {
                    Services?.AddTransient(registrationType);
                }
                else
                {
                    Services?.AddTransient(registrationBaseType, registrationType);
                }
            } break;

            default:
            {
                if (registrationBaseType is null)
                {
                    Services?.AddScoped(registrationType);
                }
                else
                {
                    Services?.AddScoped(registrationBaseType, registrationType);
                }
            } break;
        }
    }
}