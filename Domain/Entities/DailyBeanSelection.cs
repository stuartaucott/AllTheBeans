namespace AllTheBeans.Domain.Entities;

public sealed class DailyBeanSelection
{
    public DateOnly Date { get; private set; }
    public Guid BeanId { get; private set; }

    private DailyBeanSelection() { } // EF Core

    public DailyBeanSelection(DateOnly date, Guid beanId)
    {
        if (beanId == Guid.Empty)
            throw new ArgumentException("BeanId required", nameof(beanId));
        Date = date;
        BeanId = beanId;
    }
}
