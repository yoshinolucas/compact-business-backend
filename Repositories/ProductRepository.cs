using backend_dotnet.Interfaces;
using backend_dotnet.Models;
using backend_dotnet.Models.Dtos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace backend_dotnet.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IConfiguration _cfg;
        private readonly string _cn;
        public ProductRepository(IConfiguration cfg)
        {
            _cfg = cfg;
            _cn = _cfg.GetConnectionString("Conn")!;
        }
        public async Task DeleteProducts(RemoveList removeList)
        {
            await using(var conn = new SqlConnection(_cn)) {
                await conn.ExecuteAsync("DELETE FROM products WHERE id IN @ids", new { ids = removeList.ids});
            }
        }

        public async Task<Product> GetProductById(int id)
        {
            await using(var conn = new SqlConnection(_cn)) {
                return await conn.QueryFirstOrDefaultAsync<Product>("SELECT * FROM products WHERE id = @Id", new {Id = id});
            }
        }

        public async Task<int> InsertProduct(Product product)
        {
            await using(var conn = new SqlConnection(_cn)) {
                return conn.QuerySingle<int>(@"INSERT INTO products
                (measure,sku,price,brand,description,category)
                OUTPUT INSERTED.[Id]
                VALUES (
                @Measure,@Sku,@Price,@Brand,@Description,@Category
                )",product);
            }
        }

        public async Task<Object?> GetPages(Pager pager){
            string sqlWhere = "";
            string sql = "";


            if(!String.IsNullOrEmpty(pager.search)) sqlWhere += " AND (description LIKE @Search OR sku LIKE @Search OR category LIKE @Search) ";
            sql = @$"SELECT * FROM products WHERE 1=1 {sqlWhere} ORDER BY id
            OFFSET @offset ROWS FETCH NEXT @maxItems ROWS ONLY";


            var numAllRecords = ("SELECT COUNT(0) [Count] From products;");

            int offset = (pager.currentPage - 1) * pager.maxItems;

            var sqlParams = new {
                offset = offset,
                maxItems = pager.maxItems,
                search = "%"+pager.search+"%"
            };

            await using var conn = new SqlConnection(_cn);
                
            int totalRecords = await conn.QuerySingleAsync<int>(numAllRecords);
            var products = await conn.QueryAsync<Product>(sql,sqlParams);


            return new {
                totalRecords = totalRecords,
                totalPages = (int)Math.Ceiling(totalRecords / (double) pager.maxItems),
                search = pager.search,
                filters = pager?.filters,
                list = products.ToList(),
                currentPage = pager.currentPage,
                currentPageLength = products.ToList().Count,
                maxItems = pager.maxItems
            };
        }

        public async Task UpdateProduct(UpdateProductDto updateProductDto)
        {
            var sqlParams = new{
                Ids = updateProductDto.Ids,
                Description = updateProductDto.Product.Description,
                Sku = updateProductDto.Product.Sku,
                Price = updateProductDto.Product.Price,
                Brand = updateProductDto.Product.Brand,
                Category = updateProductDto.Product.Category,
                Measure = updateProductDto.Product.Measure,
            };
            await using(var conn = new SqlConnection(_cn)) {
                await conn.ExecuteAsync(@"UPDATE products SET
                measure = @Measure,
                sku=@Sku,
                price=@Price,
                brand=@Brand,
                description=@Description,
                category=@Category
                WHERE Id IN @Ids",sqlParams);
            }
        }

        public async Task<int> InsertProductTrade(ProductTrade productTrade)
        {
            await using(var conn = new SqlConnection(_cn)) {
                return conn.QuerySingle<int>(@"INSERT INTO products_trades
                (productId,amount,price,trade_date,type)
                OUTPUT INSERTED.[Id]
                VALUES (
                @ProductId,@Amount,@Price,@Trade_date,@Type
                )",productTrade);
            }
        }

        public async Task<object?> GetTradePages(Pager pager)
        {
            string sql = "";

            var numAllRecords = ("SELECT COUNT(0) [Count] From products_trades;");

            int offset = (pager.currentPage - 1) * pager.maxItems;

            var sqlParams = new {
                offset = offset,
                maxItems = pager.maxItems,
                ProductId = pager.filters!.id
            };

            var sqlWhere = "";
            if(pager.filters!.id != null) {
                sqlWhere += " AND productId = @ProductId ";
            }

            sql = @"SELECT * FROM products_trades WHERE 1=1 "+ sqlWhere +@" ORDER BY id
            OFFSET @offset ROWS FETCH NEXT @maxItems ROWS ONLY";
            await using var conn = new SqlConnection(_cn);
                
            int totalRecords = await conn.QuerySingleAsync<int>(numAllRecords);
            var product_trades = await conn.QueryAsync<ProductTrade>(sql,sqlParams);
            var totalAmount = await conn.QuerySingleAsync<int?>(@"SELECT 
            (SELECT COALESCE(SUM(amount),0) FROM products_trades WHERE productId = @ProductId AND type=1)
            -
            (SELECT COALESCE(SUM(amount),0) FROM products_trades WHERE productId = @ProductId AND type=2)" ,
            sqlParams);
            var totalIn = await conn.QuerySingleAsync<int?>("SELECT COALESCE(SUM(amount),0) FROM products_trades WHERE type=1 "+sqlWhere,sqlParams);
            var totalOut = await conn.QuerySingleAsync<int?>("SELECT COALESCE(SUM(amount),0) FROM products_trades WHERE type=2 "+sqlWhere,sqlParams);
            var totalPrice = await conn.QuerySingleAsync<decimal?>(@"SELECT 
            (SELECT COALESCE(SUM(price*amount),0) FROM products_trades WHERE productId = @ProductId AND type=2)
            -
            (SELECT  COALESCE(SUM(price*amount),0) FROM products_trades WHERE productId = @ProductId AND type=1)" ,
            sqlParams);
            var totalPriceIn = await conn.QuerySingleAsync<decimal?>("SELECT  COALESCE(SUM(price*amount),0) FROM products_trades WHERE type=1 "+sqlWhere,sqlParams);
            var totalPriceOut = await conn.QuerySingleAsync<decimal?>("SELECT  COALESCE(SUM(price*amount),0) FROM products_trades WHERE type=2 "+sqlWhere,sqlParams);
            return new {
                totalRecords = totalRecords,
                totalPages = (int)Math.Ceiling(totalRecords / (double) pager.maxItems),
                search = pager.search,
                filters = pager?.filters,
                totalIn = totalIn,
                totalOut = totalOut,
                totalAmount = totalAmount,
                totalPrice = totalPrice,
                totalPriceIn = totalPriceIn,
                totalPriceOut = totalPriceOut,
                productsTrades = product_trades.ToList(),
                currentPageLength = product_trades.ToList().Count
            };
        }

        public async Task DeleteProductsTrades(RemoveList removeList)
        {
            await using(var conn = new SqlConnection(_cn)) {
                await conn.ExecuteAsync("DELETE FROM products_trades WHERE id IN @ids", new { ids = removeList.ids});
            }
        }
    }
}