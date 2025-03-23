using Microsoft.EntityFrameworkCore;

namespace hybrid_cache.Entities;
public class Produto
{
    public int? Id { get; set; }
    public string Nome { get; set; }

    [Precision(18, 2)]
    public decimal Preco { get; set; }
    public int Estoque { get; set; }

}
