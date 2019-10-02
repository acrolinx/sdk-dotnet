namespace Acrolinx.Net.Check
{
    public class CustomField
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public CustomField(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
