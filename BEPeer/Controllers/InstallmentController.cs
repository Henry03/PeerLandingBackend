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
    public class InstallmentController : ControllerBase
    {
        private readonly IInstallmentServices _installmentServices;

        public InstallmentController(IInstallmentServices installmentServices)
        {
            _installmentServices = installmentServices;
        }

        [HttpGet]
        [Authorize(Roles="borrower")]
        public async Task<IActionResult> Installment([FromQuery] string id)
        {
            try
            {
                var res = await _installmentServices.GetInstallmentById(id);

                return Ok(new ResBaseDto<List<ResInstallmentDto>>
                {
                    Success = true,
                    Message = "User found",
                    Data = res
                });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<object>
                {
                    Success = false,
                    Message = e.Message,
                    Data = null
                });
            }
        }

        [HttpPost]
        [Authorize(Roles = "borrower")]
        public async Task<IActionResult> Installment([FromBody] ReqInstallmentsDto installments)
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
                var res = await _installmentServices.PayInstallments(installments, userId);

                return Ok(new ResBaseDto<string>
                {
                    Success = true,
                    Message = "User found",
                    Data = res
                });
            }
            catch (Exception e)
            {
                if (e.Message == "Error processing installments: Insufficient balance.")
                {
                    return BadRequest(new ResBaseDto<string>
                    {
                        Success = false,
                        Message = "Insufficient balance.",
                        Data = null
                    });
                }

                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<object>
                {
                    Success = false,
                    Message = e.Message,
                    Data = null
                });
            }
        }
    }
}
