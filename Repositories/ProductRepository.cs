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
        public async Task<bool> Delete(RemoveList removeList)
        {
            await using(var conn = new SqlConnection(_cn)) {
                await conn.ExecuteAsync("DELETE FROM products WHERE id in @Ids", new { Ids = removeList});
                return true;
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
                (measure,description_details,sku,price,brand,description,category,componentId,rawId,obs)
                OUTPUT INSERTED.[Id]
                VALUES (
                @Measure,@Description_details,@Sku,@Price,@Brand,@Description,@Category,@ComponentId,@RawId,@Obs
                )",product);
            }
        }

        public async Task<Object?> GetPages(Pager pager){
            string sql = "";
            sql = @"SELECT * FROM products ORDER BY id
            OFFSET @offset ROWS FETCH NEXT @maxItems ROWS ONLY";


            var numAllRecords = ("SELECT COUNT(0) [Count] From products;");

            int offset = (pager.currentPage - 1) * pager.maxItems;

            var sqlParams = new {
                offset = offset,
                maxItems = pager.maxItems
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
                currentPageLength = products.ToList().Count
            };
        }

        public Task Update(UpdateProductDto updateProductDto)
        {
            throw new NotImplementedException();
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
            var totalAmount = await conn.QuerySingleAsync<int>(@"SELECT 
            (SELECT SUM(amount) FROM products_trades WHERE productId = @ProductId AND type=1)
            -
            (SELECT SUM(amount) FROM products_trades WHERE productId = @ProductId AND type=2)" ,
            sqlParams);
            var totalIn = await conn.QuerySingleAsync<int>("SELECT SUM(amount) FROM products_trades WHERE type=1 "+sqlWhere,sqlParams);
            var totalOut = await conn.QuerySingleAsync<int>("SELECT SUM(amount) FROM products_trades WHERE type=2 "+sqlWhere,sqlParams);
            var totalPrice = await conn.QuerySingleAsync<decimal>(@"SELECT 
            (SELECT SUM(price*amount) FROM products_trades WHERE productId = @ProductId AND type=2)
            -
            (SELECT SUM(price*amount) FROM products_trades WHERE productId = @ProductId AND type=1)" ,
            sqlParams);
            var totalPriceIn = await conn.QuerySingleAsync<decimal>("SELECT SUM(price*amount) FROM products_trades WHERE type=1 "+sqlWhere,sqlParams);
            var totalPriceOut = await conn.QuerySingleAsync<decimal>("SELECT SUM(price*amount) FROM products_trades WHERE type=2 "+sqlWhere,sqlParams);
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
    }
}