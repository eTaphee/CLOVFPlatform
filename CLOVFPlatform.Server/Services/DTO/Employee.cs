using System;
using Newtonsoft.Json;

namespace CLOVFPlatform.Server.Services.DTO
{
    /// <summary>
    /// 직원 정보
    /// </summary>
    public class Employee
    {
        /// <summary>
        /// 키
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// 이름
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 이메일
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// 연락처
        /// </summary>
        [JsonConverter(typeof(EmployeeTelJsonConverter))]
        public string? Tel { get; set; }

        /// <summary>
        /// 입사일
        /// </summary>
        [JsonConverter(typeof(EmployeeJoinedJsonConverter))]
        public DateTime? Joined { get; set; }

        public Employee()
        {
        }
    }

    public class EmployeeTelJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.Value is string value)
            {
                return value.Replace("-", string.Empty);
            }

            return null;
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }
    }

    public class EmployeeJoinedJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string) || objectType == typeof(DateTime);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.Value is string value)
            {
                return DateTime.Parse(value);
            }

            return DateTime.MinValue;
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value is DateTime joined)
            {
                writer.WriteValue(joined.ToString("yyyy-MM-dd"));
            }
        }
    }
}

