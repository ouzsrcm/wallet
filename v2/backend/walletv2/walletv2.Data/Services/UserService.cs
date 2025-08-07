using walletv2.Data.Entities.DataTransferObjects;
using walletv2.Data.Entities.Objects;
using walletv2.Data.Repositories;

namespace walletv2.Data.Services;

public interface IUserService
{
    Task<UserDto> Register(CreateUserDto param);
    Task<UserDto> ValidateUser(UserLoginDto param);
    Task<UserDetailedInfoDto> GetUserDetails(Guid userId);
}

public class UserService : IUserService
{
    private readonly IBaseRepository<User> _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    /// <summary>
    /// user service constructor.
    /// </summary>
    /// <param name="_userRepository"></param>
    /// <param name="passwordHasher"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public UserService(IBaseRepository<User> _userRepository, IPasswordHasher passwordHasher)
    {
        this._userRepository = _userRepository ?? throw new ArgumentNullException(nameof(_userRepository));
        _passwordHasher = passwordHasher;
    }

    /// <summary>
    /// register a new user with the provided details.
    /// </summary>
    /// <param name="param">CreateUserDto</param>
    /// <returns>UserDto</returns>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task<UserDto> Register(CreateUserDto param)
    {
        var exists = _userRepository.AnyAsync(x => x.Email == param.Email || x.Username == param.Username);
        if (await exists)
            throw new InvalidOperationException("User with this email or username already exists.");

        var (salt, hash) = _passwordHasher.HashPassword(param.Password ?? throw new ArgumentNullException(nameof(param.Password)));
        var addedUser = await _userRepository.AddAsync(new User
        {
            Email = param.Email,
            Username = param.Username,
            PasswordSalt = salt,
            PasswordHash = hash
        });
        await _userRepository.SaveChangesAsync();

        return (await _userRepository.FindAsync(userInfo => userInfo.Id == addedUser.Id)).Select(x => new UserDto
        {
            Id = x.Id,
            Email = x.Email,
            Username = x.Username,
            FirstName = x.FirstName,
            LastName = x.LastName
        }).FirstOrDefault() ?? throw new InvalidOperationException("User not found after creation.");
    }

    /// <summary>
    /// validate user credentials and return user details if valid.
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<UserDto> ValidateUser(UserLoginDto param)
    {
        if (string.IsNullOrEmpty(param.Password))
            throw new ArgumentNullException("Username and Password cannot be null or empty.");

        var user = await _userRepository.FindAsync(x => x.Username == param.Username);
        if (!user.Any())
            throw new InvalidOperationException("User not found.");

        var foundUser = user.FirstOrDefault();
        if (!_passwordHasher.VerifyPassword(param.Password, foundUser.PasswordSalt, foundUser.PasswordHash))
            throw new InvalidOperationException("Invalid password.");

        return new UserDto
        {
            Id = foundUser.Id,
            Email = foundUser.Email,
            Username = foundUser.Username,
            FirstName = foundUser.FirstName,
            LastName = foundUser.LastName
        };
    }

    /// <summary>
    /// get detailed user information by user ID.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<UserDetailedInfoDto> GetUserDetails(Guid userId)
    {
        var user = await _userRepository.FindAsync(x => x.Id == userId);

        if (!user.Any())
            throw new InvalidOperationException("User not found.");

        var foundUser = user.First();
        if (foundUser.isDeleted)
            throw new InvalidOperationException("User account is deleted.");

        return new UserDetailedInfoDto
        {
            Id = foundUser.Id,
            Email = foundUser.Email,
            Username = foundUser.Username,
            Fullname = $"{foundUser.FirstName} {foundUser.LastName}",
            FirstName = foundUser.FirstName,
            LastName = foundUser.LastName,
            Address = foundUser.Address,
            City = foundUser.City,
            PhoneNumber = foundUser.PhoneNumber ?? string.Empty,
            ProfilePictureUrl = foundUser.ProfilePictureUrl,
            Bio = foundUser.Bio,
            DateOfBirth = foundUser.DateOfBirth,
            isEmailVerified = foundUser.IsEmailVerified,
            isPhoneNumberVerified = foundUser.IsPhoneNumberVerified,
            LastLogin = foundUser.LastLogin
        };
    }

}