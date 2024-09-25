using DAL.DTO.Req;
using DAL.DTO.Res;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Services.Interfaces
{
    public interface ILoanServices
    {
        Task<string> CreateLoan(ReqLoanDto loan);
        Task<List<ResListLoanDto>> GetLoanList(string status);
        Task<string> UpdateLoan(ReqUpdateLoanDto loan, string id);
        

    }
}
