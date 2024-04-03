using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace MVCAndWebAPIAuthAndAuthTest.AuthLibrary;

public class CustomTokenProvider<TUser> : DataProtectorTokenProvider<TUser> where TUser : class
{
    public CustomTokenProvider(IDataProtectionProvider dataProtectionProvider, IOptions<DataProtectionTokenProviderOptions> options, ILogger<DataProtectorTokenProvider<TUser>> logger) : base(dataProtectionProvider, options, logger)
    {
    }

    public override Task<string> GenerateAsync(string purpose, UserManager<TUser> manager, TUser user)
    {
        return Task.FromResult(GenerateToken());
    }

    private string GenerateToken()
    {
        var token = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(token);
        }
        return Convert.ToBase64String(token).Replace('+', '-').Replace('/', '_').Replace("=", "a").Replace(")","b").Replace("(","c)");
    }
}
