using Application.DTOs;
using Application.UseCases.TokenCase;
using Application.UseCases.UserCase;
using AutoMapper;
using Domain.Interfaces.InterfacesForUOW;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.UseCases.UserCase;

namespace Business.ExceptionHandler
{
    public class TokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JWTSettings _jwtSettings;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMapper _mapper;

        public TokenMiddleware(
            RequestDelegate next,
            IServiceScopeFactory serviceScopeFactory,
            IOptions<JWTSettings> jwtSettings,
            IMapper mapper)
        {
            _next = next;
            _serviceScopeFactory = serviceScopeFactory;
            _jwtSettings = jwtSettings.Value;
            _mapper = mapper;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Cookies["AccessToken"];
            var refreshToken = context.Request.Cookies["RefreshToken"];

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                // Разрешаем сервисы из созданной области
                var getRefreshTokenUseCase = scope.ServiceProvider.GetRequiredService<GetRefreshTokenUseCase>();
                var getUserByIdUseCase = scope.ServiceProvider.GetRequiredService<GetUserByIdUseCase>();
                var generateAccessTokenUseCase = scope.ServiceProvider.GetRequiredService<GenerateAccessTokenUseCase>();

                // Проверяем и обрабатываем токены как раньше
                if (!string.IsNullOrEmpty(refreshToken) && await ValidateRefreshTokenAsync(getRefreshTokenUseCase, refreshToken))
                {
                    if (string.IsNullOrEmpty(token) || !ValidateAccessToken(token))
                    {
                        var userId = await GetUserIdFromRefreshTokenAsync(getRefreshTokenUseCase, refreshToken);
                        if (userId != null)
                        {
                            var user = await getUserByIdUseCase.ExecuteAsync(userId.Value);
                            var claims = new[]
                            {
                            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                            new Claim(ClaimTypes.Name, user.Nickname),
                            new Claim(ClaimTypes.Role, user.Role)
                        };

                            var newAccessToken = generateAccessTokenUseCase.Execute(claims);

                            // Устанавливаем контекст пользователя и куки
                            var claimsIdentity = new ClaimsIdentity(claims, "Custom");
                            context.User = new ClaimsPrincipal(claimsIdentity);

                            context.Response.Cookies.Append("AccessToken", newAccessToken, new CookieOptions
                            {
                                HttpOnly = true,
                                Secure = true,
                                SameSite = SameSiteMode.Strict
                            });
                        }
                    }
                }
            }

            await _next(context);
        }


        private bool ValidateAccessToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)),
                    ValidateIssuer = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtSettings.Audience,
                    ClockSkew = TimeSpan.Zero
                };

                tokenHandler.ValidateToken(token, validationParameters, out _);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> ValidateRefreshTokenAsync(GetRefreshTokenUseCase getRefreshTokenUseCase, string refreshToken)
        {
            var refreshTokenEntity = await getRefreshTokenUseCase.ExecuteAsync(refreshToken);
            return refreshTokenEntity != null && refreshTokenEntity.ExpiresAt > DateTime.UtcNow;
        }

        private async Task<int?> GetUserIdFromRefreshTokenAsync(GetRefreshTokenUseCase getRefreshTokenUseCase, string refreshToken)
        {
            var refreshTokenEntity = await getRefreshTokenUseCase.ExecuteAsync(refreshToken);
            return refreshTokenEntity?.UserId;
        }
    }
}
