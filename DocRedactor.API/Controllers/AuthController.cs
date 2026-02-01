using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using DocRedactor.API.DTOs;
using DocRedactor.API.Interfaces;
using DocRedactor.API.Models;

namespace DocRedactor.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILogger<AuthController> _logger;
    
    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtTokenService jwtTokenService,
        ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }
    
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto registerDto)
    {
        try
        {
            var user = new ApplicationUser
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email
            };
            
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            
            if (!result.Succeeded)
            {
                return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
            }
            
            _logger.LogInformation("User {Email} registered successfully", registerDto.Email);
            
            var token = _jwtTokenService.GenerateToken(user);
            
            return Ok(new AuthResponseDto
            {
                Token = token,
                Email = user.Email!,
                UserName = user.UserName!
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user registration");
            return StatusCode(500, "An error occurred during registration");
        }
    }
    
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }
            
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            
            if (!result.Succeeded)
            {
                _logger.LogWarning("Failed login attempt for {Email}", loginDto.Email);
                return Unauthorized(new { message = "Invalid email or password" });
            }
            
            _logger.LogInformation("User {Email} logged in successfully", loginDto.Email);
            
            var token = _jwtTokenService.GenerateToken(user);
            
            return Ok(new AuthResponseDto
            {
                Token = token,
                Email = user.Email!,
                UserName = user.UserName!
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user login");
            return StatusCode(500, "An error occurred during login");
        }
    }
}
