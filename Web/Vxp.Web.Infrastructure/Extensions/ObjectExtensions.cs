namespace Vxp.Web.Infrastructure.Extensions
{
    using System;
    using Newtonsoft.Json;

    public static class ObjectExtensions
    {
        public static string ToJson(this object obj, bool formatted = false)
        {
            try
            {
                var serializerSettings = new JsonSerializerSettings
                {
                    //DateFormatString = "yyyy-MM-dd HH:mm:ss",
                    Formatting = formatted ? Formatting.Indented : Formatting.None
                };
                return JsonConvert.SerializeObject(obj, serializerSettings);
            }
            catch (Exception e)
            {
                return $"Can't serialize object of type {obj.GetType()}, ExceptionMessage: {e.Message} ";
            }
        }

        public static T ToObject<T>(this string jsonString)
        {
            try
            {
                var serializerSettings = new JsonSerializerSettings
                {
                    //DateFormatString = "yyyy-MM-dd HH:mm:ss"
                };

                return JsonConvert.DeserializeObject<T>(jsonString, serializerSettings);
            }
            catch
            {
                return default;
            }
        }
    }
}