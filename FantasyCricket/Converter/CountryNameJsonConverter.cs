using Newtonsoft.Json;
using System;

namespace FantasyCricket.Converter
{
    public class CountryNameJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            if (objectType.IsEnum)
            {
                return true;
            }
            return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return Enum.Parse(objectType, Convert.ToString(existingValue).Replace(" ", ""), true);


        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            value.ToString();
        }
    }
}
