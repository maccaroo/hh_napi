using System.Text.Json.Serialization;
using hh_napi.Domain;

namespace hh_napi.Models.Responses
{
    public class DataSourceResponse
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; } = string.Empty!;
        public DateTime CreatedAt { get; set; }
        public int CreatedByUserId { get; set; }
        public DataType Type { get; set; }

        // Optional properties
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public UserResponse? CreatedByUser { get; set; }
    }
} 