namespace Identity_Server.Entities
{
    public class DynamicItem
    {
        public Dictionary<string, object> FieldValues { get; set; } = new();

        public void SetValue(string key, object value)
        {
            if (FieldValues.ContainsKey(key))
            {
                FieldValues.Remove(key);
            }

            FieldValues.Add(key, value);
        }

        public object GetValue<T>(string key)
        {
            if (FieldValues.TryGetValue(key, out var res))
            {
                return (T)res;
            }

            return null;
        }

    }
}
