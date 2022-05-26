namespace RestApi.Controllers.Users;

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RestApi.Authorization;
using RestApi.Helpers.Users;

using RestApi.Models.Users;
using RestApi.Models.BlackListedTokens;

using RestApi.DataAccess.Users;
using RestApi.DataAccess.BlackListedTokens;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private IUserDataAccessProvider _userService;
    private IBlackListedTokenService _blackListedTokenService;
    private IMapper _mapper;
    private readonly AppSettings _appSettings;

    public UsersController(
        IUserDataAccessProvider userService,
        IBlackListedTokenService blackListedTokenService,
        IMapper mapper,
        IOptions<AppSettings> appSettings)
    {
        _userService = userService;
        _blackListedTokenService = blackListedTokenService;
        _mapper = mapper;
        _appSettings = appSettings.Value;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Authenticate(AuthenticateRequest model)
    {
        var response = _userService.Authenticate(model);
        return Ok(response);
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        var tokenToBlackList = new BlackListedToken();
        tokenToBlackList.Token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        _blackListedTokenService.InvalidateToken(tokenToBlackList);
        return Unauthorized(tokenToBlackList);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public IActionResult Register(RegisterRequest model)
    {
        _userService.Register(model);
        return Ok(new { message = "Registration successful" });
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var users = _userService.GetAll();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var user = _userService.GetById(id);
        return Ok(user);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, UpdateRequest model)
    {
        _userService.Update(id, model);
        return Ok(new { message = "User updated successfully" });
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _userService.Delete(id);
        return Ok(new { message = "User deleted successfully" });
    }
}