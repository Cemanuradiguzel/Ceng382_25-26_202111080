using Newtonsoft.Json; 

namespace MyRazorApp.Utilities
{
    public class Utils
    {
        private static Utils _instance = new Utils();
        public static Utils Instance => _instance ?? (_instance = new Utils());

        private Utils() { }

        public string ExportToJson<T>(List<T> data)
        {
            return JsonConvert.SerializeObject(data);
        }
    }
}
