using Application.DTOs;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.InterfacesForUOW;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.UseCases.TokenCase
{
    public class SaveRefreshTokenUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SaveRefreshTokenUseCase(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public void Execute(int userId, string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(nameof(token), "Token не может быть null или пустым.");
            }

            var refreshTokenEntity = _unitOfWork.RefreshTokens.GetByUserId(userId);

            if (refreshTokenEntity != null)
            {
                refreshTokenEntity.Token = token;
                refreshTokenEntity.ExpiresAt = DateTime.UtcNow.AddDays(30);
                _unitOfWork.RefreshTokens.Update(refreshTokenEntity);
            }
            else
            {
                var refreshToken = new RefreshTokenModel
                {
                    UserId = userId,
                    Token = token,
                    ExpiresAt = DateTime.UtcNow.AddDays(30)
                };

                var refreshTokenEntityNew = _mapper.Map<RefreshToken>(refreshToken);
                _unitOfWork.RefreshTokens.Add(refreshTokenEntityNew);
            }

            _unitOfWork.Complete();
        }
        public async Task ExecuteAsync(int userId, string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(nameof(token), "Token не может быть null или пустым.");
            }

            var refreshTokenEntity = await _unitOfWork.RefreshTokens.GetByUserIdAsync(userId);
            if (refreshTokenEntity != null)
            {
                refreshTokenEntity.Token = token;
                refreshTokenEntity.ExpiresAt = DateTime.UtcNow.AddDays(30);
                _unitOfWork.RefreshTokens.Update(refreshTokenEntity);
            }
            else
            {
                var refreshToken = new RefreshTokenModel
                {
                    UserId = userId,
                    Token = token,
                    ExpiresAt = DateTime.UtcNow.AddDays(30)
                };

                var refreshTokenEntityNew = new RefreshToken
                {
                    UserId = refreshToken.UserId,
                    Token = refreshToken.Token,
                    ExpiresAt = refreshToken.ExpiresAt
                };
                _unitOfWork.RefreshTokens.Add(refreshTokenEntityNew);
            }

            await _unitOfWork.CompleteAsync();
        }
    }
}
