using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Req
{
    public class ReqUpdateLoanDto
    {
        //[Required(ErrorMessage = "BorrowerId is required")]
        //public string BorrowerId { get; set; }

        //[Required(ErrorMessage = "Amount is required")]
        //[Range(0, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        //public decimal Amount { get; set; }

        //[Required(ErrorMessage = "InterestRate is required")]
        //public decimal InterestRate { get; set; }

        //[Required(ErrorMessage = "Duration is required")]
        //public int Duration { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public string Status { get; set; }

    }
}
