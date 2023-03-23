using backend_dotnet.Interfaces;
using backend_dotnet.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace backend_dotnet.Repositories
{
    public class AccessRepository : IAccessRepository
    {
        private readonly IConfiguration _cfg;
        private readonly string cs;
        public AccessRepository(IConfiguration cfg)
        {
            _cfg = cfg;
            cs = _cfg.GetConnectionString("Conn")!;
        }

        public async Task FinishAccess(int id)
        {
            await using var conn = new SqlConnection(cs);
            await conn.ExecuteAsync("UPDATE accesses SET logout_at = (getdate()) WHERE id = @Id", new { Id = id });
        }

        public async Task<Object?> GetPages(Pager pager)
        {
            string sql = "";
            sql = @"SELECT * FROM accesses ORDER BY id
                OFFSET @offset ROWS FETCH NEXT @maxItems ROWS ONLY";


            var numAllRecords = ("SELECT COUNT(0) [Count] From accesses;");

            int offset = (pager.currentPage - 1) * pager.maxItems;

            var sqlParams = new {
                offset = offset,
                maxItems = pager.maxItems
            };

            await using var conn = new SqlConnection(cs);
                
            int totalRecords = await conn.QuerySingleAsync<int>(numAllRecords);
            IEnumerable<Access> accesses = await conn.QueryAsync<Access>(sql,sqlParams);


            return new {

                totalRecords = totalRecords,
                totalPages = (int)Math.Ceiling(totalRecords / (double) pager.maxItems),
                search = pager.search,
                filters = pager?.filters,
                accesses = accesses.ToList(),
                currentPageLength = accesses.ToList().Count,
                
            };
        }

        public async Task<int> RegisterAccess(int id)
        {
            await using var conn = new SqlConnection(cs);
            return conn.QuerySingle<int>("INSERT INTO accesses (userId) OUTPUT INSERTED.[id] VALUES (@UserId)", new { UserId = id});
        }
    }
}