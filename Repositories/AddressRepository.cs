using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend_dotnet.Config;
using backend_dotnet.Interfaces;
using backend_dotnet.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace backend_dotnet.Repositories
{
    public class AddressRepository : IAddressRepository
    {
        private readonly IConfiguration _cfg;
        private readonly string cs;
        public AddressRepository(IConfiguration cfg)
        {
            _cfg = cfg;
            cs = _cfg.GetConnectionString("Conn")!;
        }
        public async Task<int> InsertAddress(Address ddress)
        {
            await using var conn = new SqlConnection(cs); 
            return conn.QuerySingle<int>(@"INSERT INTO addresses 
            (street,zip_code,state,city,number,district,country) 
            OUTPUT INSERTED.[id]
            VALUES 
            (@Street,@Zip_code,@State,@City,@Number,@District,@Country)", ddress);          
        }

        public async Task<Address> GetAddressById(int id)
        {
            await using var conn = new SqlConnection(cs); 
            return conn.QuerySingle<Address>(@"SELECT * FROM addresses WHERE id = @Id", new { Id = id}); 
        }

        public async Task UpdateAddress(Address address, int id)
        {
            await using var conn = new SqlConnection(cs); 
            await conn.ExecuteAsync(@"UPDATE addresses SET
            street = @Street,
            zip_code = @Zip_code,
            state = @State,
            city = @City,
            number = @Number,
            district = @District,
            country = @Country
            WHERE id = @Id", 
            new {
                Id = id,
                Street = address.Street,
                Zip_code = address.Zip_code,
                State = address.State,
                City = address.City,
                Number = address.Number,
                District = address.District,
                Country = address.Country
            }); 
        }
    }
}