using System.Collections.Generic;

namespace Acrolinx.Net.Check
{
    public class CheckOptions
    {
        public string GuidanceProfileId { get; set; }
        public string BatchId { get; set; }
        public List<ReportType> ReportTypes { get; set; }
        public CheckType CheckType { get; set; }
        public string ContentFormat { get; set; }
    }
}
