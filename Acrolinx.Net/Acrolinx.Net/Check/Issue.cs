using System.Collections.Generic;

namespace Acrolinx.Net.Check
{
    public class Issue
    {
        public string GoalId { get; set; }
        public string DisplayNameHtml { get; set; }
        public string GuidanceHtml { get; set; }
        public string DisplaySurface { get; set; }
        public PositionalInformation PositionalInformation { get; set; }
        public List<Suggestion> Suggestions { get; set; }
    }

    public partial class PositionalInformation
    {
        public List<Match> Matches { get; set; }
    }

    public partial class Match
    {
        public string OriginalPart { get; set; }
        public long OriginalBegin { get; set; }
        public long OriginalEnd { get; set; }
    }

    public partial class Suggestion
    {
        public string Surface { get; set; }
        public List<string> Replacements { get; set; }

    }
}
