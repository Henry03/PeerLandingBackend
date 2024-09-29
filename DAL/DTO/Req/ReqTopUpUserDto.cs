using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Req
{
    public class ReqTopUpUserDto
    {
        [Range(1, double.MaxValue, ErrorMessage = "Balance cannot be negative or zero")]
        public decimal Balance { get; set; } = 0;
    }
}
