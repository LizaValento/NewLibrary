using Application.DTOs;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.InterfacesForUOW;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.UseCases.TokenCase
{
    public class LogoutUseCase
    {
        public void Execute(HttpResponse response)
        {
            response.Cookies.Delete("AccessToken");
            response.Cookies.Delete("RefreshToken");
        }
    }
}
