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

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "OsService API",
        Version = "v1",
        Description = "API para controle de ordens de serviço",
        Contact = new()
        {
            Name = "Miguel Aguiar",
            Email = "miguel.aguiar@ivoryit.com.br"
        }
    });

    var basePath = AppContext.BaseDirectory;

    var xmlFiles = Directory.GetFiles(basePath, "*.xml", SearchOption.TopDirectoryOnly);

    foreach (var xmlFile in xmlFiles)
    {
        options.IncludeXmlComments(xmlFile);
    }
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "OsService API v1");
    c.RoutePrefix = "swagger";
});

using (var scope = app.Services.CreateScope())
{
    var databaseGenerator = scope.ServiceProvider.GetRequiredService<DatabaseGenerator>();
    await databaseGenerator.EnsureCreatedAsync(CancellationToken.None);
}

app.UseExceptionHandler(appBuilder =>
{
    appBuilder.Run(async context =>
    {
        context.Response.ContentType = "application/json";

        var exceptionHandlerPathFeature =
            context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();

        var exception = exceptionHandlerPathFeature?.Error;

        switch (exception)
        {
            case OsService.Domain.Exceptions.NotFoundException notFoundEx:
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsJsonAsync(new { error = notFoundEx.Message });
                break;

            case OsService.Domain.Exceptions.DomainException domainEx:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new { error = domainEx.Message });
                break;

            default:
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(new { error = "Ocorreu um erro inesperado." });
                break;
        }
    });
});

app.UseHttpsRedirection();

app.MapControllers();

app.MapDefaultEndpoints();

await app.RunAsync();