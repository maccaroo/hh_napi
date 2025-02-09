using hh_napi.Attributes;

namespace hh_napi.Domain
{
    public class DataSource
    {
        public int Id { get; set; }

        [Searchable]
        public string Name { get; set; } = null!;

        [Searchable]
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int CreatedByUserId { get; set; }
        public DataType Type { get; set; }

        public User? CreatedByUser { get; set; }
        public List<DataPoint> DataPoints { get; set; } = new();
    }

    public enum DataType
    {
        String,
        Numeric
    }
}