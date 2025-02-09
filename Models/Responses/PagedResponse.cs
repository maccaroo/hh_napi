using AutoMapper;

namespace hh_napi.Models.Responses;

public class PagedResponse<T>
{
    public int Offset { get; set; }
    public int Limit { get; set; }
    public int Total { get; set; }
    public IEnumerable<T> Data { get; set; } = Enumerable.Empty<T>();

    public PagedResponse(IEnumerable<T> data, int offset, int limit, int total)
    {
        Offset = offset;
        Limit = limit;
        Total = total;
        Data = data;
    }

    public PagedResponse<U> ConvertTo<U>(IMapper mapper)
    {
        return new PagedResponse<U>(mapper.Map<IEnumerable<U>>(Data), Offset, Limit, Total);
    }
}