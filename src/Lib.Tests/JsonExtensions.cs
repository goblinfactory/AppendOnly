using Newtonsoft.Json;
using System.Text;

namespace Lib.Tests
{
    public static class JsonExtensions
    {
        // public static string ToJson(this object o, bool indented = true)
        // {
        //     var json = JsonConvert.SerializeObject(o);
        //     return json;
        // }

        public static string ToJsons<T>(this T[] items)
        {
            var sb = new StringBuilder();
            foreach (var item in items)
            {
                var json = JsonConvert.SerializeObject(item);
                sb.Append(json);
                sb.AppendLine($"---------------------------------------------------------");
            }
            var lines = sb.ToString();
            return lines;
        }

    }

}