using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Res
{
    public class ResUserByIdDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public decimal? Balance { get; set; }
    }
}
