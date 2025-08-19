using Microsoft.AspNetCore.Mvc;

namespace TripFinder.API.Controllers;

[ApiController]
[Route("api/trips")]
public class TripsController : Controller
{
    [HttpGet( Name = "overtime")]
    public string Index()
    {
        return "Hello there";
    }
}