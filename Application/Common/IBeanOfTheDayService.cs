using AllTheBeans.Application.Beans.Dtos;

namespace AllTheBeans.Application.BeanOfTheDay;

public interface IBeanOfTheDayService
{
    Task<BeanDto?> GetOrSelectAsync(CancellationToken ct);
}
