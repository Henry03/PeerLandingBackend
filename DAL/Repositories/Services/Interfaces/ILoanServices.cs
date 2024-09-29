using DAL.DTO.Req;
using DAL.DTO.Res;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Services.Interfaces
{
    public interface ILoanServices
    {
        Task<string> CreateLoan(ReqLoanDto loan, string id);
        Task<ResPagedResultDto<ResListLoanDto>> GetLoanList(string status, ReqQueryParametersDto parameter);
        Task<ResPagedResultDto<ResListLoanDto>> GetLoanListById(string status, string id, ReqQueryParametersDto parameter);
        //Task<string> GetLoanDetail(string id);
        Task<string> UpdateLoan(ReqUpdateLoanDto loan, string id);
    }
}
