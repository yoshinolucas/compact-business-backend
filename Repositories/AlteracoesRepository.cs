using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend_dotnet.Config;
using backend_dotnet.Interfaces;
using backend_dotnet.Models;
using Dapper;

namespace backend_dotnet.Repositories
{
    public class AlteracoesRepository : IAlteracoesRepository
    {
        public async Task<Object?> GetPages(Pager pager)
        {
            string sql = "";
            sql = @"SELECT * FROM alteracoes ORDER BY id
                OFFSET @offset ROWS FETCH NEXT @maxItems ROWS ONLY";


            var numAllRecords = ("SELECT COUNT(0) [Count] From users;");

            int offset = (pager.currentPage - 1) * pager.maxItems;

            var sqlParams = new {
                offset = offset,
                maxItems = pager.maxItems
            };

            await using var conn = ConfigGlobal.GetConnection();
                
            int totalRecords = await conn.QuerySingleAsync<int>(numAllRecords);
            IEnumerable<Alteracoes> alteracoes = await conn.QueryAsync<Alteracoes>(sql,sqlParams);


            return new {

                totalRecords = totalRecords,
                totalPages = (int)Math.Ceiling(totalRecords / (double) pager.maxItems),
                search = pager.search,
                filters = pager?.filters,
                alteracoes = alteracoes.ToList(),
                currentPageLength = alteracoes.ToList().Count,
                
            };
        }

        public async Task<Alteracoes> Id(int id)
        {
            await using var conn = ConfigGlobal.GetConnection();
            return conn.QuerySingleOrDefault<Alteracoes>("SELECT * FROM alteracoes WHERE id = @Id",new { Id = id});
        }

        public async Task Register(Alteracoes alteracao)
        {
            await using var conn = ConfigGlobal.GetConnection();
            await conn.ExecuteAsync("INSERT INTO alteracoes (section,userId,antes,depois,action) VALUES (@Section, @UserId, @Antes, @Depois, @Action)",
            alteracao);
        }
    }
}