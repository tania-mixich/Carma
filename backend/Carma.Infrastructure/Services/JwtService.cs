using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Carma.Application.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Carma.Infrastructure.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public string GenerateToken(Guid userId, string email, string username)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
        {
            new (ClaimTypes.NameIdentifier, userId.ToString()),
            new (ClaimTypes.Email, email),
            new (ClaimTypes.Name, username)
        };
        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            null,
            DateTime.Now.AddMinutes(int.Parse(_configuration["Jwt:ExpirationInMinutes"]!)),
            credentials
            );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}