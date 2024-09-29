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

        Task<ResPagedResultDto<ResUserDto>> GetAllUsers(ReqQueryParametersDto parameter);
        Task<ResUserByIdDto> GetUserById(string id);
        Task<ResLoginDto> Login(ReqLoginDto login);
        Task<string> TopUp(string id, decimal money);
        Task<String> UpdateUser(ReqUpdateUserDto user, string id);
        Task<String> DeleteUser(string id);
    }
}
