﻿using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Repositories.Services;
using DAL.Repositories.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;

namespace BEPeer.Controllers
{
    [Route("rest/v1/user/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userServices;
        public UserController(IUserServices userServices)
        {
            _userServices = userServices;
        }

        [HttpPost]
        public async Task<IActionResult> Register(ReqRegisterUserDto register)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Any())
                        .Select(x => new
                        {
                            Field = x.Key,
                            Messages = x.Value.Errors.Select(e => e.ErrorMessage).ToList()
                        }).ToList();

                    var errorMessages = new StringBuilder("Validation Errors Occured!");

                    return BadRequest(new ResBaseDto<object>
                    {
                        Success = false,
                        Message = errorMessages.ToString(),
                        Data = errors
                    });
                }

                var res = await _userServices.Register(register);
                
                return Ok(new ResBaseDto<string>
                {
                    Success = true,
                    Message = "User registered successfully",
                    Data = res
                });
            }
            catch (Exception e)
            {
                if(e.Message == "Email already used")
                {
                    return BadRequest(new ResBaseDto<object>
                    {
                        Success = false,
                        Message = e.Message,
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

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userServices.GetAllUsers();

                return Ok(new ResBaseDto<List<ResUserDto>>
                {
                    Success = true,
                    Message = "List of users",
                    Data = users
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<object>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });

            }
        }

        [HttpPost]
        public async Task<IActionResult> Login(ReqLoginDto login)
        {
            try
            {
                var res = await _userServices.Login(login);

                return Ok(new ResBaseDto<object>
                {
                    Success = true,
                    Message = "Login successful",
                    Data = res
                });
            }
            catch (Exception e)
            {
                if(e.Message == "Invalid email or password")
                {
                    return BadRequest(new ResBaseDto<object>
                    {
                        Success = false,
                        Message = e.Message,
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

        [HttpPut("id")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(ReqUpdateUserDto user, string id)
        {
            try
            {
                var res = await _userServices.UpdateUser(user, id);

                return Ok(new ResBaseDto<object>
                {
                    Success = true,
                    Message = "User updated successfully",
                    Data = res
                });
            }
            catch (Exception e)
            {
                if (e.Message == "User not found")
                {
                    return BadRequest(new ResBaseDto<object>
                    {
                        Success = false,
                        Message = e.Message,
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

        [HttpPut("id")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> User(ReqUpdateUserDto user, string id)
        {
            try
            {
                //var res = await _userServices.UpdateUser(user, id);

                return Ok(new ResBaseDto<object>
                {
                    Success = true,
                    Message = ClaimTypes.Name,
                    Data = null
                });
            }
            catch (Exception e)
            {
                if (e.Message == "User not found")
                {
                    return BadRequest(new ResBaseDto<object>
                    {
                        Success = false,
                        Message = e.Message,
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


        [HttpDelete("id")]
        [Authorize(Roles = "Admin")]    
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var res = await _userServices.DeleteUser(id);

                return Ok(new ResBaseDto<object>
                {
                    Success = true,
                    Message = "User deleted successfully",
                    Data = res
                });
            }
            catch (Exception e)
            {
                if (e.Message == "User not found")
                {
                    return BadRequest(new ResBaseDto<object>
                    {
                        Success = false,
                        Message = e.Message,
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
