using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebApi.Products;

public static class ProductRoutes
{
    extension(IEndpointRouteBuilder api)
    {
        public IEndpointRouteBuilder MapProductEndpoints()
        {
            var productApi = api.MapGroup("/products");

            productApi.MapGet("/", async (string? category) =>
            {
                var jsonPath = Path.Combine(AppContext.BaseDirectory, "Products", "hagleitner-products.json");
                var jsonContent = await File.ReadAllTextAsync(jsonPath);
                
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                
                var wrapper = JsonSerializer.Deserialize<ProductWrapper>(jsonContent, options);
                
                if (wrapper?.Products == null)
                {
                    return Results.Ok(Array.Empty<ProductDto>());
                }

                var products = wrapper.Products.AsEnumerable();
                
                if (!string.IsNullOrWhiteSpace(category))
                {
                    products = products.Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
                }

                var productDtos = products.Select(p => new ProductDto(
                    p.ProductID,
                    p.ArticleNumber,
                    p.ArticleName,
                    p.Description,
                    p.Category,
                    p.Tags
                )).ToArray();

                return Results.Ok(productDtos);
            });

            productApi.MapGet("/categories", async () =>
            {
                var jsonPath = Path.Combine(AppContext.BaseDirectory, "Products", "hagleitner-products.json");
                var jsonContent = await File.ReadAllTextAsync(jsonPath);
                
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                
                var wrapper = JsonSerializer.Deserialize<ProductWrapper>(jsonContent, options);
                
                if (wrapper?.Products == null)
                {
                    return Results.Ok(Array.Empty<string>());
                }

                var categories = wrapper.Products
                    .Select(p => p.Category)
                    .Distinct()
                    .OrderBy(c => c)
                    .ToArray();

                return Results.Ok(categories);
            });

            return api;
        }
    }
}

// DTO for API response
public record ProductDto(
    int ProductID,
    string ArticleNumber,
    string ArticleName,
    string Description,
    string Category,
    string[] Tags
);

// Internal records for JSON deserialization
internal record ProductWrapper(
    [property: JsonPropertyName("products")] ProductJson[] Products
);

internal record ProductJson(
    [property: JsonPropertyName("productID")] int ProductID,
    [property: JsonPropertyName("articleNumber")] string ArticleNumber,
    [property: JsonPropertyName("articleName")] string ArticleName,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("category")] string Category,
    [property: JsonPropertyName("tags")] string[] Tags
);
