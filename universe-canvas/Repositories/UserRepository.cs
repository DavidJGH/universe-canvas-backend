using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using universe_canvas.Models;
using universe_canvas.Utils;

namespace universe_canvas.Repositories;

public class UserRepository : IUserRepository
{

	private readonly IConfiguration _configuration;
    private readonly IMongoCollection<User> _usersCollection;
	private readonly IPasswordHasher<User> _passwordHasher;
	
	public UserRepository(IConfiguration configuration, IOptions<DatabaseSettings> databaseSettings, IPasswordHasher<User> passwordHasher)
	{
		_configuration = configuration;
        _usersCollection = MyMongoUtils.GetCollection<User>(databaseSettings,
            databaseSettings.Value.UsersCollectionName);
		_passwordHasher = passwordHasher;
	}

	public async Task<bool> Register(User user, string password)
	{
		var existingUser = await _usersCollection.Find(u => u.Username == user.Username || u.Email == user.Email).FirstOrDefaultAsync();
		if (existingUser != null)
		{
			return false;
		}
		var hashedPassword = _passwordHasher.HashPassword(user, password);
		user.HashedPassword = hashedPassword;
		await _usersCollection.InsertOneAsync(user);
		return true;
	}
	
	public async Task<Tokens> Authenticate(User user, string password)
	{
		var existingUser = await _usersCollection.Find(u => u.Username == user.Username).FirstOrDefaultAsync();
		if (existingUser == null)
		{
			return null;
		}

		if (_passwordHasher.VerifyHashedPassword(user, existingUser.HashedPassword, password) != PasswordVerificationResult.Success)
		{
			return null;
		}
		
		var tokenHandler = new JwtSecurityTokenHandler();
		var tokenKey = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]);
		var tokenDescriptor = new SecurityTokenDescriptor
		{
		  Subject = new ClaimsIdentity(new Claim[]
		  {
			 new(ClaimTypes.Name, user.Username)                    
		  }),
		   Expires = DateTime.UtcNow.AddMinutes(60),
		   SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey),SecurityAlgorithms.HmacSha256Signature)
		};
		var token = tokenHandler.CreateToken(tokenDescriptor);
		return new Tokens { Token = tokenHandler.WriteToken(token) };

	}
}