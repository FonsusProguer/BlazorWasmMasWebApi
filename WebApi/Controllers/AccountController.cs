using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using WebApi.Dto;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        //private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IConfigurationSection _jwtSettings;

        //public AccountController(UserManager<IdentityUser> userManager, IConfiguration configuration)
        public AccountController(IConfiguration configuration)
        {
            //_userManager = userManager;
            _configuration = configuration;
            _jwtSettings = _configuration.GetSection("JwtSettings");
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserForAuthenticationDto userForAuthentication)
        {
            //var user = await _userManager.FindByNameAsync(userForAuthentication.Email);
            //if (user == null || !await _userManager.CheckPasswordAsync(user, userForAuthentication.Password))
            //    return Unauthorized(new AuthResponseDto { ErrorMessage = "Invalid Authentication" });

            const string email1 = "fonsuspro@gmail.com";
            const string email2 = "fonsusgn@gmail.com";
            const string email3 = "fonsus.surtidora@gmail.com";

            List<string> roles = new List<string>();
            switch (userForAuthentication.Email)
            {
                case email1:
                    roles.Add("Administrador");
                    break;
                case email2:
                    roles.Add("Visitante");
                    break;
                case email3:
                    roles.Add("Administrador");
                    roles.Add("Visitante");
                    break;
            }

            if (!(userForAuthentication.Email.Equals(email1)
                || userForAuthentication.Email.Equals(email2)
                || userForAuthentication.Email.Equals(email3)))

                return Unauthorized(new AuthResponseDto { ErrorMessage = "Usuario incorrecto" });



            var claims = GetClaims(userForAuthentication.Email, roles);
            //var claims = GetClaims(user);
            var signingCredentials = GetSigningCredentials();
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);
            var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            return Ok(new AuthResponseDto { IsAuthSuccessful = true, Token = token });
        }


        private SigningCredentials GetSigningCredentials()
        {
            var key = Encoding.UTF8.GetBytes(_jwtSettings["securityKey"]);
            var secret = new SymmetricSecurityKey(key);

            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }


        private List<Claim> GetClaims(string user, List<string> roles)
        {

            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, user));

            roles.ForEach(role => 
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            });
            return claims;
        }
       


        //private List<Claim> GetClaims(IdentityUser user)
        //{
        //    var claims = new List<Claim>
        //    {
        //        new Claim(ClaimTypes.Name, user.Email)
        //    };

        //    return claims;
        //}
        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var tokenOptions = new JwtSecurityToken(
                issuer: _jwtSettings["validIssuer"],
                audience: _jwtSettings["validAudience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_jwtSettings["expiryInMinutes"])),
                signingCredentials: signingCredentials);

            return tokenOptions;
        }
    }
}
