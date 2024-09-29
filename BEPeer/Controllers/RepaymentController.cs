using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Repositories.Services;
using DAL.Repositories.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;

namespace BEPeer.Controllers
{
    [Route("rest/v1/user/[action]")]
    [ApiController]
    public class RepaymentController : ControllerBase
    {
        private readonly IRepaymentServices _repaymentServices;

        public RepaymentController(IRepaymentServices repaymentServices)
        {
            _repaymentServices = repaymentServices;
        }

        [HttpGet]
        [Authorize(Roles="borrower")]
        public async Task<IActionResult> RepaidDetail([FromQuery] string idLoan)
        {
            try
            {
                var res = await _repaymentServices.GetRepaidDetail(idLoan);

                return Ok(new ResBaseDto<ResRepaidDto>
                {
                    Success = true,
                    Message = "Success give funding",
                    Data = res
                });
            }
            catch (Exception ex)
            {
                if(ex.Message == "Insufficient balance.")
                {
                    var errorResponse = new
                    {
                        Field = "Balance",
                        Message = new List<string> { ex.Message }
                    };

                    return BadRequest(new ResBaseDto<object>
                    {
                        Success = false,
                        Message = "Funding failed due to insufficient balance.",
                        Data = new List<object> { errorResponse }
                    });
                }
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpPost]
        [Authorize(Roles = "lender")]
        public async Task<IActionResult> RepaymentHistory([FromBody] ReqQueryParametersDto parameter)
        {
            try
            {
                var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value;

                var res = await _repaymentServices.GetRepayments(parameter, userId);

                return Ok(new ResBaseDto<ResPagedResultDto<ResRepaymentDto>>
                {
                    Success = true,
                    Message = "Success get loan list",
                    Data = res
                });
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }
    }
}
