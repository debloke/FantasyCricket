using FantasyCricket.Service;
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
            try
            {
                return Enum.Parse(typeof(CountryTeamName), Convert.ToString(reader.Value).Replace(" ", ""), true);
            }
            catch
            {
                return CountryTeamName.Unknown;
            }


        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            value.ToString();
        }
    }
}
