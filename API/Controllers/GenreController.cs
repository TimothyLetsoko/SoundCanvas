using API.Data;
using API.DTOs;
using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly ApplicationDb _applicationDb;

        public GenreController(ApplicationDb applicationDb)
        {
            _applicationDb = applicationDb;
        }

        [HttpGet("get-all")]
        public IActionResult GetAll()
        {
            var genres = _applicationDb.Genres.ToList();
            var toReturn = new List<GenreDto>();

            foreach (var genre in genres)
            {
                var genreDto = new GenreDto
                {
                    Id = genre.Id,
                    Name = genre.Name,
                };

                toReturn.Add(genreDto);
            }

            return Ok(toReturn);
        }

        [HttpGet("get-one/{id}")]
        public IActionResult GetOne(int id)
        {
            var genre = _applicationDb.Genres.FirstOrDefault(x => x.Id == id);

            if (genre == null) 
            {
                return NotFound();
            }

            var toReturn = new GenreDto
            {
                Id = genre.Id,
                Name = genre.Name,
            };

            return Ok(toReturn);
        }

        [HttpPost("create")]
        public IActionResult Create(GenreAddEditDto model)
        {
            if (GenreNameExists(model.Name))
            {
                return BadRequest("Genre name already exists!");
            }

            var genreToAdd = new Genre
            {
                Name = model.Name.ToLower(),
            };

            _applicationDb.Genres.Add(genreToAdd);
            _applicationDb.SaveChanges();

            return Created(nameof(GetOne), model.Name);
        }

        private bool GenreNameExists(string name)
        {
            var fetchedGenre = _applicationDb.Genres.FirstOrDefault(genre => genre.Name.ToLower().Equals(name.ToLower()));

            if (fetchedGenre == null)
            {
                return false;
            }

            return true;
        }
    }
}
