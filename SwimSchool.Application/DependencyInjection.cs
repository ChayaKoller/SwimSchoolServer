using AutoMapper.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace SwimSchool.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // CVE-2026-32933 (GHSA-rvv3-g6hj-g44x): cap recursion depth on every map
        services.AddAutoMapper(
            cfg => cfg.Internal().ForAllMaps((map, expr) => expr.MaxDepth(64)),
            typeof(DependencyInjection).Assembly);

        return services;
    }
}
