using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using log4net;
using Personal_Keyboard_Mapper.Lib.Model;

namespace Personal_Keyboard_Mapper.Lib.Interfaces
{
    public interface IConfigSource
    {
        public ILog logger { get; set; }

        public string ConfigString { get; set; }
        public string ConfigFilePath { get; set; }

        /// <summary>
        /// Reads the configuration from string.
        /// </summary>
        /// <param name="configString">The source.</param>
        /// <returns></returns>
        KeyCombinationsConfiguration ReadConfigFromString(string configString = "");

        /// <summary>
        /// Reads the configuration from database set.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns></returns>
        KeyCombinationsConfiguration ReadConfigFromDbSet(
            DbSet config);

        /// <summary>
        /// Reads the configuration from file.
        /// </summary>
        /// <param name="path">The path.</param>
        KeyCombinationsConfiguration ReadConfigFromFile(string path = "");

        /// <summary>
        /// Writes the configuration to string.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="configString">The source.</param> 
        void WriteConfigToFile(KeyCombinationsConfiguration configuration, string configString = "");

        /// <summary>
        /// Writes the configuration To database set.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="config">The configuration.</param> 
        void WriteConfigToDbSet(KeyCombinationsConfiguration configuration, DbSet config);
    }
}