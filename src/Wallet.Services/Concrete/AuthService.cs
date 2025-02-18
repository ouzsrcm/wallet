using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Wallet.Entities.EntityObjects;
using Wallet.Services.Abstract;
using Wallet.Services.DTOs.Auth;
using Wallet.Services.UnitOfWorkBase.Abstract;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

namespace Wallet.Services.Concrete;

public class AuthService : IAuthService
{
    private readonly IPersonUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;

    public AuthService(IPersonUnitOfWork unitOfWork, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _unitOfWork.Users
            .GetSingleAsync(u => u.Credential!.Username == request.Username);

        if (user == null)
            throw new Exception("User not found");

        var cre = await _unitOfWork.UserCredentials.GetSingleAsync(u => u.UserId == user.Id);

        if (user.Credential == null)
            throw new Exception("User not found");

        user.Credential = cre;

        // Verify password
        var (isValid, computedHash) = VerifyPassword(request.Password, user.Credential.PasswordHash);
        if (!isValid)
            {
                var res = $"Invalid password: {request.Password} {user.Credential.PasswordHash} {computedHash}";
                throw new Exception(res);
            }

        // Generate tokens
        var token = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        // Update user's refresh token
        user.Credential.RefreshToken = refreshToken;
        user.Credential.RefreshTokenExpireDate = DateTime.UtcNow.AddDays(7);
        await _unitOfWork.Users.UpdateAsync(user);

        return new LoginResponseDto
        {
            User = new UserDto
            {
                Id = user.Id,
                Username = user.Credential.Username,
                Email = user.Credential.Email ?? "",
                FirstName = user.Person?.FirstName ?? "",
                LastName = user.Person?.LastName ?? ""
            },
            Token = token,
            RefreshToken = refreshToken
        };
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                ClockSkew = TimeSpan.Zero
            }, out _);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<LoginResponseDto> RefreshTokenAsync(string refreshToken)
    {
        var user = await _unitOfWork.Users
            .GetSingleAsync(u => u.Credential!.RefreshToken == refreshToken);

        if (user == null || user.Credential == null)
            throw new Exception("Invalid refresh token");

        if (user.Credential.RefreshTokenExpireDate < DateTime.UtcNow)
            throw new Exception("Refresh token expired");

        var newToken = GenerateJwtToken(user);
        var newRefreshToken = GenerateRefreshToken();

        user.Credential.RefreshToken = newRefreshToken;
        user.Credential.RefreshTokenExpireDate = DateTime.UtcNow.AddDays(7);
        await _unitOfWork.Users.UpdateAsync(user);

        return new LoginResponseDto
        {
            User = new UserDto
            {
                Id = user.Id,
                Username = user.Credential.Username,
                Email = user.Credential.Email ?? "",
                FirstName = user.Person?.FirstName ?? "",
                LastName = user.Person?.LastName ?? ""
            },
            Token = newToken,
            RefreshToken = newRefreshToken
        };
    }

    public async Task LogoutAsync(Guid userId)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user?.Credential != null)
        {
            user.Credential.RefreshToken = null;
            user.Credential.RefreshTokenExpireDate = null;
            await _unitOfWork.Users.UpdateAsync(user);
        }
    }

    private string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);
        var tokenExpirationInMinutes = int.Parse(_configuration["Jwt:TokenExpirationInMinutes"] ?? "60");

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Credential?.Username ?? ""),
            new(ClaimTypes.Email, user.Credential?.Email ?? "")
        };

        // Add roles if needed
        if (user.Credential?.Roles != null)
        {
            claims.AddRange(user.Credential.Roles.Select(role => new Claim(ClaimTypes.Role, role)));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(tokenExpirationInMinutes),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha512Signature),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            NotBefore = DateTime.UtcNow
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    internal (bool, string) VerifyPassword(string password, string passwordHash)
    {
        var (hash, _) = CreatePasswordHash(password);

        return (hash == passwordHash, hash);
    }

    // Yeni şifre oluşturma için yardımcı metod
    internal (string hash, string salt) CreatePasswordHash(string password)
    {
        // Sabit salt kullan (password'den türetilmiş)
        byte[] saltBytes = Encoding.UTF8.GetBytes(password)
            .Concat(Encoding.UTF8.GetBytes("WalletApp2024!")) // Sabit suffix ekle
            .Take(32) // İlk 32 byte'ı al
            .ToArray();

        // PBKDF2 ile şifreyi hash'le
        using var pbkdf2 = new Rfc2898DeriveBytes(
            password,
            saltBytes,
            10000,  // 10000 iterasyon
            HashAlgorithmName.SHA256);

        // Hash'i hesapla
        byte[] hash = pbkdf2.GetBytes(32); // 256 bit

        // Base64'e çevir
        string hashString = Convert.ToBase64String(hash);
        string saltString = Convert.ToBase64String(saltBytes);

        return (hashString, saltString);
    }

    public async Task<bool> ChangePasswordAsync(Guid userId, string email, string currentPassword, string newPassword)
    {
        // Kullanıcıyı bul
        var user = await _unitOfWork.Users
            .GetSingleAsync(u => /* u.Id == userId &&  */u.Credential!.Email == email);

        if (user == null)
            throw new Exception("User not found");

        // Kullanıcı kimlik bilgilerini al
        var credential = await _unitOfWork.UserCredentials
            .GetSingleAsync(c => /* c.UserId == userId &&  */c.Email == email);

        if (credential == null)
            throw new Exception("User credentials not found");

        // Email doğrulaması yap
        if (!credential.IsEmailVerified)
            throw new Exception("Email is not verified");

        // Mevcut şifreyi doğrula
        var (isValid, _) = VerifyPassword(currentPassword, credential.PasswordHash);
        if (!isValid)
            throw new Exception("Current password is incorrect");

        // Yeni şifre için validasyonlar
        if (string.IsNullOrWhiteSpace(newPassword))
            throw new Exception("New password cannot be empty");

        if (newPassword.Length < 8)
            throw new Exception("New password must be at least 8 characters long");

        if (!newPassword.Any(char.IsUpper))
            throw new Exception("New password must contain at least one uppercase letter");

        if (!newPassword.Any(char.IsLower))
            throw new Exception("New password must contain at least one lowercase letter");

        if (!newPassword.Any(char.IsDigit))
            throw new Exception("New password must contain at least one number");

        if (!newPassword.Any(c => !char.IsLetterOrDigit(c)))
            throw new Exception("New password must contain at least one special character");

        // Yeni şifreyi hash'le
        var (newHash, newSalt) = CreatePasswordHash(newPassword);

        // Kullanıcı bilgilerini güncelle
        credential.PasswordHash = newHash;
        credential.PasswordSalt = newSalt;
        credential.PasswordChangedAt = DateTime.UtcNow;
        credential.RequirePasswordChange = false;
        credential.SecurityStamp = Guid.NewGuid().ToString(); // Güvenlik damgasını yenile
        credential.ModifiedDate = DateTime.UtcNow;
        credential.ModifiedBy = userId.ToString();

        // Değişiklikleri kaydet
        await _unitOfWork.UserCredentials.UpdateAsync(credential);

        // Başarılı
        return true;
    }

    public async Task<UserInfoDto> GetUserInfoAsync(Guid userId)
    {
        return await (from user in _unitOfWork.Users.GetAll()
            join person in _unitOfWork.Persons.GetAll()
                on user.PersonId equals person.Id
            join credential in _unitOfWork.UserCredentials.GetAll()
                on user.Id equals credential.UserId
            where user.Id == userId
            select new UserInfoDto
            {
                Id = user.Id,
                Username = credential.Username,
                FirstName = person.FirstName,
                LastName = person.LastName
            }).FirstOrDefaultAsync() 
            ?? throw new Exception("User not found");
    }
} 