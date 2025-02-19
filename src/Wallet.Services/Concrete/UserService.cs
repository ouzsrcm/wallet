using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Wallet.Entities.EntityObjects;
using Wallet.Services.Abstract;
using Wallet.Services.DTOs.User;
using Wallet.Services.UnitOfWorkBase.Abstract;

namespace Wallet.Services.Concrete;

public class UserService : IUserService
{
    private readonly IPersonUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IAuthService _authService;

    public UserService(
        IPersonUnitOfWork unitOfWork,
        IMapper mapper,
        IAuthService authService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _authService = authService;
    }

    public async Task<UserDto> GetUserByIdAsync(Guid id)
    {
        var user = await _unitOfWork.Users
            .GetWhere(u => u.Id == id && !u.IsDeleted)
            .Include(u => u.Credential)
            .FirstOrDefaultAsync() 
            ?? throw new Exception("User not found");

        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> GetUserByUsernameAsync(string username)
    {
        var user = await _unitOfWork.Users
            .GetWhere(u => u.Credential!.Username == username && !u.IsDeleted)
            .Include(u => u.Credential)
            .FirstOrDefaultAsync() 
            ?? throw new Exception("User not found");

        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto userDto)
    {
        // Kullanıcı adı kontrolü
        var existingUser = await _unitOfWork.UserCredentials
            .GetWhere(uc => uc.Username == userDto.Username)
            .AnyAsync();

        if (existingUser)
            throw new Exception("Username already exists");

        // Email kontrolü
        if (!string.IsNullOrEmpty(userDto.Email))
        {
            var existingEmail = await _unitOfWork.UserCredentials
                .GetWhere(uc => uc.Email == userDto.Email)
                .AnyAsync();

            if (existingEmail)
                throw new Exception("Email already exists");
        }

        // Person kontrolü
        var person = await _unitOfWork.Persons
            .GetByIdAsync(userDto.PersonId)
            ?? throw new Exception("Person not found");

        // Şifre hash'leme
        var (passwordHash, passwordSalt) = _authService.CreatePasswordHash(userDto.Password);

        // User oluşturma
        var user = new User
        {
            PersonId = userDto.PersonId,
            EmailNotificationsEnabled = true,
            PushNotificationsEnabled = true,
            SMSNotificationsEnabled = true,
            Credential = new UserCredential
            {
                Username = userDto.Username,
                Email = userDto.Email,
                PhoneNumber = userDto.PhoneNumber,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                IsActive = true,
                Roles = userDto.Roles
            }
        };

        await _unitOfWork.Users.AddAsync(user);
        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> UpdateUserAsync(Guid id, UpdateUserDto userDto)
    {
        var user = await _unitOfWork.Users
            .GetWhere(u => u.Id == id && !u.IsDeleted)
            .Include(u => u.Credential)
            .FirstOrDefaultAsync() 
            ?? throw new Exception("User not found");

        // Null değerleri güncelleme
        if (userDto.EmailNotificationsEnabled.HasValue)
            user.EmailNotificationsEnabled = userDto.EmailNotificationsEnabled.Value;
        
        if (userDto.PushNotificationsEnabled.HasValue)
            user.PushNotificationsEnabled = userDto.PushNotificationsEnabled.Value;
        
        if (userDto.SMSNotificationsEnabled.HasValue)
            user.SMSNotificationsEnabled = userDto.SMSNotificationsEnabled.Value;

        await _unitOfWork.Users.UpdateAsync(user);
        return _mapper.Map<UserDto>(user);
    }

    public async Task DeleteUserAsync(Guid id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id)
            ?? throw new Exception("User not found");

        await _unitOfWork.Users.SoftDeleteAsync(user);
    }

    public async Task<UserCredentialDto> UpdateCredentialsAsync(Guid userId, UpdateUserCredentialDto credentialDto)
    {
        var user = await _unitOfWork.Users
            .GetWhere(u => u.Id == userId && !u.IsDeleted)
            .Include(u => u.Credential)
            .FirstOrDefaultAsync() 
            ?? throw new Exception("User not found");

        var credential = user.Credential 
            ?? throw new Exception("User credentials not found");

        // Email kontrolü
        if (!string.IsNullOrEmpty(credentialDto.Email) && credentialDto.Email != credential.Email)
        {
            var existingEmail = await _unitOfWork.UserCredentials
                .GetWhere(uc => uc.Email == credentialDto.Email)
                .AnyAsync();

            if (existingEmail)
                throw new Exception("Email already exists");

            credential.Email = credentialDto.Email;
            credential.IsEmailVerified = false;
        }

        // Telefon kontrolü
        if (!string.IsNullOrEmpty(credentialDto.PhoneNumber) && credentialDto.PhoneNumber != credential.PhoneNumber)
        {
            var existingPhone = await _unitOfWork.UserCredentials
                .GetWhere(uc => uc.PhoneNumber == credentialDto.PhoneNumber)
                .AnyAsync();

            if (existingPhone)
                throw new Exception("Phone number already exists");

            credential.PhoneNumber = credentialDto.PhoneNumber;
            credential.IsPhoneVerified = false;
        }

        // Diğer alanları güncelle
        if (credentialDto.IsActive.HasValue)
            credential.IsActive = credentialDto.IsActive.Value;

        if (credentialDto.IsTwoFactorEnabled.HasValue)
            credential.IsTwoFactorEnabled = credentialDto.IsTwoFactorEnabled.Value;

        if (!string.IsNullOrEmpty(credentialDto.TwoFactorType))
            credential.TwoFactorType = credentialDto.TwoFactorType;

        if (credentialDto.Roles != null)
            credential.Roles = credentialDto.Roles;

        await _unitOfWork.UserCredentials.UpdateAsync(credential);
        return _mapper.Map<UserCredentialDto>(credential);
    }

    public async Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
    {
        var user = await _unitOfWork.Users
            .GetWhere(u => u.Id == userId && !u.IsDeleted)
            .Include(u => u.Credential)
            .FirstOrDefaultAsync() 
            ?? throw new Exception("User not found");

        var credential = user.Credential 
            ?? throw new Exception("User credentials not found");

        // Mevcut şifreyi kontrol et
        var (isValid, _) = _authService.VerifyPassword(currentPassword, credential.PasswordHash);
        if (!isValid)
            throw new Exception("Current password is incorrect");

        // Yeni şifreyi hash'le
        var (passwordHash, passwordSalt) = _authService.CreatePasswordHash(newPassword);

        credential.PasswordHash = passwordHash;
        credential.PasswordSalt = passwordSalt;
        credential.PasswordChangedAt = DateTime.UtcNow;
        credential.RequirePasswordChange = false;

        await _unitOfWork.UserCredentials.UpdateAsync(credential);
        return true;
    }

    public async Task<bool> ResetPasswordAsync(Guid userId, string newPassword)
    {
        var user = await _unitOfWork.Users
            .GetWhere(u => u.Id == userId && !u.IsDeleted)
            .Include(u => u.Credential)
            .FirstOrDefaultAsync() 
            ?? throw new Exception("User not found");

        var credential = user.Credential 
            ?? throw new Exception("User credentials not found");

        var (passwordHash, passwordSalt) = _authService.CreatePasswordHash(newPassword);

        credential.PasswordHash = passwordHash;
        credential.PasswordSalt = passwordSalt;
        credential.PasswordChangedAt = DateTime.UtcNow;
        credential.RequirePasswordChange = true;

        await _unitOfWork.UserCredentials.UpdateAsync(credential);
        return true;
    }

    public async Task<bool> VerifyEmailAsync(Guid userId, string token)
    {
        var user = await _unitOfWork.Users
            .GetWhere(u => u.Id == userId && !u.IsDeleted)
            .Include(u => u.Credential)
            .FirstOrDefaultAsync() 
            ?? throw new Exception("User not found");

        var credential = user.Credential 
            ?? throw new Exception("User credentials not found");

        if (credential.EmailVerificationToken != token)
            throw new Exception("Invalid verification token");

        if (credential.EmailVerificationTokenExpireDate < DateTime.UtcNow)
            throw new Exception("Verification token has expired");

        credential.IsEmailVerified = true;
        credential.EmailVerificationToken = null;
        credential.EmailVerificationTokenExpireDate = null;

        await _unitOfWork.UserCredentials.UpdateAsync(credential);
        return true;
    }

    public async Task<bool> VerifyPhoneAsync(Guid userId, string token)
    {
        var user = await _unitOfWork.Users
            .GetWhere(u => u.Id == userId && !u.IsDeleted)
            .Include(u => u.Credential)
            .FirstOrDefaultAsync() 
            ?? throw new Exception("User not found");

        var credential = user.Credential 
            ?? throw new Exception("User credentials not found");

        if (credential.PhoneVerificationToken != token)
            throw new Exception("Invalid verification token");

        if (credential.PhoneVerificationTokenExpireDate < DateTime.UtcNow)
            throw new Exception("Verification token has expired");

        credential.IsPhoneVerified = true;
        credential.PhoneVerificationToken = null;
        credential.PhoneVerificationTokenExpireDate = null;

        await _unitOfWork.UserCredentials.UpdateAsync(credential);
        return true;
    }

    public async Task<bool> ToggleTwoFactorAsync(Guid userId, bool enable, string? type = null)
    {
        var user = await _unitOfWork.Users
            .GetWhere(u => u.Id == userId && !u.IsDeleted)
            .Include(u => u.Credential)
            .FirstOrDefaultAsync() 
            ?? throw new Exception("User not found");

        var credential = user.Credential 
            ?? throw new Exception("User credentials not found");

        credential.IsTwoFactorEnabled = enable;
        if (enable && !string.IsNullOrEmpty(type))
        {
            credential.TwoFactorType = type;
            // İlgili doğrulama gereksinimlerini kontrol et
            if (type == "Email" && !credential.IsEmailVerified)
                throw new Exception("Email must be verified to use email 2FA");
            if (type == "SMS" && !credential.IsPhoneVerified)
                throw new Exception("Phone must be verified to use SMS 2FA");
        }
        else
        {
            credential.TwoFactorType = null;
            credential.TwoFactorKey = null;
        }

        await _unitOfWork.UserCredentials.UpdateAsync(credential);
        return true;
    }

    public async Task<bool> UpdateRolesAsync(Guid userId, IEnumerable<string> roles)
    {
        var user = await _unitOfWork.Users
            .GetWhere(u => u.Id == userId && !u.IsDeleted)
            .Include(u => u.Credential)
            .FirstOrDefaultAsync() 
            ?? throw new Exception("User not found");

        var credential = user.Credential 
            ?? throw new Exception("User credentials not found");

        credential.Roles = roles;
        await _unitOfWork.UserCredentials.UpdateAsync(credential);
        return true;
    }
} 