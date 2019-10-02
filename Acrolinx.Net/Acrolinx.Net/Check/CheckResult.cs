using System;
using System.Collections.Generic;

namespace Acrolinx.Net.Check
{
    public class CheckResult
    {
        public string Id { get; set; }
        public Quality Quality { get; set; }
        public Dictionary<String, Report> Reports { get; set; }
        //private List<Issue> issues;
    }
}
