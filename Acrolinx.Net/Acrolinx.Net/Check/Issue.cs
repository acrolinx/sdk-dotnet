/*
 * Copyright 2021-present Acrolinx GmbH
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

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

    public class PositionalInformation
    {
        public List<Match> Matches { get; set; }
    }

    public class Match
    {
        public string OriginalPart { get; set; }
        public long OriginalBegin { get; set; }
        public long OriginalEnd { get; set; }
    }

    public class Suggestion
    {
        public string Surface { get; set; }
        public List<string> Replacements { get; set; }

    }
}
