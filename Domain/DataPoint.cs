namespace hh_napi.Domain
{
    public class DataPoint
    {
        public int Id { get; set; }
        public int DataSourceId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Value { get; set; } = string.Empty; // Store all values as strings for now.

        public DataSource? DataSource { get; set; } = null!;
    }
}