using Application.DTOs;
using AutoMapper;
using Domain.Interfaces.InterfacesForUOW;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.UseCases.TokenCase
{
    public class GetRefreshTokenUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetRefreshTokenUseCase(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public RefreshTokenModel Execute(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(nameof(token), "Token не может быть null или пустым.");
            }

            var refreshTokenEntity = _unitOfWork.RefreshTokens.GetByToken(token);
            if (refreshTokenEntity == null)
            {
                return null;
            }

            return _mapper.Map<RefreshTokenModel>(refreshTokenEntity);
        }

        public async Task<RefreshTokenModel> ExecuteAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(nameof(token), "Token не может быть null или пустым.");
            }

            var refreshTokenEntity = await _unitOfWork.RefreshTokens.GetByTokenAsync(token);
            if (refreshTokenEntity == null)
            {
                return null;
            }

            return _mapper.Map<RefreshTokenModel>(refreshTokenEntity);
        }
    }
}
