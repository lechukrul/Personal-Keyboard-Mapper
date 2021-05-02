using System;
using System.Collections.Generic;
using System.Configuration;
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
        public JsonConfigSource(ILog log)
        {
            logger = log;
        }

        public JsonConfigSource(ILog log, string path)
        {
            logger = log;
            if (!path.Contains(ConfigurationManager.AppSettings["DefaultConfigFileExtension"]))
            {
                path += $".{ConfigurationManager.AppSettings["DefaultConfigFileExtension"]}";
            }
            ReadConfigFromFile(path);
        }

        public KeyCombinationsConfiguration ReadConfigFromFile(string path = "")
        {
            if (!string.IsNullOrEmpty(path))
            {
                ConfigFilePath = path;
            }
            ConfigString = (String.Join("", File.ReadLines(ConfigFilePath))
                .Replace("\\\"", "/'"));
            return ReadConfigFromString(ConfigString);
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

        public void WriteConfigToFile(KeyCombinationsConfiguration configuration, string configFileName = "")
        {
            if (!string.IsNullOrEmpty(configFileName))
            {
                if (!configFileName.Contains(ConfigurationManager.AppSettings["DefaultConfigFileExtension"]))
                {
                    configFileName += $".{ConfigurationManager.AppSettings["DefaultConfigFileExtension"]}";
                }
                ConfigFilePath = configFileName; 
            }

            var result = JsonConvert.SerializeObject(configuration, new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                Converters = new List<JsonConverter>(){new CombinationsConfigurationConverter()},
                PreserveReferencesHandling = PreserveReferencesHandling.All

            });
            if (!string.IsNullOrEmpty(ConfigFilePath))
            {
                using (var file = File.CreateText(ConfigFilePath))
                {
                    file.Write(result.ToCharArray());
                }
            }
        }

        public void WriteConfigToDbSet(KeyCombinationsConfiguration configuration, DbSet config)
        {
            throw new NotImplementedException();
        }
    }
}