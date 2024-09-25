using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Models;
using DAL.Repositories.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<string> CreateLoan(ReqLoanDto loan)
        {
            var newLoan = new MstLoans
            {
                BorrowerId = loan.BorrowerId,
                Amount = loan.Amount,
                InterestRate = loan.InterestRate,
                Duration = loan.Duration,
            };

            await _peerlendingContext.MstLoans.AddAsync(newLoan);
            await _peerlendingContext.SaveChangesAsync();

            return newLoan.BorrowerId;
        }

        public Task<List<ResListLoanDto>> GetLoanList(string status)
        {
            var listLoan = _peerlendingContext.MstLoans
                .Select(x => new ResListLoanDto
                {
                    LoanId = x.Id,
                    BorrowerName = x.User.Name,
                    Amount = x.Amount,
                    InterestRate = x.InterestRate,
                    Duration = x.Duration,
                    Status = x.Status,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt
                })
                .Where(x => x.Status.Contains(status))
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return listLoan;
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
