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
        if (request is null)
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
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAll());
    }

    [HttpGet("average")]
    public async Task<IActionResult> GetAverage()
    {
        return Ok(await _service.GetAverageRating());
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteMovie(id);
        return Ok();
    }

    [HttpPut("{id}/rating")]
    public async Task<IActionResult> UpdateRating(int id, [FromBody] RatingUpdate request)
    {
        await _service.UpdateRating(id, request.Rating);
        return Ok();
    }
}