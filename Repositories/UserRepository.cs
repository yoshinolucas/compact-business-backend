using backend_dotnet.Interfaces;
using backend_dotnet.Config;
using Microsoft.Data.SqlClient;
using Dapper;
using backend_dotnet.Models;

namespace backend_dotnet.Repositories
{
    public class UserRepository : IUserRepository
    {

        public async Task<List<User>> All()
        {
            await using var conn = ConfigGlobal.GetConnection(); 
            var users = await conn.QueryAsync<User>("SELECT * FROM users");
            return users.ToList();        
        }

        public async Task<User> Get(int id)
        {
            await using(var conn = ConfigGlobal.GetConnection()) {
                return await conn.QueryFirstOrDefaultAsync<User>("SELECT * FROM users WHERE id = @id",new { id = id }); 
            }
        }

        public async Task Remove(RemoveUser removeUser)
        {
            await using(var conn = ConfigGlobal.GetConnection()) {
                await conn.ExecuteAsync("DELETE FROM users WHERE id IN @ids", new { ids = removeUser.ids });
            }
        }

        public async Task Update(UpdateUser updateUser)
        {
            await using(var conn = ConfigGlobal.GetConnection()) {
                var Allparams = new { 
                    Id = updateUser.ids,
                    Username = updateUser?.user?.Username,
                    Password = updateUser?.user?.Password,
                    Nome = updateUser?.user?.Nome,
                    Sobrenome = updateUser?.user?.Sobrenome,
                    Email = updateUser?.user?.Email,
                    Status = updateUser?.user?.Status,
                    Privilegio = updateUser?.user?.Privilegio,
                    Equipe = updateUser?.user?.Equipe,
                    Arquivado = updateUser?.user?.Arquivado
                };
                if(updateUser?.columns == null) {
                    await conn.ExecuteAsync(@"UPDATE users SET
                    username = @Username,
                    password = @Password,
                    nome = @Nome,
                    sobrenome = @Sobrenome,
                    email = @Email,
                    status = @Status,
                    privilegio = @Privilegio,
                    equipe = @Equipe
                    WHERE id IN @Id",
                    Allparams
                    );
                } else {
                    string columns = "";
                    int last = (updateUser.columns.Count()) - 1;
                    foreach(string column in updateUser.columns){
                        if(column == updateUser.columns[last]) {
                            columns += $"{column} = @{column} ";
                        } else {
                            columns += $"{column} = @{column},";
                        }
                    }
                    await conn.ExecuteAsync(@$"
                    UPDATE users SET {columns}
                    WHERE id in @Id",
                    Allparams);
                }
            }
        }

        public async Task Create(User user)
        {
            await using(var conn = ConfigGlobal.GetConnection()) await conn.ExecuteAsync(@"INSERT INTO users 
            (username,password,email,nome,sobrenome,status,privilegio,equipe,arquivado) 
            VALUES (
            @Username,
            @Password, 
            @Email, 
            @Nome,
            @Sobrenome,
            @Status,
            @Privilegio,
            @Equipe,
            @Arquivado
            )", user);
        }

        public static async Task<User> GetByUsernameAndPassword(String? username, String? password){
            await using(var conn = ConfigGlobal.GetConnection()) {
                return await conn.QueryFirstOrDefaultAsync<User>(@"
                SELECT id, username FROM users WHERE username=@username AND password=@password",new { username = username, password=password }); 
            }
        }

        //Pagination
        public async Task<Object?> GetPage(Pager pager)
        {
            //Verifica ordenação
            var hasOrd = pager.hasOrd == null ? "id" : "username " + pager.hasOrd;

            //Verifica se existe pesquisa
            var searchQuery = "";
            var filterQuery = "";
            if(pager.search != ""){
                searchQuery = " AND (username LIKE @Username OR nome LIKE @Nome) ";
            }

            
            if(pager?.filters?.status.Length > 0) filterQuery += " AND status IN @FilterStatus ";    

            if(!pager.filters.arquivado) filterQuery += " AND arquivado = 0 ";    

            var indefinido = "";
            var prefix = "";
            var prefix2 = "";
            var prefix3 = "";

            prefix2 = pager.filters.arquivado 
                && pager?.filters?.equipe?.Length > 0 
                && pager?.filters?.status.Length == 0 ?
                "OR" : "AND";

            prefix3 = pager?.filters?.status.Length == 0 
            ? "AND" : "OR";

            bool hasUndefined = Array.IndexOf(pager?.filters?.equipe, "Indefinido") >= 0 ? true : false;

            if(pager.filters.arquivado) {
                if(pager?.filters?.equipe?.Length > 0) {
                    filterQuery += " AND arquivado = 1 AND equipe IN @FilterEquipe ";
                } else {
                    filterQuery += $" {prefix3} arquivado = 1 ";
                }  

                if(hasUndefined) {
                    if( pager?.filters?.equipe?.Length > 0 ) {
                        filterQuery += " AND equipe is null OR arquivado = 1 AND 5=5 ";
                    }
                }
            }

            if(pager?.filters?.equipe?.Length > 0) {
                prefix = hasUndefined ?
                " AND equipe is null OR " : 
                pager.filters.arquivado 
                && pager?.filters?.status?.Length > 0 ?
                "OR" :
                "AND";
                filterQuery += $" {prefix} equipe IN @FilterEquipe {prefix2} status IN @FilterStatus ";
            }

            
            
            
            if( pager?.filters?.status.Length == 0
            && !pager.filters.arquivado) {
                filterQuery = " AND status IN ('') ";
            }

            
            //Pega o total de registros
            var numAllRecords = ("SELECT COUNT(0) [Count] From users;");
            var numAllRecordsWithSearch = ("SELECT COUNT(0) [Count] From users WHERE 1=1" + searchQuery + filterQuery);

            //Pega o primeiro item da página atual
            var offset = (pager?.currentPage - 1) * pager?.maxItems;

            var query = @"SELECT * FROM users
                WHERE 1=1" + searchQuery + filterQuery + @"
                AND 3=3 ORDER BY " + hasOrd + @" 
                OFFSET @offset ROWS FETCH NEXT @maxItems ROWS ONLY";

            var allParams = new {
                offset = offset,
                maxItems = pager?.maxItems,
                Username = "%" + pager?.search + "%",
                Nome = "%" + pager?.search + "%",
                FilterStatus = pager?.filters?.status,
                FilterEquipe = pager?.filters?.equipe
            };

            await using(var conn = ConfigGlobal.GetConnection()) {
                
                var totalRecords = await conn.QuerySingleAsync<int>(numAllRecords);
                var totalRecordsWithSearch = await conn.QuerySingleAsync<int>(numAllRecordsWithSearch, allParams);
                var results = await conn.QueryAsync<User>(query,
                allParams);

                var equipesOptions = await conn.QueryAsync<User>(
                    "SELECT DISTINCT equipe FROM users"
                );
                return new {
                    totalRecords = totalRecords,
                    totalRecordsWithSearch = totalRecordsWithSearch,
                    totalPages = (int)Math.Ceiling(totalRecords / (double) pager?.maxItems),
                    totalPagesWithSearch = (int)Math.Ceiling(totalRecordsWithSearch / (double) pager.maxItems),
                    search = pager.search,
                    filters = pager.filters,
                    equipeOptions = equipesOptions.ToList(),
                    users = results.ToList(),
                    
                };
            }        
        }
    }
}