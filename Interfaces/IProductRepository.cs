
using backend_dotnet.Models;
using backend_dotnet.Models.Dtos;

namespace backend_dotnet.Interfaces
{
    public interface IProductRepository
    {
        Task<int> InsertProduct(Product product);
        Task<Product> GetProductById(int id);
        Task DeleteProducts(RemoveList removeList);
        Task<Object?> GetPages (Pager pager);
        Task<Object?> GetTradePages (Pager pager);
        Task UpdateProduct (UpdateProductDto updateProductDto);
        Task<int> InsertProductTrade(ProductTrade productTrade);
        Task DeleteProductsTrades(RemoveList removeList);

    }
}