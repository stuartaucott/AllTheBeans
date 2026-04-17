using AllTheBeans.Application.BeanOfTheDay;
using AllTheBeans.Application.Common;
using AllTheBeans.Infrastructure.Persistence;
using AllTheBeans.Infrastructure.Persistence.Repositories;
using AllTheBeans.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
//register application services - could refactor into seperate file
builder.Services.AddDbContext<BeansDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("BeansDb")));

builder.Services.AddScoped<IBeanRepository, BeanRepository>();
builder.Services.AddScoped<IDailyBeanRepository, DailyBeanRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IBeanOfTheDayService, BeanOfTheDayService>();
builder.Services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BeansDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    await DbInitializer.SeedAsync(db, app.Environment, logger);
}

app.Run();
