using AllTheBeans.Application.BeanOfTheDay;
using AllTheBeans.Application.Common;
using AllTheBeans.Domain.Entities;
using FluentAssertions;
using Moq;

namespace AllTheBeans.Tests.Unit;

public class BeanOfTheDayServiceTests
{
    private static Bean MakeBean(string name = "Kenya AA") =>
        new(Guid.NewGuid(), name, "desc", "Kenya", "Medium", 12.50m, "http://x/x.png");

    private sealed class FakeClock(DateOnly today) : IDateTimeProvider
    {
        public DateTimeOffset UtcNow => new(today.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
        public DateOnly Today => today;
    }

    [Fact]
    public async Task Returns_existing_selection_if_today_already_picked()
    {
        var bean = MakeBean();
        var today = new DateOnly(2026, 4, 19);

        var beans = new Mock<IBeanRepository>();
        beans.Setup(r => r.GetAsync(bean.Id, It.IsAny<CancellationToken>())).ReturnsAsync(bean);

        var daily = new Mock<IDailyBeanRepository>();
        daily.Setup(r => r.GetForDateAsync(today, It.IsAny<CancellationToken>()))
             .ReturnsAsync(new DailyBeanSelection(today, bean.Id));

        var sut = new BeanOfTheDayService(beans.Object, daily.Object,
            new FakeClock(today), Mock.Of<IUnitOfWork>());

        var result = await sut.GetOrSelectAsync(CancellationToken.None);

        result!.Id.Should().Be(bean.Id);
        beans.Verify(r => r.GetRandomExcludingAsync(It.IsAny<Guid?>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Excludes_previous_day_bean_when_selecting()
    {
        var yesterday = MakeBean("Yesterday");
        var today = new DateOnly(2026, 4, 19);
        var todayBean = MakeBean("Today");

        var beans = new Mock<IBeanRepository>();
        beans.Setup(r => r.CountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(10);
        beans.Setup(r => r.GetRandomExcludingAsync(yesterday.Id, It.IsAny<CancellationToken>()))
             .ReturnsAsync(todayBean);

        var daily = new Mock<IDailyBeanRepository>();
        daily.Setup(r => r.GetForDateAsync(today, It.IsAny<CancellationToken>()))
             .ReturnsAsync((DailyBeanSelection?)null);
        daily.Setup(r => r.GetMostRecentAsync(It.IsAny<CancellationToken>()))
             .ReturnsAsync(new DailyBeanSelection(today.AddDays(-1), yesterday.Id));

        var uow = new Mock<IUnitOfWork>();

        var sut = new BeanOfTheDayService(beans.Object, daily.Object, new FakeClock(today), uow.Object);
        var result = await sut.GetOrSelectAsync(CancellationToken.None);

        result!.Id.Should().Be(todayBean.Id);
        daily.Verify(r => r.AddAsync(
            It.Is<DailyBeanSelection>(d => d.Date == today && d.BeanId == todayBean.Id),
            It.IsAny<CancellationToken>()), Times.Once);
        uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Returns_null_when_no_beans_exist()
    {
        var today = new DateOnly(2026, 4, 19);

        var beans = new Mock<IBeanRepository>();
        beans.Setup(r => r.CountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);

        var daily = new Mock<IDailyBeanRepository>();
        daily.Setup(r => r.GetForDateAsync(today, It.IsAny<CancellationToken>()))
             .ReturnsAsync((DailyBeanSelection?)null);

        var sut = new BeanOfTheDayService(beans.Object, daily.Object,
            new FakeClock(today), Mock.Of<IUnitOfWork>());

        var result = await sut.GetOrSelectAsync(CancellationToken.None);

        result.Should().BeNull();
    }
}