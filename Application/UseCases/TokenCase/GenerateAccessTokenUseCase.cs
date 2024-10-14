using Application.DTOs;
using AutoMapper;
using Domain.Interfaces.InterfacesForUOW;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.UseCases.TokenCase
{
    public class GenerateAccessTokenUseCase
    {
        private readonly JWTSettings _jwtSettings;

        public GenerateAccessTokenUseCase(IOptions<JWTSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value; 
        }


        public string Execute(IEnumerable<Claim> claims)
        {
            if (claims == null || !claims.Any())
            {
                throw new ArgumentNullException(nameof(claims), "Claims не могут быть null или пустыми.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _jwtSettings.Issuer,
                _jwtSettings.Audience,
                claims,
                expires: DateTime.Now.AddMinutes(_jwtSettings.AccessTokenLifetime),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
