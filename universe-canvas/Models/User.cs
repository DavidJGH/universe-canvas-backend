#nullable enable
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace universe_canvas.Models;

public class User
{
	[BsonId]
	[BsonRepresentation(BsonType.ObjectId)]
	public string? Id { get; set; }
	public string Username { get; set; }
	public string? Email { get; set; }
	public string? HashedPassword { get; set; }
	
	public User(string username)
	{
		Username = username;
	}
	
	public User(string username, string email)
	{
		Username = username;
		Email = email;
	}
	
	public User(string username, string email, string hashedPassword)
	{
		Username = username;
		Email = email;
		HashedPassword = hashedPassword;
	}
}

public class UserRegisterDto
{
	public string Username { get; set; }
	public string Email { get; set; }
	public string Password { get; set; }
	
	public UserRegisterDto(string username, string email, string password)
	{
		Username = username;
		Email = email;
		Password = password;
	}
}

public class UserLoginDto
{
	public string Username { get; set; }
	public string Password { get; set; }
	
	public UserLoginDto(string username, string password)
	{
		Username = username;
		Password = password;
	}
}
