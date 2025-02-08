namespace hh_napi.Models
{
    public class PaginationParams
    {
        public int Limit { get; set; } = 10;
        public int Offset { get; set; } = 0;
        public string? Search { get; set; }
    }
}