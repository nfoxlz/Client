using System;

namespace Compete.Utils
{
    public interface IConfiguration
    {
        object? GetConfig(ConfigurationNames name, Type type);

        T? GetConfig<T>(ConfigurationNames name);
    }

}
