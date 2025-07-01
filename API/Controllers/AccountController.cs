using API.Entities;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using API.Data;
using API.DTOs;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(DataContext Context, ITokenService tokenService) : BaseApiController
{
    [HttpPost("register")] // account/register
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (await UserExists(registerDto.Username))
        {
            return BadRequest("Username is taken");
        }

        using var hmac = new HMACSHA512();

        var user = new AppUser
        {
            UserName = registerDto.Username.ToLower(),
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            PasswordSalt = hmac.Key
        };

        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        return new UserDto
        {
            Username = user.UserName,
            Token = tokenService.CreateToken(user)
        };
    }

    [HttpPost("login")] // account/login
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await Context.Users.FirstOrDefaultAsync(x =>
            x.UserName == loginDto.Username.ToLower());
        
            

        if (user == null) return Unauthorized("Invalid username");
 
        using var hmac = new HMACSHA512(user.PasswordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

        for (int i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
        }

        return new UserDto
        {
            Username = user.UserName,
            Token = tokenService.CreateToken(user)
        };
    }

    private async Task<bool> UserExists(string username)
    {
        //return await Context.Users.AnyAsync(x => x.UserName == username); //Bob != bob
        return await Context.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower()); // case insensitive comparison
    }
}
