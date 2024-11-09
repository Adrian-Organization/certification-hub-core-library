namespace CertificationHub.Core.Library.IoC.DependencyInjectionHelpers;

[AttributeUsage(AttributeTargets.Class)]
public class RegistrationKindAttribute : Attribute
{
    public RegistrationType Type { get; set; }
    public bool AsSelf { get; set; }
    public string? InjectionInterface { get; set; }
}