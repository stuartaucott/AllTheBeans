namespace AllTheBeans.Application.Common
{
    public interface IDateTimeProvider
    {
        DateTimeOffset UtcNow { get; }
        DateOnly Today { get; }
    }
}
