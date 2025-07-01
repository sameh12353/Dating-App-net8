using API.Entities;
using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers;


public class UsersController(DataContext context) : BaseApiController
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
    {
        var users = await context.Users.ToListAsync();

        return Ok(users);
    }
    
    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<AppUser>> GetUser(int id)
    {
        var user = await context.Users.FindAsync(id);

        if (user == null) return NotFound();

        return Ok(user);
    }
}
