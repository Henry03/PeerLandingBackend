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
    public class InstallmentServices : IInstallmentServices
    {
        private readonly PeerlendingContext _peerlendingContext;

        public InstallmentServices(PeerlendingContext peerlendingContext)
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

                lender.Balance -= loanData.Amount;

                var borrower = await _peerlendingContext.MstUsers.FirstOrDefaultAsync(x => x.Id == loanData.BorrowerId);
                borrower.Balance += loanData.Amount;

                await _peerlendingContext.SaveChangesAsync();

                return newFunding.LenderId;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<List<ResInstallmentDto>> GetInstallmentById(string id)
        {
            var installments = await _peerlendingContext.TrnInstallments
                .Where(x => x.LoanId == id)
                .OrderBy(x => x.Issue)
                .ToListAsync();

            return installments.Select(x => new ResInstallmentDto
            {
                Id = x.Id,
                Issue = x.Issue,
                Amount = x.Amount,
                PaidAt = x.PaidAt
            }).ToList();
        }

        public async Task<string> PayInstallments(ReqInstallmentsDto installment, string id)
        {
            using (var transaction = await _peerlendingContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var installments = _peerlendingContext.TrnInstallments
                                           .Where(i => installment.Id.Contains(i.Id) && i.PaidAt == DateTime.MinValue)
                                           .ToList();

                    if (installments.Count == 0)
                    {
                        throw new Exception("No valid installments found for payment.");
                    }

                    foreach (var item in installments)
                    {
                        item.PaidAt = DateTime.UtcNow;
                    }

                    var totalInstallmentAmount = installments.Sum(i => i.Amount);
                    var user = _peerlendingContext.MstUsers.FirstOrDefault(u => u.Id == id);
                    var funding = _peerlendingContext.TrnFundings.FirstOrDefault(f => f.LoanId == installment.LoanId);
                    var lender = _peerlendingContext.MstUsers.FirstOrDefault(u => u.Id == funding.LenderId);
                    var repayment = _peerlendingContext.TrnRepayments.FirstOrDefault(r => r.LoanId == installment.LoanId);
                    var loan = _peerlendingContext.MstLoans.FirstOrDefault(l => l.Id == installment.LoanId);

                    if (user.Balance < totalInstallmentAmount)
                    {
                        throw new Exception("Insufficient balance.");
                    }

                    user.Balance -= totalInstallmentAmount;
                    lender.Balance += totalInstallmentAmount;

                    repayment.RepaidAmount += totalInstallmentAmount;
                    repayment.BalanceAmount -= totalInstallmentAmount;

                    await _peerlendingContext.SaveChangesAsync();

                    var unpaidInstallments = _peerlendingContext.TrnInstallments
                                               .Where(i => i.LoanId == installment.LoanId && i.PaidAt == DateTime.MinValue)
                                               .ToList();

                    if (!unpaidInstallments.Any())
                    {
                        repayment.RepaidStatus = "Done";
                        repayment.PaidAt = DateTime.UtcNow;
                        loan.Status = "Repaid";
                        loan.UpdatedAt = DateTime.UtcNow;
                        await _peerlendingContext.SaveChangesAsync();  
                    }

                    await transaction.CommitAsync();

                    return "Success: Installment payment completed.";
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception($"Error processing installments: {ex.Message}");
                }
            }
        }
    }
}
