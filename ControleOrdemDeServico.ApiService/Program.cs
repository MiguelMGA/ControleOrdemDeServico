using OsService.Infrastructure.Databases;
using OsService.Infrastructure.Logging;
using OsService.Infrastructure.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(OsService.Services.V1.CreateCustomer.CreateCustomerCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(OsService.Services.V1.SearchCustomer.SearchCustomerHandler).Assembly);
});

builder.Services.AddSingleton<IDefaultSqlConnectionFactory>(_ =>
    new SqlConnectionFactory(builder.Configuration.GetConnectionString("DefaultConnection")!));

builder.Services.AddSingleton<IAdminSqlConnectionFactory>(_ =>
    new SqlConnectionFactory(builder.Configuration.GetConnectionString("CreateTable")!)
);

builder.Services.AddScoped<CustomerRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IServiceOrderRepository, ServiceOrderRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();

builder.Services.AddSingleton<DatabaseGenerator>();

builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
builder.Services.AddControllers();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var repository = scope.ServiceProvider.GetRequiredService<DatabaseGenerator>();
    await repository.EnsureCreatedAsync(CancellationToken.None);
}

app.UseExceptionHandler(appBuilder =>
{
    appBuilder.Run(async context =>
    {
        context.Response.ContentType = "application/json";

        var exceptionHandlerPathFeature =
            context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();

        if (exceptionHandlerPathFeature?.Error is OsService.Domain.Exceptions.DomainException domainEx)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new { error = domainEx.Message });
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new { error = "Ocorreu um erro inesperado." });
        }
    });
});

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

string[] summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapDefaultEndpoints();

await app.RunAsync();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}