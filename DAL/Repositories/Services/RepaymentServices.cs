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
    public class RepaymentServices : IRepaymentServices
    {
        private readonly PeerlendingContext _peerlendingContext;

        public RepaymentServices(PeerlendingContext peerlendingContext)
        {
            _peerlendingContext = peerlendingContext;
        }

        public async Task<string> CreateFunding(ReqFundingDto loan, string id)
        {
            try
            {
                var loanData = await _peerlendingContext.MstLoans.FirstOrDefaultAsync(x => x.Id == loan.LoanId);

                if (loanData == null)
                {
                    throw new Exception("Loan not found.");
                }

                var lender = await _peerlendingContext.MstUsers.FirstOrDefaultAsync(x => x.Id == id);

                if (loanData.Amount > lender.Balance)
                {
                    throw new Exception("Insufficient balance.");
                }

                loanData.Status = "Funded";

                var newFunding = new TrnFunding
                {
                    LenderId = id,
                    LoanId = loan.LoanId,
                    Amount = loanData.Amount
                }; 

                await _peerlendingContext.TrnFundings.AddAsync(newFunding);


                var monthlyInterestRate = (double)(loanData.InterestRate / 100 / 12);
                var monthlyPayment = (decimal)((monthlyInterestRate * (double)loanData.Amount)
                                               / (1 - Math.Pow((1 + monthlyInterestRate), (-loanData.Duration))));

                for (int i = 1; i <= loanData.Duration; i++)
                {
                    var installment = new TrnInstallment
                    {
                        LoanId = loan.LoanId,
                        Issue = i,  
                        Amount = monthlyPayment
                    };

                    _peerlendingContext.TrnInstallments.Add(installment);
                }

                var repayment = new TrnRepayment
                {
                    LoanId = loan.LoanId,
                    Amount = monthlyPayment * loanData.Duration,
                    RepaidAmount = 0,
                    BalanceAmount = monthlyPayment * loanData.Duration,
                    RepaidStatus = "On Repay",
                    PaidAt = DateTime.UtcNow
                };

                await _peerlendingContext.TrnRepayments.AddAsync(repayment);

                lender.Balance -= loanData.Amount;

                var borrower = await _peerlendingContext.MstUsers.FirstOrDefaultAsync(x => x.Id == loanData.BorrowerId);
                borrower.Balance += loanData.Amount;

                await _peerlendingContext.SaveChangesAsync();

                return newFunding.LenderId;
            }catch(Exception e)
            {
                throw;
            }
        }

        public Task<ResRepaidDto> GetRepaidDetail(string id)
        {
            try
            {
                var repayment = _peerlendingContext.TrnRepayments.FirstOrDefault(x => x.LoanId == id);

                if (repayment == null)
                {
                    throw new Exception("Repayment not found.");
                }

                var res = new ResRepaidDto
                {
                    Amount = repayment.Amount,
                    PaidDate = repayment.PaidAt
                };

                return Task.FromResult(res);

            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<ResPagedResultDto<ResRepaymentDto>> GetRepayments(ReqQueryParametersDto parameter, string id)
        {
            parameter.SortBy = string.IsNullOrEmpty(parameter.SortBy) ? "CreatedAt" : parameter.SortBy;
            parameter.Ascending = parameter.Ascending ? parameter.Ascending : false;


            var query = _peerlendingContext.TrnFundings
                .Join(
                    _peerlendingContext.MstLoans,
                    funding => funding.LoanId,
                    loan => loan.Id,
                    (funding, loan) => new { funding, loan }
                    )
                .Join(
                    _peerlendingContext.TrnRepayments,
                    funding => funding.loan.Id,
                    repayment => repayment.LoanId,
                    (funding, repayment) => new { funding, repayment }
                    )
                .Select(x => new ResRepaymentDto
                {
                    Id = x.repayment.Id,
                    LoanId = x.repayment.LoanId,
                    LenderId = x.funding.funding.LenderId,
                    BorrowerName = x.funding.loan.User.Name,
                    Amount = x.repayment.Amount,
                    RepaidAmount = x.repayment.RepaidAmount,
                    BalanceAmount = x.repayment.BalanceAmount,
                    RepaidStatus = x.repayment.RepaidStatus,
                    PaidAt = x.repayment.PaidAt,
                    FundedAt = x.funding.funding.FundedAt

                })
                .Where(x => x.LenderId == id);

            if (!string.IsNullOrEmpty(parameter.Search))
            {
                query = query.Where(x => x.BorrowerName.Contains(parameter.Search) ||
                                         x.Amount.ToString().Contains(parameter.Search));
            }

            query = parameter.SortBy.ToLower() switch
            {
                "fundedat" => parameter.Ascending ? query.OrderBy(x => x.FundedAt) : query.OrderByDescending(x => x.FundedAt),
                "name" => parameter.Ascending ? query.OrderBy(x => x.BorrowerName) : query.OrderByDescending(x => x.BorrowerName),
                "amount" => parameter.Ascending ? query.OrderBy(x => x.Amount) : query.OrderByDescending(x => x.Amount),
                "repaid" => parameter.Ascending ? query.OrderBy(x => x.RepaidAmount) : query.OrderByDescending(x => x.RepaidAmount),
                "balance" => parameter.Ascending ? query.OrderBy(x => x.BalanceAmount) : query.OrderByDescending(x => x.BalanceAmount),
                "status" => parameter.Ascending ? query.OrderBy(x => x.RepaidStatus) : query.OrderByDescending(x => x.RepaidStatus),
                _ => parameter.Ascending ? query.OrderBy(x => x.FundedAt) : query.OrderByDescending(x => x.FundedAt)
            };

            var totalItems = await query.CountAsync();

            var pagedData = await query
                .Skip((parameter.PageNumber - 1) * parameter.PageSize)
                .Take(parameter.PageSize)
                .ToListAsync();

            return new ResPagedResultDto<ResRepaymentDto>
            {
                Data = pagedData,
                TotalItems = totalItems,
                PageSize = parameter.PageSize,
                PageNumber = parameter.PageNumber
            };
        }
    }
}
