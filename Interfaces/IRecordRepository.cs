using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend_dotnet.Models;

namespace backend_dotnet.Interfaces
{
    public interface IRecordRepository
    {
        
        Task RegisterRecord(Record rec);
        Task<Record> GetRecordById(int id);
        Task<Object?> GetPages(Pager pager);
        Task DeleteRecords(RemoveList removeList);
    }
}