namespace DevOpsExamMovie.Controllers;

using DevOpsExamMovie.Models;
using DevOpsExamMovie.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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
    public async Task<IActionResult> Add([FromBody] CreateMovieRequest? request)
    {
        if (request == null)
            return BadRequest("Movie cannot be null");

        var movie = new Movie
        {
            Title = request.Title,
            Rating = request.Rating,
            PosterUrl = request.PosterUrl,
            ReleaseYear = request.ReleaseYear,
            Genre = request.Genre
        };

        await _service.AddMovie(movie);

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
    
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _service.DeleteMovie(id);
        return Ok();
    }
    
    [HttpPut("{id}/rating")]
    public IActionResult UpdateRating(int id, [FromBody] RatingUpdate request)
    {
        if (!request.Rating.HasValue)
            return BadRequest("Rating is required.");

        _service.UpdateRating(id, request.Rating.Value);
        return Ok();
    }
}