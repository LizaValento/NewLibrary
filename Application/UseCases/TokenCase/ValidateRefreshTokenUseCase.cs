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
    public class ValidateRefreshTokenUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ValidateRefreshTokenUseCase(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> Execute(int userId, string token)
        {
            var refreshToken = await new GetRefreshTokenUseCase(_unitOfWork, _mapper).ExecuteAsync(token);
            return refreshToken != null && refreshToken.UserId == userId && refreshToken.ExpiresAt > DateTime.UtcNow;
        }
    }
}
