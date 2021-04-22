using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Personal_Keyboard_Mapper.Lib.Comparers;
using Personal_Keyboard_Mapper.Lib.Enums;
using Personal_Keyboard_Mapper.Lib.Interfaces;
using Personal_Keyboard_Mapper.Lib.Model;
using ConfigurationException = Personal_Keyboard_Mapper.Lib.Exceptions.ConfigurationException;

namespace Personal_Keyboard_Mapper.Lib.Converters
{
    public class CombinationsConfigurationConverter : JsonConverter<KeyCombinationsConfiguration>
    {
        private ILog logger;
        public CombinationsConfigurationConverter()
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CombinationsConfigurationConverter"/> class.
        /// </summary>
        /// <param name="log">The logger.</param>
        public CombinationsConfigurationConverter(ILog log)
        {
            logger = log;
        }
        public override void WriteJson(JsonWriter writer, KeyCombinationsConfiguration value, JsonSerializer serializer)
        {
            if (!IsCombinationsDistinct(value.Combinations))
            {
                throw new ConfigurationException(ConfigurationManager.AppSettings["DuplicateConfigError"]);
            }

            writer.WriteStartObject();

            writer.WritePropertyName(nameof(value.CombinationSize));
            serializer.Serialize(writer, value.CombinationSize); 

            writer.WritePropertyName(nameof(value.Combinations));
            writer.WriteStartArray();

            WriteCombinationsNode(writer, serializer, value);

            writer.WriteEndArray();

            writer.WriteEndObject();
        }

        private void WriteCombinationsNode(JsonWriter writer, JsonSerializer serializer, KeyCombinationsConfiguration value)
        {
            foreach (var combination in value.Combinations)
            {
                writer.WriteStartObject();

                writer.WritePropertyName("FirstKey");
                serializer.Serialize(writer, int.Parse(((TwoKeysCombination)combination).FirstKeyVirtualCode));

                writer.WritePropertyName("SecondKey");
                serializer.Serialize(writer, int.Parse(((TwoKeysCombination)combination).SecondKeyVirtualCode));
                
                if (value.CombinationSize == 3)
                {
                    writer.WritePropertyName("ThirdKey");
                    serializer.Serialize(writer, int.Parse(((ThreeKeysCombination)combination).ThirdKeyVirtualCode));
                }

                WriteCombinationActionNode(writer, serializer, combination);

                writer.WriteEndObject();
            }
        }

        private static void WriteCombinationActionNode(JsonWriter writer, JsonSerializer serializer,
            IKeyCombination combination)
        {
            writer.WritePropertyName(nameof(combination.Action));

            writer.WriteStartObject();

            writer.WritePropertyName(nameof(combination.Action.Type));
            serializer.Serialize(writer, combination.Action.Type.ToString());

            writer.WritePropertyName("OutputVirtualKeys");
            writer.WriteStartArray();

            serializer.Converters.Add(new OutputKeysConverter());
            serializer.PreserveReferencesHandling = PreserveReferencesHandling.None;
            switch (combination.Action.Type)
            {
                case ActionType.Keyboard:
                    if (combination.Action.IsActionWithLeftArrowAdded() || combination.Action.IsDotOrSemiColonAction())
                    {
                        serializer.Serialize(writer, string.Join("", combination.Action.ActionStringKeys));
                    }
                    else
                    {
                        serializer.Serialize(writer, combination.Action.VirtualKeys);
                    }
                    break;
                case ActionType.Mouse:
                    serializer.Serialize(writer, combination.Action.ActionStringKeys[0]);
                    break;

                default:
                    throw new Exception("Could not serialize action, uknow action type");
            }

            writer.WriteEndArray();

            writer.WriteEndObject();
        }

        public override KeyCombinationsConfiguration ReadJson(JsonReader reader, Type objectType,
            KeyCombinationsConfiguration existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                var item = JObject.Load(reader);
                var conbinationSize = item["CombinationSize"]?.ToObject<int>();
                if (conbinationSize == null)
                {
                    throw new Exception($"Configuration error: lack of combination size");
                }
                if (item["Combinations"] != null)
                {
                    if (conbinationSize == 2)
                    {
                        var combinations = item["Combinations"].ToObject<IEnumerable<TwoKeysCombination>>(serializer);
                        foreach (var combination in combinations)
                        {
                            combination.SetLogger(logger);
                        }
                        if (!IsCombinationsDistinct(combinations))
                        {
                            throw new ConfigurationException(ConfigurationManager.AppSettings["DuplicateConfigError"]);
                        }
                        return new KeyCombinationsConfiguration(conbinationSize.Value, combinations);
                    }
                    if (conbinationSize == 3)
                    {
                        var combinations = item["Combinations"].ToObject<IEnumerable<ThreeKeysCombination>>(serializer);
                        foreach (var combination in combinations)
                        {
                            combination.SetLogger(logger);
                        }
                        if (!IsCombinationsDistinct(combinations))
                        {
                            throw new ConfigurationException(ConfigurationManager.AppSettings["DuplicateConfigError"]);
                        }
                        return new KeyCombinationsConfiguration(conbinationSize.Value, combinations);
                    }

                    return null;
                }
                else
                {
                    throw new Exception($"Configuration error: lack of combinations list");
                }
            }

            throw new Exception("Read json config error");
        }

        /// <summary>
        /// Determines whether [is combinations distinct].
        /// </summary>
        /// <param name="combinations">The combinations.</param>
        /// <returns>
        ///   <c>true</c> if [is combinations distinct]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsCombinationsDistinct(IEnumerable<IKeyCombination> combinations)
        {
            return (combinations.Count() == combinations.Distinct(new KeyCombinationsComparer()).Count());
        }

    }
}