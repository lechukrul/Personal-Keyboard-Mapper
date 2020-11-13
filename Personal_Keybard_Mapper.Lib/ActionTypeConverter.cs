using System;
using System.Reflection;
using log4net;
using Newtonsoft.Json;
using Personal_Keyboard_Mapper.Lib.Enums;

namespace Personal_Keyboard_Mapper.Lib
{
    public class ActionTypeConverter : JsonConverter
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is ActionType action)
            {
                writer.WriteValue(action.ToString());
            }
            else
            {
                log.Error($"Unable to serialize an {nameof(ActionType)}");
            }
        }

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.String)
            {
                log.Error($"Bad action type format");
            }

            if (Enum.TryParse<ActionType>((string)existingValue, true, out var result))
            {
                return result;
            }

            return string.Empty;
        }

        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }
}