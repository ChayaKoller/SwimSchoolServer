using SwimSchool.Application.Tests.TestSupport;

namespace SwimSchool.Application.Tests.Mapping;

public class MappingConfigurationTests
{
    // Fails if a DTO field has no source, so a new field never silently stays empty
    [Fact]
    public void AutoMapperConfiguration_IsValid()
    {
        TestMapper.Configuration.AssertConfigurationIsValid();
    }
}
