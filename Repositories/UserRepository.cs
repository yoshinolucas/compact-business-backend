using backend_dotnet.Interfaces;
using backend_dotnet.Config;
using Dapper;
using backend_dotnet.Models;
using backend_dotnet.Models.Dtos;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using backend_dotnet.Services;

namespace backend_dotnet.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IConfiguration _cfg;
        private readonly string cs;
        private readonly Hash _hash;
        public UserRepository(IConfiguration cfg, Hash hash)
        {
            _cfg = cfg;
            cs = _cfg.GetConnectionString("Conn")!;
            _hash = hash;
        }
        public async Task<IEnumerable<ViewUser>> GetUserById(int id)
        {
            await using(var conn = new SqlConnection(cs)) {
                return await conn.QueryAsync<ViewUser,Address,Document,ViewUser>(sql:@"SELECT * FROM users 
                LEFT JOIN addresses ON users.id = addresses.userId
                LEFT JOIN documents ON users.id = documents.userId
                WHERE users.id = @id",
                map:(v,a,d)=>{
                    v.Address = a;
                    v.Document = d;
                    return v;
                }, param:new { id = id }); 
            }
        }

        public async Task DeleteUser(RemoveList removeList)
        {
            await using(var conn = new SqlConnection(cs)) {
                await conn.ExecuteAsync(@"DELETE FROM users WHERE id IN @ids", new { ids = removeList.ids });
            }
        }

        public async Task UpdateUser(UpdateUser updateUser)
        {
            await using(var conn = new SqlConnection(cs)) {
                var Allparams = new { 
                    Id = updateUser.ids,
                    Username = updateUser?.user?.Username,
                    Name = updateUser?.user?.Name,
                    Lastname = updateUser?.user?.Lastname,
                    Email = updateUser?.user?.Email,
                    Status = updateUser?.user?.Status,
                    Role = updateUser?.user?.Role,
                    Team = updateUser?.user?.Team,
                    Archived = updateUser?.user?.Archived,
                    Gender = updateUser?.user?.Gender != "" ? updateUser?.user?.Gender : null,
                    Birthday_date = updateUser?.user?.Birthday_date != "" ? updateUser?.user?.Birthday_date : null,
                    Salary = updateUser?.user?.Salary != "" ? updateUser?.user?.Salary : null
                };
                if(updateUser?.columns?.Length == 0) {
                    await conn.ExecuteAsync(@"UPDATE users SET
                    username = @Username,
                    Name = @Name,
                    lastname = @Lastname,
                    email = @Email,
                    status = @Status,
                    role = @Role,
                    team = @Team,
                    archived = @Archived,
                    updated_at = (SELECT getdate()),
                    birthday_date = @Birthday_date,
                    gender = @Gender,
                    salary = @Salary
                    WHERE id IN @Id",
                    Allparams
                    );
                } else {
                    string columns = "";
                    int last = (updateUser!.columns!.Count()) - 1;
                    foreach(string column in updateUser!.columns!){
                        if(column == updateUser.columns[last]) {
                            columns += $"{column} = @{column} ";
                        } else {
                            columns += $"{column} = @{column},";
                        }
                    }
                    await conn.ExecuteAsync(@$"
                    UPDATE users SET {columns}
                    ,updated_at = (SELECT getdate())
                    WHERE id in @Id",
                    Allparams);
                }
            }
        }

        public async Task<int> InsertUser(User user)
        {
            
            user.Password = _hash.HashPassword(user.Password);
            await using var conn = new SqlConnection(cs); 
            var newUserId = conn.QuerySingle<int>(@"INSERT INTO users 
            (username,password,email,name,lastname,status,role,team,archived) 
            OUTPUT INSERTED.[id]
            VALUES (
            @Username,
            @Password, 
            @Email, 
            @Name,
            @Lastname,
            @Status,
            @Role,
            @Team,
            @Archived
            )", user);
            return newUserId;
        }

        public async Task<User?> GetByUsernameAndPassword(String username, String password){
            await using(var conn = new SqlConnection(cs)) {
                var user = await conn.QueryFirstOrDefaultAsync<User>("SELECT id, username, password,role,status FROM users WHERE username=@username",new { username = username }); 
                if(_hash.ValidatePass(password, user.Password)) return user;
                return null;
            }
        }

        //Pagination,Search,Filters
        public async Task<Object?> GetPages(Pager pager)
        {
            //Verifica ordenação
            var orderQuery = "";
            if(pager.filters!.order != "") {
                if(pager.filters.order!.Contains("-")){
                    var valuesOrder = pager.filters.order.Split('-');
                    orderQuery = $"{valuesOrder[0]} {valuesOrder[1]}";
                } else {
                    orderQuery = $"{pager.filters.order}";
                }  
            } else {
                orderQuery = "id";
            }
            

            //Verifica se existe pesquisa
            var searchQuery = "";
            var filterQuery = "";
            if(pager.search != ""){
                searchQuery = " AND (username LIKE @Search OR name LIKE @Search OR email LIKE @Search) ";
            }
            
            //Processo de filtros de colunas
            if(pager?.filters?.status?.Length > 0) filterQuery += " AND status IN @FilterStatus ";    

            if(!pager!.filters.archived) filterQuery += " AND archived = 0 ";    

            var prefix = "";
            var prefix2 = "";
            var prefix3 = "";
            var prefix4 = "AND";
            if(pager.filters.status?.Length > 0 
                && pager.filters.archived 
                    && pager.filters.teams?.Length > 0  ) prefix4 = " AND team IN @FilterTeams OR ";
            

            prefix2 = pager.filters!.archived 
                && pager.filters.teams!.Length > 0 
                    && pager.filters.status!.Length == 0 ?
                "OR" : "AND";

            prefix3 = pager.filters.status!.Length == 0 
            ? "AND" : "OR";

            if(pager.filters.archived) {
                filterQuery += pager.filters.teams!.Length > 0 ? 
                $" {prefix4} archived = 1 AND team IN @FilterTeams " 
                : $" {prefix3} archived = 1 ";     
            }

            if(pager.filters.teams!.Length > 0) {
                prefix = pager.filters.archived 
                && pager.filters.status.Length > 0 ?
                "OR" : "AND";
                filterQuery += $" {prefix} team IN @FilterTeams {prefix2} status IN @FilterStatus ";
            }

            //Se não houver filtros de status aplicado
            if( pager.filters.status.Length == 0
                && !pager.filters.archived) filterQuery = " AND status IN ('') ";
            
            //Se houver filtro de datas
            if( pager.filters.date >= 0) filterQuery += " AND DATEDIFF(day,created_at,getdate()) < @FilterDate ";
            

            //Se houver data customizada
            if( pager.filters.date == -2) filterQuery += " AND created_at BETWEEN @Min AND @Max ";
             
            
            //Pega o total de registros
            var numAllRecords = ("SELECT COUNT(0) [Count] From users;");
            var numAllRecordsWithFilters = ("SELECT COUNT(0) [Count] From users WHERE 1=1" + searchQuery + filterQuery);

            //Pega o primeiro item da página atual
            var offset = (pager.currentPage - 1) * pager.maxItems;

            var query = @"SELECT * FROM users
                WHERE 1=1" + searchQuery + filterQuery + @"
                AND 3=3 ORDER BY " + orderQuery + @" 
                OFFSET @offset ROWS FETCH NEXT @maxItems ROWS ONLY";

            //Junta todos os parâmetros
            var allParams = new {
                offset = offset,
                maxItems = pager.maxItems,
                Search = "%" + pager.search + "%",
                FilterStatus = pager.filters.status,
                FilterTeams = pager.filters.teams,
                FilterDate = pager.filters.date,
                Min = pager.filters.date_range![0],
                Max = pager.filters.date_range[1]
            };

            await using(var conn = new SqlConnection(cs)) {
                
                var totalRecords = await conn.QuerySingleAsync<int>(numAllRecords);
                var totalRecordsWithFilters = await conn.QuerySingleAsync<int>(numAllRecordsWithFilters, allParams);
                var results = await conn.QueryAsync<ViewUser>(query,
                allParams);

                var teamOptions = await conn.QueryAsync<string>("SELECT DISTINCT team FROM users");

                return new {

                    totalRecords = totalRecords,
                    totalRecordsWithFilters = totalRecordsWithFilters,
                    totalPages = (int)Math.Ceiling(totalRecords / (double) pager.maxItems),
                    totalPagesWithFilters = (int)Math.Ceiling(totalRecordsWithFilters / (double) pager.maxItems),
                    search = pager.search,
                    filters = pager.filters,
                    teamOptions = teamOptions.ToList(),
                    users = results.ToList(),
                    currentPageLength = results.ToList().Count,
                    
                };
            }        
        }
    }
}