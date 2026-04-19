using AllTheBeans.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AllTheBeans.Tests.Integration;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _connection.Open();
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            var dbDescriptors = services
                .Where(d => d.ServiceType.FullName?.Contains("DbContext") == true
                         || d.ServiceType.FullName?.Contains("EntityFramework") == true
                         || d.ServiceType.FullName?.Contains("SqlServer") == true)
                .ToList();
            foreach (var d in dbDescriptors) services.Remove(d);

            services.AddDbContext<BeansDbContext>(opt =>
                opt.UseSqlite(_connection)
                   .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning)));

            services.AddAuthentication("Test")
                .AddScheme<AuthenticationSchemeOptions, FakeAuthHandler>("Test", null);
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);
        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BeansDbContext>();
        db.Database.EnsureCreated();
        return host;
    }

    protected override void Dispose(bool disposing)
    {
        _connection.Close();
        base.Dispose(disposing);
    }
}