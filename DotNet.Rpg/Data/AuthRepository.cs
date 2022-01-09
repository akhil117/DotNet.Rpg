using DotNet.Rpg.Data.Interface;
using DotNet.Rpg.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Rpg.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IDataContext _dataContext;
        private readonly IConfiguration _configuration;
        public AuthRepository(IDataContext dataContext, IConfiguration configuration)
        {
            _configuration = configuration;
            _dataContext = dataContext;
        }

        public async Task<ServiceResponse<string>> Login(string userName, string password)
        {
            var response = new ServiceResponse<string>();
            var user = await _dataContext.Users.SingleOrDefaultAsync(x => x.UserName.ToLower().Equals(userName.ToLower()));
            if (user == null)
            {
                response.Data = "INVALID";
                response.message = "The provided userId is invalid";
                response.success = false;

                return response;
            }

            if (verifyPassword(password, user.PasswordHash, user.PasswordSalt))
            {
                response.Data = CreateToken(user);
                response.message = "Valid user";
                response.success = true;
            }
            else
            {
                response.Data = "INVALID";
                response.message = "The provided password is invalid";
                response.success = false;
            }

            return response;
        }

        public async Task<ServiceResponse<int>> Register(User user, string password)
        {
            var response = new ServiceResponse<int>();


            if (await UserExists(user.UserName))
            {
                response.success = false;
                response.message = "User already exists";
                return response;
            }

            CreateHashPassword(password, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _dataContext.Users.AddAsync(user);
            await _dataContext.SaveAsync(CancellationToken.None);

            response.Data = user.Id;
            return response;
        }

        public async Task<bool> UserExists(string userName)
        {
            if( await _dataContext.Users.SingleOrDefaultAsync(x => x.UserName.ToLower().Equals(userName.ToLower())) != null)
            {
                return true;
            }
            return false;
        }

        private void CreateHashPassword(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool verifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                bool output = true;

                var computedPasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                if (computedPasswordHash.Length != passwordHash.Length)
                {
                    return false;
                }

                for(int i=0;i< computedPasswordHash.Length; i++)
                {
                    if (computedPasswordHash[i] != passwordHash[i])
                    {
                        output = false;
                        break;
                    }
                }
                return output;
            }

        }


        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Id.ToString())
            };

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Settings:Token").Value));

            SigningCredentials signing = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            SecurityTokenDescriptor securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = signing
            };

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);
            return jwtSecurityTokenHandler.WriteToken(token);
        }
    }
}
