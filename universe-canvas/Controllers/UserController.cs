using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using universe_canvas.Models;
using universe_canvas.Repositories;

namespace universe_canvas.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
	private readonly IUserRepository _userRepository;

	public UserController(IUserRepository userRepository)
	{
		_userRepository = userRepository;
	}
	
	[HttpPost]
	[Route("addUser")]
	public async Task<IActionResult> AddUser(UserRegisterDto userRegisterDto)
	{
		var success = await _userRepository.Register(new User(userRegisterDto.Username, userRegisterDto.Email), userRegisterDto.Password);

		if (!success)
		{
			return BadRequest();
		}

		return Ok();
	}
	
	[AllowAnonymous]
	[HttpPost]
	[Route("authenticate")]
	public async Task<IActionResult> Authenticate(UserLoginDto userLoginDto)
	{
		var token = await _userRepository.Authenticate(new User(userLoginDto.Username), userLoginDto.Password);

		if (token == null)
		{
			return Unauthorized();
		}

		return Ok(token);
	}
}