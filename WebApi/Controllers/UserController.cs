using Data.Entities;
using Data.Repositories;
using Domain.Dtos.UserDtos;
using Domain.Extentions;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]

public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly UserManager<UserEntity> _userManager;
    private readonly PostalCodeRepository _postalCodeRepository;
    private readonly IConfiguration _configuration;

    public UserController(IUserRepository userRepository,UserManager<UserEntity> userManager,PostalCodeRepository postalCodeRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _userManager = userManager;
        _postalCodeRepository = postalCodeRepository;
        _configuration = configuration;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] UserLoginFormData formData)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userManager.FindByEmailAsync(formData.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, formData.Password))
            return Unauthorized(new { Error = "Felaktiga inloggningsuppgifter" });


        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            
        };
        if (!string.IsNullOrEmpty(user.ClientId))
        {
            claims.Add(new Claim("client_id", user.ClientId));
        }

        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        //Här frågade jag faktiskt gpt för att få fram rätt kod för att skapa token, vilket jag tyckt varit svårt då det inte fungerade i frontend först när jag skrev det själv.
        //Så gpt korrigerade den så den sedan fungerade.
        //Jag har även lagt till en expiration för att token ska gå ut efter 7 dagar då frontend hela tiden slutade att fungera när jag testade.

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddDays(7);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            expiration = expires,
            role = roles
        });
    }


    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _userRepository.GetAsync(u => u.Id == userId);

        if (!result.Succeeded)
            return StatusCode(result.StatusCode ?? 500, new { error = result.Error });

        var user = result.Result!;

        var userProfile = new
        {
            user.Id,
            user.Email,
            Profile = user.Profile != null ? new
            {
                user.Profile.FirstName,
                user.Profile.LastName,
                user.Profile.PhoneNumber,
                user.Profile.Image
            } : null,
            Address = user.Address != null ? new
            {
                user.Address.StreetName,
                user.Address.StreetNumber,
                user.Address.PostalCode?.PostalCode,
                user.Address.PostalCode?.City,
                user.Address.PostalCode?.Country
            } : null
        };

        return Ok(userProfile);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] UserRegistrationFormData formData)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existingUser = await _userManager.FindByEmailAsync(formData.Email);
        if (existingUser != null)
            return Conflict(new { error = "A user with this email already exists" });

        var postalCodeExists = await _postalCodeRepository.ExistsAsync(pc => pc.PostalCode == formData.PostalCode);

        if (!postalCodeExists.Succeeded || postalCodeExists.StatusCode == 404)
        {
            var postalCodeEntity = new PostalCodeEntity
            {
                PostalCode = formData.PostalCode,
                City = formData.City,
                Country = formData.Country
            };

            var createPostalCodeResult = await _postalCodeRepository.AddAsync(postalCodeEntity);

            if (!createPostalCodeResult.Succeeded)
                return StatusCode(createPostalCodeResult.StatusCode ?? 500, new { error = createPostalCodeResult.Error });
        }

        var userEntity = new UserEntity
        {
            UserName = formData.Email,
            Email = formData.Email,
            Profile = new UserProfileEntity
            {
                FirstName = formData.FirstName,
                LastName = formData.LastName,
                PhoneNumber = formData.PhoneNumber
            },
            Address = new UserAddressEntity
            {
                StreetName = formData.StreetName,
                StreetNumber = formData.StreetNumber,
                PostalCodeId = formData.PostalCode
            }
        };

        var result = await _userRepository.AddAsync(userEntity, formData.Password);
        if (!result.Succeeded)
            return StatusCode(result.StatusCode ?? 500, new { error = result.Error });

        await _userManager.AddToRoleAsync(userEntity, "Member");
        return CreatedAtAction(nameof(GetProfile), new { id = userEntity.Id }, new { userEntity.Email });
    }

    [HttpPut("profile")]
    [Authorize]
    
    public async Task<IActionResult> UpdateProfile([FromBody] UserProfileUpdateFormData formData)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();
        var userResult = await _userRepository.GetAsync(u => u.Id == userId);

        if (!userResult.Succeeded)
            return StatusCode(userResult.StatusCode ?? 500, new { error = userResult.Error });

        var user = userResult.Result;

        if (user.Profile == null)
            user.Profile = new UserProfileModel();

        user.Profile.FirstName = formData.FirstName;
        user.Profile.LastName = formData.LastName;
        user.Profile.PhoneNumber = formData.PhoneNumber;

        if (!string.IsNullOrEmpty(formData.Image))
            user.Profile.Image = formData.Image;

        var updateResult = await _userRepository.UpdateAsync(user.MapTo<UserEntity>());

        if (!updateResult.Succeeded)
            return StatusCode(updateResult.StatusCode ?? 500, new { error = updateResult.Error });

        return NoContent();
    }

    [HttpPut("address")]
    [Authorize]
    
    public async Task<IActionResult> UpdateAddress([FromBody] UserAddressUpdateFormData formData)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var userResult = await _userRepository.GetAsync(u => u.Id == userId);

        if (!userResult.Succeeded)
            return StatusCode(userResult.StatusCode ?? 500, new { error = userResult.Error });

        var user = userResult.Result;
        var postalCodeExists = await _postalCodeRepository.ExistsAsync(pc => pc.PostalCode == formData.PostalCode);

        if (!postalCodeExists.Succeeded || postalCodeExists.StatusCode == 404)
        {
            var postalCodeEntity = new PostalCodeEntity
            {
                PostalCode = formData.PostalCode,
                City = formData.City,
                Country = formData.Country
            };

            var createPostalCodeResult = await _postalCodeRepository.AddAsync(postalCodeEntity);

            if (!createPostalCodeResult.Succeeded)
                return StatusCode(createPostalCodeResult.StatusCode ?? 500, new { error = createPostalCodeResult.Error });
        }

        if (user.Address == null)
            user.Address = new UserAddressModel();

        user.Address.StreetName = formData.StreetName;
        user.Address.StreetNumber = formData.StreetNumber;
        
        var updateResult = await _userRepository.UpdateAsync(user.MapTo<UserEntity>());

        if (!updateResult.Succeeded)
            return StatusCode(updateResult.StatusCode ?? 500, new { error = updateResult.Error });

        return NoContent();
    }

    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> DeleteAccount()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var userResult = await _userRepository.GetAsync(u => u.Id == userId);
        if (!userResult.Succeeded)
            return StatusCode(userResult.StatusCode ?? 500, new { error = userResult.Error });

        var user = userResult.Result;
        var deleteResult = await _userRepository.DeleteAsync(user.MapTo<UserEntity>());
        if (!deleteResult.Succeeded)
            return StatusCode(deleteResult.StatusCode ?? 500, new { error = deleteResult.Error });

        return NoContent();
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUserById(string id)
    {
        var result = await _userRepository.GetAsync(u => u.Id == id);

        if (!result.Succeeded)
            return StatusCode(result.StatusCode ?? 500, new { error = result.Error });

        var user = result.Result;

        var userProfile = new
        {
            user.Id,
            user.Email,
            Profile = user.Profile != null ? new
            {
                user.Profile.FirstName,
                user.Profile.LastName,
                user.Profile.PhoneNumber,
                user.Profile.Image
            } : null,
            Address = user.Address != null ? new
            {
                user.Address.StreetName,
                user.Address.StreetNumber,
                user.Address.PostalCode?.PostalCode,
                user.Address.PostalCode?.City,
                user.Address.PostalCode?.Country
            } : null
        };

        return Ok(userProfile);
    }


    [HttpGet]
    [Authorize(Roles = "Admin")]
    
    public async Task<IActionResult> GetAllUsers()
    {
        var result = await _userRepository.GetAllAsync(
            sortByExpression: u => u.Email
        );

        if (!result.Succeeded)
            return StatusCode(result.StatusCode ?? 500, new { error = result.Error });

        var users = result.Result.Select(user => new
        {
            user.Id,
            user.Email,
            Profile = user.Profile != null ? new
            {
                user.Profile.FirstName,
                user.Profile.LastName,
                user.Profile.PhoneNumber
            } : null
        });

        return Ok(users);
    }
}
