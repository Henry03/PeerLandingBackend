using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    [Table("trn_installment")]
    public class TrnInstallment
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [ForeignKey("Loan")]
        [Column("loan_id")]
        public string LoanId { get; set; }

        [Required]
        [Column("issue")]
        public int Issue { get; set; }

        [Required]
        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("paid_at")]
        public DateTime PaidAt { get; set; }

        public MstLoans Loan { get; set; }
    }
}