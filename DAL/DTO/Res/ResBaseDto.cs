using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Res
{
    public class ResBaseDto
    {
        public bool Success { get; set; }
        public object Message { get; set; }
        public object Data { get; set; }

        public ResBaseDto()
        {
            Success = false;
            Message = string.Empty;
            Data = default;
        }

        public void SetMessage(string message)
        {
            Message = message;
        }

        public void SetMessage(Dictionary<string, List<string>> messages)
        {
            Message = messages;
        }

        public void SetMessage(IEnumerable<string> messages)
        {
            Message = messages;
        }
    }
}
