using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Models;
using DAL.Repositories.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Services
{
    public class UserServices : IUserServices
    {
        private readonly PeerlendingContext _context;
        private readonly IConfiguration _configuration;

        public UserServices(PeerlendingContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<List<ResUserDto>> GetAllUsers()
        {
            return await _context.MstUsers
                .Select(user => new ResUserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role,
                    Balance = user.Balance
                })
                .Where(user => user.Role != "Admin")
                .ToListAsync();
        }


        public async Task<string> Register(ReqRegisterUserDto register)
        {
            var isAnyEmail = await _context.MstUsers.SingleOrDefaultAsync(e => e.Email == register.Email);

            if(isAnyEmail != null)
            {
                throw new Exception("Email already used");
            }

            var newUser = new MstUser
            {
                Name = register.Name,
                Email = register.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(register.Password),
                Role = register.Role,
                Balance = register.Balance
            };

            await _context.MstUsers.AddAsync(newUser);
            await _context.SaveChangesAsync();

            return newUser.Name;
        }
        public async Task<ResLoginDto> Login(ReqLoginDto reqLogin)
        {
            var user = await _context.MstUsers.SingleOrDefaultAsync(e => e.Email == reqLogin.Email);
            if (user == null)
            {
                throw new Exception("Invalid email or password");
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(reqLogin.Password, user.Password);
            if (!isPasswordValid)
            {
                throw new Exception("Invalid email or password");
            }

            var token = GenerateJwtToken(user);

            var loginResponse = new ResLoginDto
            {
                Token = token
            };

            return loginResponse;
        }

        private string GenerateJwtToken(MstUser user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);   

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["ValidIssuer"],
                audience: jwtSettings["ValidAudience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public Task<String> UpdateUser(ReqUpdateUserDto user, string id)
        {
            var oldUser = _context.MstUsers.Find(id);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            oldUser.Name = user.Name;
            oldUser.Role = user.Role;
            oldUser.Balance = user.Balance;

            _context.SaveChanges();

            return Task.FromResult("User updated successfully");
        }

        public Task<String> DeleteUser(string id)
        {
            var user = _context.MstUsers.Find(id);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            _context.MstUsers.Remove(user);
            _context.SaveChanges();

            return Task.FromResult("User deleted successfully");
        }
    }
}
