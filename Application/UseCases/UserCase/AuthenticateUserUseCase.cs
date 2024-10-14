using Application.DTOs;
using Application.UseCases.TokenCase;
using AutoMapper;
using Domain.Interfaces.InterfacesForUOW;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Application.UseCases.UserCase
{
    public class AuthenticateUserUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly GenerateAccessTokenUseCase _generateAccessTokenUseCase;
        private readonly GenerateRefreshTokenUseCase _generateRefreshTokenUseCase;
        private readonly SaveRefreshTokenUseCase _saveRefreshTokenUseCase;
        private readonly SetCookiesUseCase _setCookiesUseCase;

        public AuthenticateUserUseCase(IUnitOfWork unitOfWork, IOptions<JWTSettings> jwtSettings, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _generateAccessTokenUseCase = new GenerateAccessTokenUseCase(jwtSettings);
            _generateRefreshTokenUseCase = new GenerateRefreshTokenUseCase();
            _saveRefreshTokenUseCase = new SaveRefreshTokenUseCase(_unitOfWork, mapper);
            _setCookiesUseCase = new SetCookiesUseCase(); 
        }

        public TokenModel Execute(LoginModel model, HttpContext httpContext)
        {
            var user = _unitOfWork.Users.GetByNickname(model.Nickname);
            if (user == null || user.Password != model.Password)
            {
                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Nickname),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var accessToken = _generateAccessTokenUseCase.Execute(claims);
            var refreshToken = _generateRefreshTokenUseCase.Execute();

            _saveRefreshTokenUseCase.Execute(user.Id, refreshToken);

            _setCookiesUseCase.Execute(new TokenModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            }, httpContext);

            return new TokenModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
    }
}
