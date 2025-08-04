using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

public interface IPasswordHasher
{
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

        var outputBytes = new byte[SaltSize + KeySize];
        Buffer.BlockCopy(salt, 0, outputBytes, 0, SaltSize);
        Buffer.BlockCopy(key, 0, outputBytes, SaltSize, KeySize);

        return (
            Convert.ToBase64String(salt),
            Convert.ToBase64String(outputBytes)
            );
    }

    public bool VerifyPassword(string password, string saltBase64, string hashBase64)
    {
        var salt = Convert.FromBase64String(saltBase64);

        var keyToCheck = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: Iterations,
            numBytesRequested: KeySize);

        return CryptographicOperations.FixedTimeEquals(
            Convert.FromBase64String(hashBase64),
            keyToCheck
        );
    }
}
