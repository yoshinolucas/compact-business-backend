using backend_dotnet.Interfaces;
using backend_dotnet.Models;
using backend_dotnet.Models.Dtos;
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

        public async Task DeleteAccess(RemoveList removeList)
        {
            using(var conn = new SqlConnection(cs)) {
                await conn.ExecuteAsync("DELETE FROM accesses WHERE id in @ids",
                new { removeList.ids });
            }
        }

        public async Task FinishAccess(int id)
        {
            await using var conn = new SqlConnection(cs);
            await conn.ExecuteAsync("UPDATE accesses SET logout_at = (getdate()) WHERE id = @Id", new { Id = id });
        }

        public async Task<Object?> GetPages(Pager pager)
        {
            string sql = "";
            sql = @"SELECT accesses.*, users.* 
            FROM accesses INNER JOIN users ON accesses.userId = users.Id 
            ORDER BY accesses.id
            OFFSET @offset ROWS FETCH NEXT @maxItems ROWS ONLY";


            var numAllRecords = ("SELECT COUNT(0) [Count] From accesses;");

            int offset = (pager.currentPage - 1) * pager.maxItems;

            var sqlParams = new {
                offset = offset,
                maxItems = pager.maxItems
            };

            await using var conn = new SqlConnection(cs);
                
            int totalRecords = await conn.QuerySingleAsync<int>(numAllRecords);
            var accesses = await conn.QueryAsync<Access, ViewUser,Access>(sql:sql,
            map: (access,user) => {
                access.User = user;
                return access;
            },
            splitOn: "id",
            param:sqlParams);


            return new {
                totalRecords = totalRecords,
                totalPages = (int)Math.Ceiling(totalRecords / (double) pager.maxItems),
                search = pager.search,
                filters = pager?.filters,
                accesses = accesses.ToList(),
                currentPageLength = accesses.ToList().Count,
                currentPage = pager.currentPage,
                maxItems = pager.maxItems
            };
        }

        public async Task<int> RegisterAccess(int id)
        {
            await using var conn = new SqlConnection(cs);
            return conn.QuerySingle<int>("INSERT INTO accesses (userId) OUTPUT INSERTED.[id] VALUES (@UserId)", new { UserId = id});
        }

        
    }
}