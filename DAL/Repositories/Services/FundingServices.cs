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
    public class FundingServices : IFundingServices
    {
        private readonly PeerlendingContext _peerlendingContext;

        public FundingServices(PeerlendingContext peerlendingContext)
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
    }
}
