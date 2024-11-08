using CertificationHub.Core.Library.Secrets;
using Microsoft.Extensions.Hosting;
using Moq;

namespace CertificationHub.Core.Library.Tests;

public class SecretServiceTests
{
    private readonly Mock<IHostEnvironment> _hostEnvironmentMock;
    private readonly SecretService _secretService;

    public SecretServiceTests()
    {
        _hostEnvironmentMock = new Mock<IHostEnvironment>();
        _secretService = new SecretService(_hostEnvironmentMock.Object);
    }

    [Fact]
    public async Task InitAsync_ShouldLoadSecrets_WhenInLocalDevelopmentEnvironment()
    {
        //Arrange
        _hostEnvironmentMock.Setup(env => env.EnvironmentName).Returns("testLocal");
        _hostEnvironmentMock.Setup(env => env.ContentRootPath).Returns(Directory.GetCurrentDirectory());
        
        var mockSecretsFilePath = Path.Combine(Directory.GetCurrentDirectory(), "test-secrets.json");
        const string mockSecretsContent = "{ \"PostgresSqlUserManagementApiConnectionString\": \"SampleConnectionString\" }";
        await File.WriteAllTextAsync(mockSecretsFilePath, mockSecretsContent);

        try
        {
            //Act
            await _secretService.InitAsync();

            //Assert
            Assert.NotNull(_secretService.PostgresSqlUserManagementApiConnectionString);
            Assert.Equal("SampleConnectionString", _secretService.PostgresSqlUserManagementApiConnectionString);
        }
        finally
        {
            //Clean
            if (File.Exists(mockSecretsFilePath))
            {
                File.Delete(mockSecretsFilePath);
            }
        }
    }

    [Fact]
    public async Task InitAsync_ShouldThrowException_WhenSecretsFileNotFound()
    {
        // Arrange
        _hostEnvironmentMock.Setup(env => env.EnvironmentName).Returns("testLocal");
        _hostEnvironmentMock.Setup(env => env.ContentRootPath).Returns(Directory.GetCurrentDirectory());

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _secretService.InitAsync());
        Assert.Equal("Specified file test-secrets.json could not be found", exception.Message);
    }
    
    [Fact]
    public async Task GetSecretsAsync_ShouldThrowException_WhenSecretsAreInvalid()
    {
        // Arrange
        _hostEnvironmentMock.Setup(env => env.EnvironmentName).Returns("testLocal");
        _hostEnvironmentMock.Setup(env => env.ContentRootPath).Returns(Directory.GetCurrentDirectory());

        var mockSecretsFilePath = Path.Combine(Directory.GetCurrentDirectory(), "test-secrets.json");
        var mockInvalidContent = "";
        await File.WriteAllTextAsync(mockSecretsFilePath, mockInvalidContent);

        try
        {
            // Act
            await _secretService.InitAsync();
        }
        catch (Exception ex)
        {
            // Assert
            Assert.Equal("Could not retrieve secrets from current file", ex.Message);
            return;
        }

        // Fail the test if no exception is thrown
        Assert.Fail("Expected exception was not thrown.");
    }
}