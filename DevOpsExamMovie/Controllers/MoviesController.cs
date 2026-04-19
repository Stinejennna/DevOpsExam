namespace DevOpsExamMovie.Controllers;

using DevOpsExamMovie.Models;
using DevOpsExamMovie.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/movies")]
public class MoviesController : ControllerBase
{
    private readonly MovieService _service;

    public MoviesController(MovieService service)
    {
        _service = service;
    }

    [HttpPost]
    public IActionResult Add([FromBody] Movie movie)
    {
        if (movie is null)
            return BadRequest("Movie cannot be null");

        _service.AddMovie(movie);
        return Ok();
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_service.GetAll());
    }

    [HttpGet("average")]
    public IActionResult GetAverage()
    {
        return Ok(_service.GetAverageRating());
    }
}