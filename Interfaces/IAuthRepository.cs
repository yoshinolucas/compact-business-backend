using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend_dotnet.Models;

namespace backend_dotnet.Interfaces
{
    public interface IAuthRepository
    {
        Task<Object?> Login(LoginUser loginUser);
    }
}