using System;
using System.Collections.Generic;
using Compete.Utils;

namespace Compete.Mis
{
    internal sealed class ConfigurationByService : IConfiguration
    {
        private readonly IDictionary<string, string> globalConfigurations;// = new Dictionary<string, string>();

        public ConfigurationByService() => globalConfigurations = Frame.Services.GlobalServices.FrameService.GetConfigurations();

        public object? GetConfig(ConfigurationNames name, Type type) => globalConfigurations.TryGetValue(Enum.GetName(typeof(ConfigurationNames), name)!, out string? value) ? Convert.ChangeType(value, type) : null;

        public T? GetConfig<T>(ConfigurationNames name) => (T?)GetConfig(name, typeof(T));
    }
}
