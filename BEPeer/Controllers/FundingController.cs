using DAL.DTO.Req;
using DAL.DTO.Res;
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
    public class FundingController : ControllerBase
    {
        private readonly IFundingServices _fundingServices;

        public FundingController(IFundingServices fundingServices)
        {
            _fundingServices = fundingServices;
        }

        [HttpPost]
        [Authorize(Roles="lender")]
        public async Task<IActionResult> Funding(ReqFundingDto funding)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Any())
                        .Select(x => new
                        {
                            Field = x.Key,
                            Message = x.Value.Errors.Select(e => e.ErrorMessage).ToList()
                        }).ToList();

                    var errorMessage = new StringBuilder("Validation errors occured!");

                    return BadRequest(new ResBaseDto<object>
                    {
                        Success = false,
                        Message = errorMessage.ToString(),
                        Data = errors
                    });
                }

                var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value;

                var res = await _fundingServices.CreateFunding(funding, userId);

                return Ok(new ResBaseDto<string>
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
    }
}
