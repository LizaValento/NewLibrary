using Application.DTOs;
using AutoMapper;
using Domain.Interfaces.InterfacesForUOW;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.UseCases.UserCase;
using Microsoft.AspNetCore.Http;

namespace Application.UseCases.TokenCase
{
    public class RefreshTokenUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly GetRefreshTokenUseCase _getRefreshTokenUseCase;
        private readonly GenerateAccessTokenUseCase _generateAccessTokenUseCase;
        private readonly GenerateRefreshTokenUseCase _generateRefreshTokenUseCase;
        private readonly SaveRefreshTokenUseCase _saveRefreshTokenUseCase;
        private readonly SetCookiesUseCase _setCookiesUseCase; 

        public RefreshTokenUseCase(IUnitOfWork unitOfWork, IMapper mapper, IOptions<JWTSettings> jwtSettings)
        {
            _unitOfWork = unitOfWork;
            _getRefreshTokenUseCase = new GetRefreshTokenUseCase(unitOfWork, mapper);
            _generateAccessTokenUseCase = new GenerateAccessTokenUseCase(jwtSettings);
            _generateRefreshTokenUseCase = new GenerateRefreshTokenUseCase();
            _saveRefreshTokenUseCase = new SaveRefreshTokenUseCase(unitOfWork, mapper);
            _mapper = mapper;
            _setCookiesUseCase = new SetCookiesUseCase();
        }

        public async Task<TokenModel> ExecuteAsync(string refreshToken, HttpContext httpContext) 
        {
            var refreshTokenEntity = await _getRefreshTokenUseCase.ExecuteAsync(refreshToken);

            if (refreshTokenEntity == null || refreshTokenEntity.ExpiresAt < DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Invalid refresh token.");
            }

            var user = await _unitOfWork.Users.GetByIdAsync(refreshTokenEntity.UserId);
            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found.");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Nickname),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var newAccessToken = _generateAccessTokenUseCase.Execute(claims);
            var newRefreshToken = _generateRefreshTokenUseCase.Execute();

            await _saveRefreshTokenUseCase.ExecuteAsync(user.Id, newRefreshToken);

            _setCookiesUseCase.Execute(new TokenModel
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            }, httpContext);

            return new TokenModel
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }
    }
}
