using System.Net;
using System.Net.Http.Json;
using AllTheBeans.Application.Beans.Commands;
using AllTheBeans.Application.Beans.Dtos;
using FluentAssertions;
using Xunit;

namespace AllTheBeans.Tests.Integration;

public class BeansApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public BeansApiTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Search_returns_empty_list_when_no_beans()
    {
        var response = await _client.GetAsync("/api/beans");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var beans = await response.Content.ReadFromJsonAsync<List<BeanDto>>();
        beans.Should().NotBeNull();
    }

    [Fact]
    public async Task Create_and_get_bean_round_trip()
    {
        var cmd = new CreateBeanCommand("Test Bean", "Tasty", "Brazil", "Dark", 9.99m, "http://x/x.png");
        var createResponse = await _client.PostAsJsonAsync("/api/beans", cmd);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await createResponse.Content.ReadFromJsonAsync<BeanDto>();
        created!.Name.Should().Be("Test Bean");

        var getResponse = await _client.GetAsync($"/api/beans/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var fetched = await getResponse.Content.ReadFromJsonAsync<BeanDto>();
        fetched!.Id.Should().Be(created.Id);
    }

    [Fact]
    public async Task Get_nonexistent_bean_returns_404()
    {
        var response = await _client.GetAsync($"/api/beans/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Bean_of_the_day_returns_ok_after_seeding()
    {
        // Seed a bean first
        var cmd = new CreateBeanCommand("BOTD Bean", "Daily pick", "Peru", "Medium", 15m, "http://x/x.png");
        await _client.PostAsJsonAsync("/api/beans", cmd);

        var response = await _client.GetAsync("/api/bean-of-the-day");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var bean = await response.Content.ReadFromJsonAsync<BeanDto>();
        bean!.Name.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Create_bean_with_invalid_data_returns_400()
    {
        var cmd = new CreateBeanCommand("", "", "", "", -5m, "");
        var response = await _client.PostAsJsonAsync("/api/beans", cmd);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}