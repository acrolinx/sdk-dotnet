using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Acrolinx.Net.Check
{
    public class CheckOptions
    {
        public string GuidanceProfileId { get; set; }
        public string BatchId { get; set; }
        public List<ReportType> ReportTypes { get; set; }
        [JsonConverter(typeof(StringEnumConverter), converterParameters: typeof(CamelCaseNamingStrategy))]
        public CheckType CheckType { get; set; }
        public string ContentFormat { get; set; }
    }
}
