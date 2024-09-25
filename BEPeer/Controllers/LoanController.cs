using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Repositories.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace BEPeer.Controllers
{
    [Route("rest/v1/user/[action]")]
    [ApiController]
    public class LoanController : ControllerBase
    {
        private readonly ILoanServices _loanServices;

        public LoanController(ILoanServices loanServices)
        {
            _loanServices = loanServices;
        }

        [HttpPost]
        public async Task<IActionResult> NewLoan(ReqLoanDto loan)
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

                var res = await _loanServices.CreateLoan(loan);

                return Ok(new ResBaseDto<string>
                {
                    Success = true,
                    Message = "Success add loan data",
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

        [HttpPut("id")]
        public async Task<IActionResult> UpdateLoan(ReqUpdateLoanDto loan, string id)
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

                var res = await _loanServices.UpdateLoan(loan, id);

                return Ok(new ResBaseDto<string>
                {
                    Success = true,
                    Message = "Success update loan data",
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

        [HttpGet("status")]
        public async Task<IActionResult> Loans(string status = "")
        {
            try
            {
                var res = await _loanServices.GetLoanList(status);

                return Ok(new ResBaseDto<List<ResListLoanDto>>
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
