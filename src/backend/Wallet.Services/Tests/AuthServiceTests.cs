using System.Security.Cryptography;
using Xunit;
using Wallet.Services.Concrete;
using Wallet.Services.UnitOfWorkBase.Abstract;
using Moq;
using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;

namespace Wallet.Services.Tests;

[Collection("Sequential")]  // Testleri sıralı çalıştır
public class AuthServiceTests
{
    private readonly ITestOutputHelper _output;
    private readonly AuthService _authService;

    public AuthServiceTests(ITestOutputHelper output)
    {
        _output = output;
        // Mock dependencies
        var mockUnitOfWork = new Mock<IPersonUnitOfWork>();
        var mockConfiguration = new Mock<IConfiguration>();
        _authService = new AuthService(mockUnitOfWork.Object, mockConfiguration.Object);
    }

    [Fact]
    public void CreatePasswordHash_ShouldCreateSameHashForSamePassword()
    {
        // Arrange
        var password = "MySecureP@ss2024";

        // Act
        var (hash1, salt1) = _authService.CreatePasswordHash(password);
        var (hash2, salt2) = _authService.CreatePasswordHash(password);
        var (hash3, salt3) = _authService.CreatePasswordHash(password);

        // Assert
        Assert.Equal(hash1, hash2); // Aynı şifre için aynı hash
        Assert.Equal(hash1, hash3); // Aynı şifre için aynı hash
        Assert.Equal(salt1, salt2); // Aynı şifre için aynı salt
        Assert.Equal(salt1, salt3); // Aynı şifre için aynı salt

        _output.WriteLine($"""
            Password: {password}
            Hash: {hash1}
            Salt: {salt1}
            ----------------------------------------
            """);
    }

    [Theory]
    [InlineData("Password123!", "StrongP@ssw0rd")]
    [InlineData("MyP@ssw0rd123", "C0mplexP@ss")]
    public void CreatePasswordHash_ShouldCreateDifferentHashesForDifferentPasswords(string password1, string password2)
    {
        // Act
        var (hash1, salt1) = _authService.CreatePasswordHash(password1);
        var (hash2, salt2) = _authService.CreatePasswordHash(password2);

        // Assert
        Assert.NotEqual(hash1, hash2); // Farklı şifreler için farklı hash'ler
        Assert.NotEqual(salt1, salt2); // Farklı şifreler için farklı salt'lar

        _output.WriteLine($"""
            Password1: {password1}
            Hash1: {hash1}
            Salt1: {salt1}
            
            Password2: {password2}
            Hash2: {hash2}
            Salt2: {salt2}
            ----------------------------------------
            """);
    }

    [Fact]
    public void VerifyPassword_ShouldVerifyCorrectPassword()
    {
        // Arrange
        var password = "MySecureP@ss2024";
        var (hash, salt) = _authService.CreatePasswordHash(password);

        // Act
        var (isValid, computedHash) = _authService.VerifyPassword(password, hash);
        var (isInvalid, computedHash2) = _authService.VerifyPassword("WrongPassword", hash);

        // Assert
        Assert.True(isValid);
        Assert.False(isInvalid);
    }

    [Fact]
    public void GeneratePasswordExamples()
    {
        var passwords = new[] { "MySecureP@ss2024" };

        foreach (var password in passwords)
        {
            var (hash, salt) = _authService.CreatePasswordHash(password);
            _output.WriteLine($"""
                Password: {password}
                Hash: {hash}
                Salt: {salt}
                ----------------------------------------
                """);
        }
    }
} 