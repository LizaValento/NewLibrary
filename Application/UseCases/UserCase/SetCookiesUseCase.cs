using Application.DTOs;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.InterfacesForUOW;
using Microsoft.AspNetCore.Http;

namespace Application.UseCases.UserCase
{
    public class SetCookiesUseCase
    {
        public void Execute(TokenModel tokenModel, HttpContext httpContext)
        {
            httpContext.Response.Cookies.Append("AccessToken", tokenModel.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddMinutes(1)
            });

            httpContext.Response.Cookies.Append("RefreshToken", tokenModel.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(30)
            });
        }
    }

}
