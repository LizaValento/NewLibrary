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
    public class CheckAndUpdateTokensUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CheckAndUpdateTokensUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Execute()
        {
            var refreshTokenEntities = await _unitOfWork.RefreshTokens.GetAllAsync();
            foreach (var refreshTokenEntity in refreshTokenEntities)
            {
                if (refreshTokenEntity.ExpiresAt < DateTime.UtcNow)
                {
                    _unitOfWork.RefreshTokens.Delete(refreshTokenEntity);
                }
            }

            await _unitOfWork.CompleteAsync();
        }
    }
}
