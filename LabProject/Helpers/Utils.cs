using System.Text.Json;

namespace LabProject.Helpers
{
    public class Utils
    {
        private static Utils? _instance;
        private static readonly object _lock = new();

        private Utils() { }

        public static Utils Instance
        {
            get
            {
                lock (_lock)
                {
                    _instance ??= new Utils();
                    return _instance;
                }
            }
        }

        public string ExportToJson<T>(List<T> data, List<string>? selectedColumns = null)
        {
            if (selectedColumns == null || selectedColumns.Count == 0)
            {
                return JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            }

            var projectedData = data.Select(item =>
            {
                var dict = new Dictionary<string, object?>();
                var props = typeof(T).GetProperties();

                foreach (var prop in props)
                {
                    if (selectedColumns.Contains(prop.Name))
                    {
                        dict[prop.Name] = prop.GetValue(item);
                    }
                }

                return dict;
            }).ToList();

            return JsonSerializer.Serialize(projectedData, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}
