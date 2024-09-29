using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    [Table("trn_repayment")]
    public class TrnRepayment
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [ForeignKey("Loan")]
        [Column("loan_id")]
        public string LoanId { get; set; }

        [Required]
        [Column("amount")]
        public decimal Amount { get; set; }

        [Required]
        [Column("repaid_amount")]
        public decimal RepaidAmount { get; set; }

        [Required]
        [Column("balance_amount")]
        public decimal BalanceAmount { get; set; }

        [Required]
        [Column("repaid_status")]
        public string RepaidStatus { get; set; }

        [Required]
        [Column("paid_at")]
        public DateTime PaidAt { get; set; } = DateTime.UtcNow;

        public MstLoans Loan { get; set; }
    }
}