using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

public interface IPasswordHasher
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="password"></param>
    /// <returns>1. : salt 2. : hash</returns>
    (string, string) HashPassword(string password);
    bool VerifyPassword(string password, string saltBase64, string hashBase64);
}

public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16; // 128 bit
    private const int KeySize = 32;  // 256 bit
    private const int Iterations = 100_000;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="password"></param>
    /// <returns>salt: string, hash: string</returns>
    public (string, string) HashPassword(string password)
    {
        using var rng = RandomNumberGenerator.Create();
        var salt = new byte[SaltSize];
        rng.GetBytes(salt);

        var key = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: Iterations,
            numBytesRequested: KeySize);

        return (
            Convert.ToBase64String(salt),
            Convert.ToBase64String(key)
        );
    }

    public bool VerifyPassword(string password, string saltBase64, string hashBase64)
    {
        var salt = Convert.FromBase64String(saltBase64);
        var expectedHash = Convert.FromBase64String(hashBase64);

        var keyToCheck = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: Iterations,
            numBytesRequested: KeySize);

        return CryptographicOperations.FixedTimeEquals(expectedHash, keyToCheck);
    }

}
