namespace sameplens;

public class ServiceCollectionContainerBuilderExtensions
{
    private static readonly Type __generated_type = Type.GetType("Microsoft.Extensions.DependencyInjection.ServiceCollectionContainerBuilderExtensions, Microsoft.Extensions.DependencyInjection");

    public Microsoft.Extensions.DependencyInjection.ServiceProvider BuildServiceProvider(Microsoft.Extensions.DependencyInjection.IServiceCollection services)
    {
        var __generated_method = __generated_type.GetMethod("BuildServiceProvider", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.InvokeMethod);
        var __generated_result = __generated_method.Invoke(null, services);
        return __generated_result as Microsoft.Extensions.DependencyInjection.ServiceProvider;
    }

    public Microsoft.Extensions.DependencyInjection.ServiceProvider BuildServiceProvider(Microsoft.Extensions.DependencyInjection.IServiceCollection services, System.Boolean validateScopes)
    {
        var __generated_method = __generated_type.GetMethod("BuildServiceProvider", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.InvokeMethod);
        var __generated_result = __generated_method.Invoke(null, services, validateScopes);
        return __generated_result as Microsoft.Extensions.DependencyInjection.ServiceProvider;
    }

    public Microsoft.Extensions.DependencyInjection.ServiceProvider BuildServiceProvider(Microsoft.Extensions.DependencyInjection.IServiceCollection services, Microsoft.Extensions.DependencyInjection.ServiceProviderOptions options)
    {
        var __generated_method = __generated_type.GetMethod("BuildServiceProvider", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.InvokeMethod);
        var __generated_result = __generated_method.Invoke(null, [services, options]);
        return __generated_result as Microsoft.Extensions.DependencyInjection.ServiceProvider;
    }
}

