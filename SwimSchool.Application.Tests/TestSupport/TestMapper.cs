using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace SwimSchool.Application.Tests.TestSupport;

// The real AutoMapper, built exactly like the API builds it (AddApplication, including MaxDepth)
public static class TestMapper
{
    private static readonly Lazy<IServiceProvider> Provider =
        new(() => new ServiceCollection().AddApplication().BuildServiceProvider());

    public static IMapper Instance => Provider.Value.GetRequiredService<IMapper>();

    public static AutoMapper.IConfigurationProvider Configuration => Instance.ConfigurationProvider;
}
