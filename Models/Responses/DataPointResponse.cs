using System.Text.Json.Serialization;

namespace hh_napi.Models.Responses;

public class DataPointResponse
{
    public int Id { get; set; }
    public int DataSourceId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Value { get; set; } = null!;

    // Optional properties
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DataSourceResponse? DataSource { get; set; }
}
