using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace WebShop1.Models
{
    public static class SessionExtension
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            var json = session.GetString(key);

            if (string.IsNullOrEmpty(json))
            {
                return default!;
            }

            return JsonSerializer.Deserialize<T>(json);
        }
    }
}
