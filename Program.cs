using hybrid_cache.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;

var builder = WebApplication.CreateBuilder(args);

// Adiciona suporte para a documenta��o da API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Cache em mem�ria
builder.Services.AddMemoryCache();

// Cache distribu�do usando o Redis no Docker
builder.Services.AddStackExchangeRedisCache(opt =>
{
    opt.Configuration = "localhost:6379";
});

// Configura��o do HybridCache
builder.Services.AddHybridCache(opt =>
{
    opt.MaximumPayloadBytes = 1024 * 1024;
    opt.MaximumKeyLength = 1024;
    opt.DefaultEntryOptions = new HybridCacheEntryOptions
    {
        Expiration = TimeSpan.FromSeconds(20),
        LocalCacheExpiration = TimeSpan.FromSeconds(20)
    };
});
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddControllers();

var app = builder.Build();

// Configura��o do Swagger apenas em desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
