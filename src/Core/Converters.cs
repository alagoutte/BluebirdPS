using Newtonsoft.Json;

namespace BluebirdPS.Core
{
    internal class Converters
    {
        internal static T ConvertFromJson<T>(string input)
        {
            return JsonConvert.DeserializeObject<T>(input);
        }

        internal static string ConvertToJson(object input)
        {
            return JsonConvert.SerializeObject(input, Formatting.Indented);
        }
    }
}
