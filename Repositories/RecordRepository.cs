using backend_dotnet.Interfaces;
using backend_dotnet.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace backend_dotnet.Repositories
{
    public class RecordRepository : IRecordRepository
    {
        private readonly IConfiguration _cfg;
        private readonly string cs;
        public RecordRepository(IConfiguration cfg)
        {
            _cfg = cfg;
            cs = _cfg.GetConnectionString("Conn")!;
        }

        public async Task DeleteRecords(RemoveList removeList)
        {
            using(var conn = new SqlConnection(cs)) {
                await conn.ExecuteAsync("DELETE FROM records WHERE id IN @ids",
                new { removeList.ids });
            }
        }

        public async Task<Object?> GetPages(Pager pager)
        {
            string sql = "";
            sql = @"SELECT * FROM records ORDER BY id
                OFFSET @offset ROWS FETCH NEXT @maxItems ROWS ONLY";


            var numAllRecords = ("SELECT COUNT(0) [Count] From records;");

            int offset = (pager.currentPage - 1) * pager.maxItems;

            var sqlParams = new {
                offset = offset,
                maxItems = pager.maxItems
            };

            await using var conn = new SqlConnection(cs);
                
            int totalRecords = await conn.QuerySingleAsync<int>(numAllRecords);
            IEnumerable<Record> records = await conn.QueryAsync<Record>(sql,sqlParams);
            return new {

                totalRecords = totalRecords,
                totalPages = (int)Math.Ceiling(totalRecords / (double) pager.maxItems),
                search = pager.search,
                filters = pager?.filters,
                records = records.ToList(),
                currentPage = pager.currentPage,
                maxItems = pager.maxItems,
                currentPageLength = records.ToList().Count,
                
            };
        }

        public async Task<Record> GetRecordById(int id)
        {
            await using var conn = new SqlConnection(cs);
            return conn.QuerySingleOrDefault<Record>("SELECT * FROM records WHERE id = @Id",new { Id = id});
        }

        public async Task RegisterRecord(Record record)
        {
            await using var conn = new SqlConnection(cs);
            await conn.ExecuteAsync("INSERT INTO records (section,userId,before,after,action) VALUES (@Section, @UserId, @Before, @After, @Action)",
            record);
        }
    }
}