﻿using DAL.DTO.Req;
using DAL.DTO.Res;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Services.Interfaces
{
    public interface IRepaymentServices
    {
        Task<ResRepaidDto> GetRepaidDetail(string id);
        Task<ResPagedResultDto<ResRepaymentDto>> GetRepayments(ReqQueryParametersDto parameter, string id);
    }
}
