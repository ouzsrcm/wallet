using walletv2.Data.Entities.DataTransferObjects;
using walletv2.Data.Entities.Objects;
using walletv2.Data.Repositories;

namespace walletv2.Data.Services;

public interface IUserService
{
    Task<UserDto> Register(CreateUserDto param);
}

public class UserService
{
    private readonly IBaseRepository<User> _userRepository;
    private readonly IPasswordHasher _passwordHasher;

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

        return (await _userRepository.FindAsync(userInfo => userInfo.id == addedUser.id)).Select(x => new UserDto
        {
            Id = x.id,
            Email = x.Email,
            Username = x.Username,
            FirstName = x.FirstName,
            LastName = x.LastName
        }).FirstOrDefault() ?? throw new InvalidOperationException("User not found after creation.");
    }
}