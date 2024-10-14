using Application.DTOs;
using AutoMapper;
using Domain.Interfaces.InterfacesForUOW;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.UseCases.TokenCase
{
    public class GenerateRefreshTokenUseCase
    {
        public string Execute()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
