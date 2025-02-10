using hh_napi.Attributes;

namespace hh_napi.Domain
{
    public class DataPoint
    {
        public int Id { get; set; }
        public int DataSourceId { get; set; }

        [DefaultOrderBy]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        [Searchable]
        public string Value { get; set; } = string.Empty; // Store all values as strings for now.

        public DataSource? DataSource { get; set; } = null!;
    }
}