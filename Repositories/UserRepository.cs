using backend_dotnet.Interfaces;
using backend_dotnet.Config;
using Dapper;
using backend_dotnet.Models;
using backend_dotnet.Models.Dtos;

namespace backend_dotnet.Repositories
{
    public class UserRepository : IUserRepository
    {
        public async Task<ViewUser> GetUserById(int id)
        {
            await using(var conn = ConfigGlobal.GetConnection()) {
                return await conn.QuerySingleOrDefaultAsync<ViewUser>("SELECT * FROM users WHERE id = @id",new { id = id }); 
            }
        }

        public async Task DeleteUser(RemoveUser removeUser)
        {
            await using(var conn = ConfigGlobal.GetConnection()) {
                var users = await conn.QueryAsync<ViewUser>(@"SELECT
                enderecoId,documentoId FROM users WHERE id IN @ids", new { ids = removeUser.ids});
                List<int?> enderecoIds = new List<int?>();
                List<int?> documentoIds = new List<int?>();
                foreach(var user in users){
                    enderecoIds.Add(user.EnderecoId);
                    documentoIds.Add(user.DocumentoId);
                }
                
                await conn.ExecuteAsync(@"DELETE FROM enderecos 
                WHERE id IN @ids", new { ids = enderecoIds.ToArray() });
                await conn.ExecuteAsync(@"DELETE FROM documentos 
                WHERE id IN @ids", new { ids =  documentoIds.ToArray() });
                await conn.ExecuteAsync(@"DELETE FROM users 
                WHERE id IN @ids", new { ids = removeUser.ids });
            }
        }

        public async Task UpdateUser(UpdateUser updateUser)
        {
            await using(var conn = ConfigGlobal.GetConnection()) {
                var Allparams = new { 
                    Id = updateUser.ids,
                    Username = updateUser?.user?.Username,
                    Nome = updateUser?.user?.Nome,
                    Sobrenome = updateUser?.user?.Sobrenome,
                    Email = updateUser?.user?.Email,
                    Status = updateUser?.user?.Status,
                    Privilegio = updateUser?.user?.Privilegio,
                    Equipe = updateUser?.user?.Equipe,
                    Arquivado = updateUser?.user?.Arquivado,
                    EnderecoId = updateUser?.user?.EnderecoId,
                    DocumentoId = updateUser?.user?.DocumentoId,
                    Genero = updateUser?.user?.Genero,
                    Data_nasc = updateUser?.user?.Data_nasc,
                    Salario = updateUser?.user?.Salario
                };
                if(updateUser?.columns?.Length == 0) {
                    await conn.ExecuteAsync(@"UPDATE users SET
                    username = @Username,
                    nome = @Nome,
                    sobrenome = @Sobrenome,
                    email = @Email,
                    status = @Status,
                    privilegio = @Privilegio,
                    equipe = @Equipe,
                    updated_at = (SELECT getdate()),
                    documentoId = @DocumentoId,
                    enderecoId = @EnderecoId,
                    data_nasc = @Data_nasc,
                    genero = @Genero,
                    salario = @Salario
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
            await using var conn = ConfigGlobal.GetConnection(); 
            var newUserId = conn.QuerySingle<int>(@"INSERT INTO users 
            (username,password,email,nome,sobrenome,status,privilegio,equipe,arquivado,enderecoId,documentoId) 
            OUTPUT INSERTED.[id]
            VALUES (
            @Username,
            @Password, 
            @Email, 
            @Nome,
            @Sobrenome,
            @Status,
            @Privilegio,
            @Equipe,
            @Arquivado,
            @EnderecoId,
            @DocumentoId
            )", user);
            return newUserId;
        }

        public static async Task<User> GetByUsernameAndPassword(String? username, String? password){
            await using(var conn = ConfigGlobal.GetConnection()) {
                return await conn.QueryFirstOrDefaultAsync<User>(@"
                SELECT id, username,privilegio FROM users WHERE username=@username AND password=@password",new { username = username, password=password }); 
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
                searchQuery = " AND (username LIKE @Search OR nome LIKE @Search OR email LIKE @Search) ";
            }
            
            //Processo de filtros de colunas
            if(pager?.filters?.status?.Length > 0) filterQuery += " AND status IN @FilterStatus ";    

            if(pager!.filters.arquivado) filterQuery += " AND arquivado = 0 ";    

            var prefix = "";
            var prefix2 = "";
            var prefix3 = "";
            var prefix4 = "AND";
            if(pager.filters.status?.Length > 0 
                && pager.filters.arquivado 
                    && pager.filters.equipe?.Length > 0  ) prefix4 = " AND equipe IN @FilterEquipe OR ";
            

            prefix2 = pager.filters!.arquivado 
                && pager.filters.equipe!.Length > 0 
                    && pager.filters.status!.Length == 0 ?
                "OR" : "AND";

            prefix3 = pager.filters.status!.Length == 0 
            ? "AND" : "OR";

            if(pager.filters.arquivado) {
                filterQuery += pager.filters.equipe!.Length > 0 ? 
                $" {prefix4} arquivado = 1 AND equipe IN @FilterEquipe " 
                : $" {prefix3} arquivado = 1 ";     
            }

            if(pager.filters.equipe!.Length > 0) {
                prefix = pager.filters.arquivado 
                && pager.filters.status.Length > 0 ?
                "OR" : "AND";
                filterQuery += $" {prefix} equipe IN @FilterEquipe {prefix2} status IN @FilterStatus ";
            }

            //Se não houver filtros de status aplicado
            if( pager.filters.status.Length == 0
                && !pager.filters.arquivado) filterQuery = " AND status IN ('') ";
            
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
                FilterEquipe = pager.filters.equipe,
                FilterDate = pager.filters.date,
                Min = pager.filters.date_range![0],
                Max = pager.filters.date_range[1]
            };

            await using(var conn = ConfigGlobal.GetConnection()) {
                
                var totalRecords = await conn.QuerySingleAsync<int>(numAllRecords);
                var totalRecordsWithFilters = await conn.QuerySingleAsync<int>(numAllRecordsWithFilters, allParams);
                var results = await conn.QueryAsync<ViewUser>(query,
                allParams);

                var equipesOptions = await conn.QueryAsync<EquipeOptions>("SELECT DISTINCT equipe FROM users");

                return new {

                    totalRecords = totalRecords,
                    totalRecordsWithFilters = totalRecordsWithFilters,
                    totalPages = (int)Math.Ceiling(totalRecords / (double) pager.maxItems),
                    totalPagesWithFilters = (int)Math.Ceiling(totalRecordsWithFilters / (double) pager.maxItems),
                    search = pager.search,
                    filters = pager.filters,
                    equipeOptions = equipesOptions.ToList(),
                    users = results.ToList(),
                    currentPageLength = results.ToList().Count,
                    
                };
            }        
        }
    }
}