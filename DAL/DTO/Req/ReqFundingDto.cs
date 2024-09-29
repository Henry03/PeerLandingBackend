using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Req
{
    public class ReqFundingDto
    {
        [Required(ErrorMessage = "Load Id Required")]
        public string LoanId { get; set; }
    }
}
