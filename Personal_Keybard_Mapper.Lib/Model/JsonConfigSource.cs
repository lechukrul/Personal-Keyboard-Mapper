using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Personal_Keyboard_Mapper.Lib.Converters;
using Personal_Keyboard_Mapper.Lib.Interfaces;

namespace Personal_Keyboard_Mapper.Lib.Model
{
    public class JsonConfigSource : IConfigSource
    {
        public ILog logger { get; set; }
        public string ConfigString { get; set; }

        public string ConfigFilePath { get; set; }
        public JsonConfigSource(string path)
        {
            ConfigFilePath = path;
            ReadConfigStringFromFile(ConfigFilePath);
        }

        public JsonConfigSource(ILog log, string path)
        {
            logger = log;
            ConfigFilePath = path;
            ReadConfigStringFromFile(ConfigFilePath);
        }

        private void ReadConfigStringFromFile(string path)
        {
            ConfigString = (String.Join("", File.ReadLines(path))
                .Replace("\\\"", "/'"));
        }


        /// <inheritdoc />
        public KeyCombinationsConfiguration ReadConfigFromString(string jsonSource = "")
        {
            if (string.IsNullOrEmpty(ConfigString))
            {
                if (!string.IsNullOrEmpty(jsonSource))
                {
                    ConfigString = jsonSource;
                }
                else
                {
                    throw new NullReferenceException(nameof(ConfigString));
                }
            }

            KeyCombinationsConfiguration combinations = null;
            if (logger == null)
            {
                combinations = JsonConvert.DeserializeObject<KeyCombinationsConfiguration>(ConfigString,
                    new CombinationsConfigurationConverter());
            }
            else
            {
                combinations = JsonConvert.DeserializeObject<KeyCombinationsConfiguration>(ConfigString,
                    new CombinationsConfigurationConverter(logger));
            }
            return combinations;
        }
         
        public KeyCombinationsConfiguration ReadConfigFromDbSet(DbSet config)
        {
            throw new NotImplementedException();
        }

        public void WriteConfigToString(KeyCombinationsConfiguration configuration, string configFileName = "")
        {
            if (configFileName != "")
            {
                ConfigFilePath = configFileName; 
            }

            var result = JsonConvert.SerializeObject(configuration, new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                Converters = new List<JsonConverter>(){new CombinationsConfigurationConverter()}
            });
            using (var file = File.CreateText("config.txt"))
            {
                file.Write(result.ToCharArray());
            }
        }

        public void WriteConfigToDbSet(KeyCombinationsConfiguration configuration, DbSet config)
        {
            throw new NotImplementedException();
        }
    }
}