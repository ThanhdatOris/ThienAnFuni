using Newtonsoft.Json;

namespace ThienAnFuni.Extensions
{
    public static class SessionExtensions
    {
        // Lưu object vào session
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            var json = JsonConvert.SerializeObject(value);
            session.SetString(key, json);
        }

        // Lấy object từ session
        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var json = session.GetString(key);
            return json == null ? default : JsonConvert.DeserializeObject<T>(json);
        }

        // Set Decimal vào session
        public static void SetDecimal(this ISession session, string key, decimal value)
        {
            session.SetString(key, value.ToString("F2"));
        }

        // Lấy Decimal từ session
        public static decimal GetDecimal(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value != null ? decimal.Parse(value) : 0m;
        }

        // Set Int32 vào session
        public static void SetCustomInt32(this ISession session, string key, int value)
        {
            session.SetString(key, value.ToString());
        }


        // Lấy Int32 từ session
        public static int GetCustomInt32(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value != null ? int.Parse(value) : 0;
        }
    }

}
