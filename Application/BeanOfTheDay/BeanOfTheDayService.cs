using AllTheBeans.Application.Beans.Dtos;
using AllTheBeans.Application.Common;
using AllTheBeans.Domain.Entities;

namespace AllTheBeans.Application.BeanOfTheDay;

/// <summary>
/// Store each day's pick in the DailyBeanSelections table.
/// If today already has a row, return it.
/// Otherwise pick a random bean, excluding yesterday's.
/// </summary>
public sealed class BeanOfTheDayService(
    IBeanRepository beans,
    IDailyBeanRepository daily,
    IDateTimeProvider clock,
    IUnitOfWork uow) : IBeanOfTheDayService
{
    public async Task<BeanDto?> GetOrSelectAsync(CancellationToken ct)
    {
        var today = clock.Today;

        // 1. Already selected today? Return it.
        var existing = await daily.GetForDateAsync(today, ct);
        if (existing is not null)
        {
            var bean = await beans.GetAsync(existing.BeanId, ct);
            return bean is null ? null : BeanDto.From(bean);
        }

        // 2. No beans in the catalogue? Nothing to select.
        var total = await beans.CountAsync(ct);
        if (total == 0) return null;

        // 3. Find yesterday's pick so we can exclude it.
        Guid? excludeId = null;
        if (total > 1)
        {
            var previous = await daily.GetMostRecentAsync(ct);
            excludeId = previous?.BeanId;
        }

        // 4. Pick a random bean (excluding yesterday's).
        var chosen = await beans.GetRandomExcludingAsync(excludeId, ct);
        if (chosen is null) return null;

        // 5. Persist the selection.
        await daily.AddAsync(new DailyBeanSelection(today, chosen.Id), ct);
        await uow.SaveChangesAsync(ct);

        return BeanDto.From(chosen);
    }
}