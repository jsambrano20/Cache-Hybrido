using hybrid_cache.Context;
using hybrid_cache.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;

namespace hybrid_cache.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutosController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly HybridCache _hybridCache;
    private readonly string cacheKey = "produtos";

    public ProdutosController(AppDbContext context,
        HybridCache hybridCache)
    {
        _context = context;
        _hybridCache = hybridCache;
    }

    [HttpGet]
    public async Task<IEnumerable<Produto>> GetProdutos()
    {
        return await _hybridCache.GetOrCreateAsync(cacheKey, async cancellationToken =>
        {
            await Task.Delay(3000);
            List<Produto> produtos = await _context.Produtos.ToListAsync();
            return produtos;
        },
        new HybridCacheEntryOptions
        {
            //Tempo de Expiração no cache distribuido
            Expiration = TimeSpan.FromSeconds(60),
            //Tempo de Expiração no cache local
            LocalCacheExpiration = TimeSpan.FromSeconds(60),
        },
        //Tags para invalidar o cache em grupo
        new[] { "produtos-tag" }
        );
    }

    [HttpGet("{id}")]
    public async Task<Produto> GetProduto(int id, CancellationToken ct)
    {
        string cacheKey = $"produto-{id}";

        return await _hybridCache.GetOrCreateAsync<Produto>
        (cacheKey, async cancellationToken =>
        {
            await Task.Delay(3000);
            var produto = await _context.Produtos.FindAsync(id);
            return produto;
        },
        new HybridCacheEntryOptions
        {
            //Tempo de Expiração no cache distribuido
            Expiration = TimeSpan.FromSeconds(60),
            //Tempo de Expiração no cache local
            LocalCacheExpiration = TimeSpan.FromSeconds(60),
        },
        //Tags para invalidar o cache em grupo
        new[] { $"produto-tag-{id}" }
        );
    }

    [HttpPost]
    public async Task<ActionResult<Produto>> PostProdutos(Produto produto)
    {
        _context.Produtos.Add(produto);
        await _context.SaveChangesAsync();

        //Invalida dados do cache por serem dados passados
        await _hybridCache.RemoveAsync(cacheKey);

        return CreatedAtAction("GetProduto", new { id = produto.Id }, produto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutProdutos(int id, Produto produto)
    {
        if (id != produto.Id)
        {
            return BadRequest();
        }
        _context.Entry(produto).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ProdutoExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        //Invalida dados do cache por serem dados passados
        await _hybridCache.RemoveAsync(cacheKey);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProdutos(int id)
    {
        var produto = await _context.Produtos.FindAsync(id);
        if (produto == null)
        {
            return NotFound();
        }

        _context.Produtos.Remove(produto);
        await _context.SaveChangesAsync();

        //Invalida dados do cache por serem dados passados
        await _hybridCache.RemoveAsync(cacheKey);

        return NoContent();
    }

    private bool ProdutoExists(int id)
    {
        return _context.Produtos.Any(e => e.Id == id);
    }
}

