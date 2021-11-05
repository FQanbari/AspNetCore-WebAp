using AspNetCore_WebApi.Common;
using AspNetCore_WebApi.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore_WebApi.Services.Services
{
    public class JwtService : IJwtService
    {
        private readonly SiteSettings _siteSetting;

        public JwtService(IOptionsSnapshot<SiteSettings> settings)
        {
            _siteSetting = settings.Value;
        }
        public string Generate(User user)
        {
            var secretKey = Encoding.UTF8.GetBytes(_siteSetting.JwtSettings.SecretKey); //longer than 16 character
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature);
            var claims = _getClaims(user);
            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = _siteSetting.JwtSettings.Issuer,
                Audience = _siteSetting.JwtSettings.Audience,
                IssuedAt = DateTime.Now,
                NotBefore = DateTime.Now.AddHours(_siteSetting.JwtSettings.NotBeforeMinutes),
                Expires = DateTime.Now.AddHours(_siteSetting.JwtSettings.ExpirationMinutes),
                SigningCredentials = signingCredentials,
                Subject = new ClaimsIdentity(claims)
            };
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(descriptor);
            var jwt = tokenHandler.WriteToken(securityToken);
            return jwt;
        }

        private IEnumerable<Claim> _getClaims(User user)
        {
            //JwtRegisteredClaimNames.Name;
            var list = new List<Claim>
            {
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
            };
            var roles = new Role[] { new Role { Name = "Admin" } };
            foreach (var role in roles)
                list.Add(new Claim(ClaimTypes.Role, role.Name));

            return list;
        }
    }
}
