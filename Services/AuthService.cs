using Microsoft.AspNetCore.Identity;
using WebWerverPart.Models;
using WebWerverPart.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace WebWerverPart.Services
{
    public class AuthService
    {
        private readonly IvanvisionDbContext _db;
        private readonly PasswordHasher<User> _hasher = new PasswordHasher<User>();
        private JWTService _jwtService;

        public AuthService(IvanvisionDbContext db, JWTService jwtService)
        {
            _db = db;
            _jwtService = jwtService;
        }

        public async Task<(bool Success, string Message)> RegisterAsync(RegisterDTO dto)
        {
            // проверяем уникальность логина
            if (await _db.Users.AnyAsync(u => u.UserLogin == dto.UserLogin))
                return (false, "Login already exists");

            // создаём пользователя
            var user = new User
            {
                UserLogin = dto.UserLogin,
                UserEmail = dto.UserEmail
            };

            // хешируем пароль
            user.PasswordHash = _hasher.HashPassword(user, dto.Password);

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return (true, "Registration successful");
        }
        public async Task<(bool Success, string AccessToken, string RefreshToken)> LoginAsync(LoginDTO dto)
        {
            var user = await _db.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(x => x.UserEmail == dto.UserEmail);

            if (user == null)
                return (false, "", "");

            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);

            if (result != PasswordVerificationResult.Success)
                return (false, "", "");

            var accessToken = _jwtService.GenerateToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.IdUser,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false
            };

            _db.RefreshTokens.Add(refreshTokenEntity);
            await _db.SaveChangesAsync();

            return (true, accessToken, refreshToken);

        }

    }
}
