using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WindowsInput;
using WindowsInput.Native;
using log4net;
using Newtonsoft.Json;
using Personal_Keyboard_Mapper.Lib.Comparers;
using Personal_Keyboard_Mapper.Lib.Enums;
using Personal_Keyboard_Mapper.Lib.Interfaces;
using ConfigurationException = Personal_Keyboard_Mapper.Lib.Exceptions.ConfigurationException;

namespace Personal_Keyboard_Mapper.Lib.Model
{
    /// <summary>
    /// Base class for keyboard keys combination 
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class KeyCombinationsConfiguration
    {
        [JsonIgnore]
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
         
        public KeyCombinationsConfiguration()
        { 
        }

        public KeyCombinationsConfiguration(int combinationSize, IEnumerable<IKeyCombination> combinations)
        {
            CombinationSize = combinationSize;
            Combinations = combinations;
        }

        /// <summary>
        /// Gets the combination instance based on <see cref="CombinationSize"/>.
        /// </summary>
        /// <returns></returns>
        public IKeyCombination GetCombinationInstance(ILog logger)
        {
            IKeyCombination result;
            switch (CombinationSize)
            {
                case 2:
                    result = new TwoKeysCombination();
                    result.SetLogger(logger);
                    return result;
                case 3:
                    result = new ThreeKeysCombination();
                    result.SetLogger(logger);
                    return result;
                default:
                    throw new Exception($"No combination instance corresponded to {nameof(CombinationSize)}");
            }
        }

        [JsonProperty]
        [JsonRequired]
        public int CombinationSize { get; set; }


        [JsonProperty]
        [JsonRequired]
        public IEnumerable<IKeyCombination> Combinations { get; set; }


    }
}
