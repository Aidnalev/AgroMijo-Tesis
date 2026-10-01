using AgroMijo.API.Data;
using Microsoft.AspNetCore.Mvc;

namespace AgroMijo.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MongoTestController : ControllerBase
{
    private readonly MongoDbService _mongoDb;

    public MongoTestController(MongoDbService mongoDb)
    {
        _mongoDb = mongoDb;
    }

    [HttpGet]
    public IActionResult TestConnection()
    {
        try
        {
            _mongoDb.Database.RunCommand<MongoDB.Bson.BsonDocument>(
                new MongoDB.Bson.BsonDocument("ping", 1)
            );

            return Ok(new
            {
                conectado = true,
                mensaje = "Conexión con MongoDB exitosa."
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                conectado = false,
                mensaje = "No se pudo conectar con MongoDB.",
                error = ex.Message
            });
        }
    }
}