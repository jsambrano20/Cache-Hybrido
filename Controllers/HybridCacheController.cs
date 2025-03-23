using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.Memory;

namespace hybrid_cache.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HybridCacheController : ControllerBase
{
    private readonly HybridCache _hybridCache;
    private readonly IMemoryCache _memoryCache;
    private readonly IDistributedCache _distributedCache;
    private readonly string cacheKey = "WeatherForecast";

    public HybridCacheController(
        HybridCache hybridCache,
        IMemoryCache memoryCache,
        IDistributedCache distributedCache
    )
    {
        _hybridCache = hybridCache;
        _memoryCache = memoryCache;
        _distributedCache = distributedCache;
    }

    [HttpPost("SetAsync_HybridCache")]
    public async Task<IActionResult> SetCache()
    {
        await _hybridCache.SetAsync(cacheKey,
            GetWeatherForecast(),
            new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromSeconds(20),
                LocalCacheExpiration = TimeSpan.FromSeconds(20)
            },
            new[] { "tag1" });

        return Ok($"Chave '{cacheKey}' armazenada no cache.");
    }

    [HttpGet("GetOrCreate_HybridCache")]
    public async Task<IActionResult> GetOrCreateCache()
    {
        var tags = new List<string> { "tag2" };

        var value = await _hybridCache.GetOrCreateAsync(
            cacheKey,
            async cancellationToken =>
            {
                await Task.Delay(3000);
                var result = GetWeatherForecast();
                return result;
            },
            new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromSeconds(20),
                LocalCacheExpiration = TimeSpan.FromSeconds(20),
                Flags = HybridCacheEntryFlags.None
            },
            tags);

        return Ok(new
        {
            Message = $"Valor recuperado ou criado",
            Data = value
        });
    }

    [HttpDelete("RemoveAsync_HybridCache")]
    public async Task<IActionResult> RemoveCache()
    {
        await _hybridCache.RemoveAsync(cacheKey);
        return Ok($"Chave '{cacheKey}' removida com sucesso!");
    }

    [HttpDelete("RemoveByTagAsync_HybridCache")]
    public async Task<IActionResult> RemoveCacheByTag(string tag)
    {
        await _hybridCache.RemoveByTagAsync(tag);
        return Ok($"Todas as chaves associadas á Tag '{tag}' foram removidas com sucesso!");
    }

    [HttpGet("Verify_Cache")]
    public IActionResult VerifyCache()
    {
        string memory_cache = null;
        string distributed_cache = null;

        if (_memoryCache.TryGetValue(cacheKey, out var memoryValue))
        {
            memory_cache = memoryValue?.ToString();
        }

        string distributedValue = _distributedCache.GetString(cacheKey);
        if (distributedValue is not null)
        {
            distributed_cache = distributedValue?.ToString();
        }

        return Ok(new
        {
            MemoryCache = memory_cache ?? "Não encontrado",
            DistributedCache = distributed_cache ?? "Não encotrado"
        });
    }

    #region PRIVATES
    private IEnumerable<WeatherForecast> GetWeatherForecast()
    {
        return Enumerable.Range(1, 5).Select(i => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(i)),
            Temperature = Random.Shared.Next(-20, 55),
            Humidity = Random.Shared.Next(5, 100)
        }).ToArray();
    }
    #endregion
}

