using AutoMapper;

using MoneyGroup.Infrastucture.AutoMapper.Profiles;

namespace MoneyGroup.IntegrationTests.AutoMapper.Profiles;
public class OrderProfileTest
{
    [Fact]
    public void OrderProfile_ShouldHaveValidMappings()
    {
        // Arrange
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<OrderProfile>();
        });

        // Assert
        configuration.AssertConfigurationIsValid();
    }
}