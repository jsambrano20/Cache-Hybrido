# Hybrid Cache - .NET 9

## Visão Geral
Este projeto implementa um sistema de **cache híbrido** no **.NET 9**, utilizando **Redis** e **In-Memory Cache** para otimizar o desempenho e reduzir a carga no banco de dados.

## Tecnologias Utilizadas
- **.NET 9**
- **Redis** (Cache distribuído)
- **In-Memory Cache** (MemoryCache do .NET)
- **Entity Framework Core** (Opcional, para persistência de dados)

## Estratégia do Cache Híbrido
O cache híbrido combina dois tipos de cache:
1. **In-Memory Cache**: Utilizado para acessos rápidos dentro da instância da aplicação.
2. **Redis**: Cache distribuído para compartilhamento de dados entre múltiplas instâncias.

A lógica de acesso funciona assim:
- Primeiro, verifica se o dado está no **In-Memory Cache**.
- Se não estiver, verifica no **Redis**.
- Se também não estiver, busca do banco de dados e armazena em ambos os caches.

## Configuração do Redis
Antes de rodar a aplicação, configure o Redis:

### Docker:
```sh
 docker run -d --name redis-hybrid -p 6379:6379 -v redis-data:/data redis:latest
```
Ou use uma instância do **Azure Cache for Redis**.

## Implementação no .NET 9

### Configuração do Cache no `Program.cs`
```csharp
var builder = WebApplication.CreateBuilder(args);

// Adicionando MemoryCache
builder.Services.AddMemoryCache();

// Configurando Redis Cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:ConnectionString"];
});

var app = builder.Build();
```

## Como Executar
1. Configure a **string de conexão do Redis** no `appsettings.json`:
```json
"Redis": {
  "ConnectionString": "localhost:6379"
}
```
2. Execute o projeto:
```sh
 dotnet run
```

## Conclusão
Este projeto implementa uma solução eficiente de **cache híbrido** no .NET 9, aproveitando a rapidez do **In-Memory Cache** e a escalabilidade do **Redis**. Assim, reduzimos a latência e otimizamos o desempenho da aplicação.

