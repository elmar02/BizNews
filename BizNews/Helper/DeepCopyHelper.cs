using Newtonsoft.Json;

namespace BizNews.Helper
{
    public static class DeepCopyHelper
    {
        public static T DeepCopy<T>(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            string json = JsonConvert.SerializeObject(obj);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }

}
