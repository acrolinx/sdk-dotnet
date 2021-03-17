/*
 * Copyright 2019-present Acrolinx GmbH
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

using Acrolinx.Net.Check;
using Acrolinx.Net.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace Acrolinx.Net.Platform
{
    public class CheckingCapabilities
    {
        public List<GuidanceProfile> GuidanceProfiles { get; set; }
        public List<ContentFormat> ContentFormats { get; set; }
        public List<ContentEncoding> ContentEncodings { get; set; }
        public string ReferencePattern { get; set; }
        public List<CheckType> CheckTypes { get; set; }
        [JsonProperty(ItemConverterType = typeof(TolerantEnumConverter))]
        public List<ReportType?> ReportTypes { get; set; }
    }
}
