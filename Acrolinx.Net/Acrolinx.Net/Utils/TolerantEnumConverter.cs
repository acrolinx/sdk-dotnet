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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Acrolinx.Net.Utils
{
    public class TolerantEnumConverter : JsonConverter
    {
        [ThreadStatic]
        private static Dictionary<Type, Dictionary<string, object>> _fromValueMap; // string representation to Enum value map

        [ThreadStatic]
        private static Dictionary<Type, Dictionary<object, string>> _toValueMap; // Enum value to string map

        public string UnknownValue { get; set; } = "Unknown";

        public override bool CanConvert(Type objectType)
        {
            Type type = IsNullableType(objectType) ? Nullable.GetUnderlyingType(objectType) : objectType;
            return type.IsEnum;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            bool isNullable = IsNullableType(objectType);
            Type enumType = isNullable ? Nullable.GetUnderlyingType(objectType) : objectType;

            InitMap(enumType);

            if (reader.TokenType == JsonToken.String)
            {
                string enumText = reader.Value.ToString();

                object val = FromValue(enumType, enumText);

                if (val != null)
                    return val;
            }
            else if (reader.TokenType == JsonToken.Integer)
            {
                int enumVal = Convert.ToInt32(reader.Value);
                int[] values = (int[])Enum.GetValues(enumType);
                if (values.Contains(enumVal))
                {
                    return Enum.Parse(enumType, enumVal.ToString());
                }
            }

            if (!isNullable)
            {
                string[] names = Enum.GetNames(enumType);

                string unknownName = names
                    .Where(n => string.Equals(n, UnknownValue, StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefault();

                if (unknownName == null)
                {
                    throw new JsonSerializationException($"Unable to parse '{reader.Value}' to enum {enumType}. Consider adding Unknown as fail-back value.");
                }

                return Enum.Parse(enumType, unknownName);
            }

            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Type enumType = value.GetType();

            InitMap(enumType);

            string val = ToValue(enumType, value);

            writer.WriteValue(val);
        }

        private bool IsNullableType(Type t)
        {
            return (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        private void InitMap(Type enumType)
        {
            if (_fromValueMap == null)
                _fromValueMap = new Dictionary<Type, Dictionary<string, object>>();

            if (_toValueMap == null)
                _toValueMap = new Dictionary<Type, Dictionary<object, string>>();

            if (_fromValueMap.ContainsKey(enumType))
                return; // already initialized

            Dictionary<string, object> fromMap = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
            Dictionary<object, string> toMap = new Dictionary<object, string>();

            FieldInfo[] fields = enumType.GetFields(BindingFlags.Static | BindingFlags.Public);

            foreach (FieldInfo field in fields)
            {
                string name = field.Name;
                object enumValue = Enum.Parse(enumType, name);

                // use EnumMember attribute if exists
                EnumMemberAttribute enumMemberAttrbiute = field.GetCustomAttribute<EnumMemberAttribute>();

                if (enumMemberAttrbiute != null)
                {
                    string enumMemberValue = enumMemberAttrbiute.Value;

                    fromMap[enumMemberValue] = enumValue;
                    toMap[enumValue] = enumMemberValue;
                }
                else
                {
                    toMap[enumValue] = name;
                }

                fromMap[name] = enumValue;
            }

            _fromValueMap[enumType] = fromMap;
            _toValueMap[enumType] = toMap;
        }

        private string ToValue(Type enumType, object obj)
        {
            Dictionary<object, string> map = _toValueMap[enumType];

            return map[obj];
        }

        private object FromValue(Type enumType, string value)
        {
            Dictionary<string, object> map = _fromValueMap[enumType];

            if (!map.ContainsKey(value))
                return null;

            return map[value];
        }
    }
}
