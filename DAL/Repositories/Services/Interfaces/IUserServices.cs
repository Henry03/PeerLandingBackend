using DAL.DTO.Req;
using DAL.DTO.Res;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Services.Interfaces
{
    public interface IUserServices
    {
        Task<string> Register(ReqRegisterUserDto register);

        Task<List<ResUserDto>> GetAllUsers();
        Task<ResLoginDto> Login(ReqLoginDto login);
        Task<String> UpdateUser(ReqUpdateUserDto user, string id);
        Task<String> DeleteUser(string id);
    }
}
