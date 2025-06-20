using Microsoft.EntityFrameworkCore;
using ProductsApi.Data;
using WebApi.Infrastructure.Extensions;
using StackExchange.Redis;
using ProductsApi.Services;
using ProductsApi.Repository.Products;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();

    // Pega a ConnectionString da variável de ambiente ou do appsettings (prefere env)
    var connectionString = configuration.GetValue<string>("Redis:ConnectionString")
                          ?? throw new InvalidOperationException("Redis connection string is missing");

    // Agora carrega o restante das configurações da seção "Redis"
    var redisSection = configuration.GetSection("Redis");

    var configurationOptions = ConfigurationOptions.Parse(connectionString);

    // Agora, configura o resto usando os valores do appsettings.json
    configurationOptions.ClientName = redisSection.GetValue<string>("ClientName") ?? "ProductsApi";
    configurationOptions.AbortOnConnectFail = redisSection.GetValue<bool?>("AbortOnConnectFail") ?? false;
    configurationOptions.ConnectRetry = redisSection.GetValue<int?>("ConnectRetry") ?? 3;
    configurationOptions.ConnectTimeout = redisSection.GetValue<int?>("ConnectTimeout") ?? 5000;
    configurationOptions.SyncTimeout = redisSection.GetValue<int?>("SyncTimeout") ?? 5000;
    configurationOptions.AllowAdmin = redisSection.GetValue<bool?>("AllowAdmin") ?? false;
    configurationOptions.DefaultDatabase = redisSection.GetValue<int?>("DefaultDatabase") ?? 0;

    return ConnectionMultiplexer.Connect(configurationOptions);
});


builder.Services.AddScoped<ProductCacheService>();

builder.Services.AddScoped<IProductInterface, ProductRepository>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.ApplyMigrations();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
