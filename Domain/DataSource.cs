namespace hh_napi.Domain
{
    public class DataSource
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int CreatedByUserId { get; set; }
        public DataType Type { get; set; }

        public User CreatedByUser { get; set; } = null!;
        public List<DataPoint> DataPoints { get; set; } = new();
    }

    public enum DataType
    {
        String,
        Numeric
    }
}