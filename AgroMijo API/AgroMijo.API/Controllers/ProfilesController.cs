using AgroMijo.API.Data;
using AgroMijo.API.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace AgroMijo.API.Controllers;

[ApiController]
[Route("api/profiles")]
public class ProfilesController : ControllerBase
{
    private readonly IMongoCollection<PlayerProfile> _profiles;

    public ProfilesController(MongoDbService mongoDb)
    {
        _profiles = mongoDb.Database.GetCollection<PlayerProfile>("profiles");
    }

    [HttpPost]
    public async Task<IActionResult> CreateProfile(PlayerProfile profile)
    {
        var existingProfile = await _profiles
            .Find(p => p.Id == profile.Id)
            .FirstOrDefaultAsync();

        if (existingProfile != null)
        {
            return Conflict(new
            {
                mensaje = "El perfil ya existe."
            });
        }

        await _profiles.InsertOneAsync(profile);

        return Ok(profile);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProfile(string id)
    {
        var profile = await _profiles
            .Find(p => p.Id == id)
            .FirstOrDefaultAsync();

        if (profile == null)
        {
            return NotFound(new
            {
                mensaje = "El perfil no existe."
            });
        }

        return Ok(profile);
    }
}