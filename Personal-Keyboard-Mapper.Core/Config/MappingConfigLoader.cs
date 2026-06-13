using System;
using System.IO;
using System.Text.Json;

namespace Personal_Keyboard_Mapper.Core.Config
{
    public static class MappingConfigLoader
    {
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public static MappingConfig Load(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"Config file not found: {path}");

            var json = File.ReadAllText(path);
            var config = JsonSerializer.Deserialize<MappingConfig>(json, Options);

            if (config == null)
                throw new InvalidOperationException($"Failed to deserialize config from: {path}");
            if (config.Combinations == null)
                throw new InvalidOperationException("Config is missing 'Combinations' array.");

            return config;
        }
    }
}
