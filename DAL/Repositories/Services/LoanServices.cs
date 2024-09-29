using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Models;
using DAL.Repositories.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Services
{
    public class LoanServices : ILoanServices
    {
        private readonly PeerlendingContext _peerlendingContext;

        public LoanServices(PeerlendingContext peerlendingContext)
        {
            _peerlendingContext = peerlendingContext;
        }
        public async Task<string> CreateLoan(ReqLoanDto loan, string id)
        {
            var newLoan = new MstLoans
            {
                Amount = loan.Amount,
                InterestRate = loan.InterestRate,
                Duration = loan.Duration,
                BorrowerId = id
            };

            await _peerlendingContext.MstLoans.AddAsync(newLoan);
            await _peerlendingContext.SaveChangesAsync();

            return newLoan.BorrowerId;
        }

        //public Task<string> GetLoanDetail(string id)
        //{
        //    var loan = _peerlendingContext.MstLoans.Find(id);

        //    return loan.BorrowerId;

        //}

        async public Task<ResPagedResultDto<ResListLoanDto>> GetLoanList(string status, ReqQueryParametersDto parameter)
        {
            parameter.SortBy = string.IsNullOrEmpty(parameter.SortBy) ? "CreatedAt" : parameter.SortBy;
            parameter.Ascending = parameter.Ascending ? parameter.Ascending : false;


            var query = _peerlendingContext.MstLoans
                .Select(x => new ResListLoanDto
                {
                    LoanId = x.Id,
                    BorrowerId = x.BorrowerId,
                    BorrowerName = x.User.Name,
                    Amount = x.Amount,
                    InterestRate = x.InterestRate,
                    Duration = x.Duration,
                    Status = x.Status,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt
                })
                .Where(x => x.Status.Contains(status));

            if (!string.IsNullOrEmpty(parameter.Search))
            {
                query = query.Where(x => x.BorrowerName.Contains(parameter.Search) ||
                                         x.Status.Contains(parameter.Search) ||
                                         x.Amount.ToString().Contains(parameter.Search));
            }

            query = parameter.SortBy.ToLower() switch
            {
                "name" => parameter.Ascending ? query.OrderBy(x => x.BorrowerName) : query.OrderByDescending(x => x.BorrowerName),
                "amount" => parameter.Ascending ? query.OrderBy(x => x.Amount) : query.OrderByDescending(x => x.Amount),
                "interestrate" => parameter.Ascending ? query.OrderBy(x => x.InterestRate) : query.OrderByDescending(x => x.InterestRate),
                "duration" => parameter.Ascending ? query.OrderBy(x => x.Duration) : query.OrderByDescending(x => x.Duration),
                "status" => parameter.Ascending ? query.OrderBy(x => x.Status) : query.OrderByDescending(x => x.Status),
                _ => parameter.Ascending ? query.OrderBy(x => x.CreatedAt) : query.OrderByDescending(x => x.CreatedAt)
            };

            var totalItems = await query.CountAsync();

            var pagedData = await query
                .Skip((parameter.PageNumber - 1) * parameter.PageSize)
                .Take(parameter.PageSize)
                .ToListAsync();

            return new ResPagedResultDto<ResListLoanDto>
            {
                Data = pagedData,
                TotalItems = totalItems,
                PageSize = parameter.PageSize,
                PageNumber = parameter.PageNumber
            };
        }

        public async Task<ResPagedResultDto<ResListLoanDto>> GetLoanListById(string status, string id, ReqQueryParametersDto parameter)
        {
            parameter.SortBy = string.IsNullOrEmpty(parameter.SortBy) ? "CreatedAt" : parameter.SortBy;
            parameter.Ascending = parameter.Ascending ? parameter.Ascending : false;


            var query = _peerlendingContext.MstLoans
                .Select(x => new ResListLoanDto
                {
                    LoanId = x.Id,
                    BorrowerId = x.BorrowerId,
                    BorrowerName = x.User.Name,
                    Amount = x.Amount,
                    InterestRate = x.InterestRate,
                    Duration = x.Duration,
                    Status = x.Status,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt
                })
                .Where(x => x.Status.Contains(status))
                .Where(x => x.BorrowerId == id);

            if (!string.IsNullOrEmpty(parameter.Search))
            {
                query = query.Where(x => x.Status.Contains(parameter.Search) ||
                                         x.Amount.ToString().Contains(parameter.Search));
            }

            query = parameter.SortBy.ToLower() switch
            {
                "createdat" => parameter.Ascending ? query.OrderBy(x => x.CreatedAt) : query.OrderByDescending(x => x.CreatedAt),
                "amount" => parameter.Ascending ? query.OrderBy(x => x.Amount) : query.OrderByDescending(x => x.Amount),
                "interestrate" => parameter.Ascending ? query.OrderBy(x => x.InterestRate) : query.OrderByDescending(x => x.InterestRate),
                "duration" => parameter.Ascending ? query.OrderBy(x => x.Duration) : query.OrderByDescending(x => x.Duration),
                "status" => parameter.Ascending ? query.OrderBy(x => x.Status) : query.OrderByDescending(x => x.Status),
                _ => parameter.Ascending ? query.OrderBy(x => x.CreatedAt) : query.OrderByDescending(x => x.CreatedAt)
            };

            var totalItems = await query.CountAsync();

            var pagedData = await query
                .Skip((parameter.PageNumber - 1) * parameter.PageSize)
                .Take(parameter.PageSize)
                .ToListAsync();

            return new ResPagedResultDto<ResListLoanDto>
            {
                Data = pagedData,
                TotalItems = totalItems,
                PageSize = parameter.PageSize,
                PageNumber = parameter.PageNumber
            };
        }

        public Task<string> UpdateLoan(ReqUpdateLoanDto loan, string id)
        {
            var updatedLoan = _peerlendingContext.MstLoans.Find(id);

            if (updatedLoan == null)
            {
                return Task.FromResult("Loan not found");
            }

            updatedLoan.Status = loan.Status;
            updatedLoan.UpdatedAt = DateTime.UtcNow;

            _peerlendingContext.MstLoans.Update(updatedLoan);
            _peerlendingContext.SaveChanges();

            return Task.FromResult("Loan updated");
        }
    }
}
