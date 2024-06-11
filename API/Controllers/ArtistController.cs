using API.Data;
using API.DTOs;
using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtistController : ControllerBase
    {
        private readonly ApplicationDb _applicationDb;

        public ArtistController(ApplicationDb applicationDb)
        {
            _applicationDb = applicationDb;
        }

        [HttpGet("get-all")]
        public ActionResult<List<ArtistDto>> GetAll()
        {
            var artists = _applicationDb.Artists
                .Include(artist => artist.Genre)
                .Select(artist => new ArtistDto
                {
                    Id = artist.Id,
                    Name = artist.Name,
                    PhotoUrl = artist.PhotoUrl,
                    Genre = artist.Genre.Name
                })
                .ToList();

            return artists;
        }

        [HttpGet("get-one/{id}")]
        public ActionResult<ArtistDto> GetOne(int id)
        {
            var artist = _applicationDb.Artists
                .Where(artist => artist.Id == id)
                .Select(artist => new ArtistDto
                {
                    Id = artist.Id,
                    Name = artist.Name,
                    PhotoUrl = artist.PhotoUrl,
                    Genre = artist.Genre.Name
                })
                .FirstOrDefault();

            if (artist == null)
            {
                return NotFound();
            }

            return artist;
        }

        [HttpPut("create")]
        public ActionResult Create(ArtistAddEditDto model)
        {
            if (ArtistNameExists(model.Name.ToLower()))
            {
                return BadRequest("Username already exists!");
            }

            var fetchedGenre = GetGenreByName(model.Genre.ToLower());

            if (fetchedGenre == null)
            {
                return BadRequest("Genre name does not exist");
            }

            var toAdd = new Artist
            {
                Id = model.Id,
                Name = model.Name.ToLower(),
                PhotoUrl = model.PhotoUrl,
                Genre = fetchedGenre
            };

            _applicationDb.Artists.Add(toAdd);
            _applicationDb.SaveChanges();

            return CreatedAtAction(nameof(GetOne), new { id = toAdd.Id }, toAdd);
        }

        [HttpPut("update")]
        public ActionResult Update(ArtistAddEditDto model)
        {
            var fetchedArtist = _applicationDb.Artists.FirstOrDefault(artist => artist.Id == model.Id);

            if (fetchedArtist == null)
            {
                return NotFound("Artist Id does not exist");
            }

            if (ArtistNameExists(model.Name.ToLower()))
            {
                return BadRequest("Artist Name Already exists!");
            }

            var fetchedGenre = GetGenreByName(model.Genre.ToLower());

            if (fetchedGenre == null)
            {
                return BadRequest("Genre does not exist");
            }

            var toUpdate = new Artist
            {
                Id= model.Id,
                Name = model.Name.ToLower(),
                PhotoUrl = model.PhotoUrl,
                Genre = fetchedGenre
            };

            _applicationDb.Artists.Add(toUpdate);
            _applicationDb.SaveChanges();

            return NoContent();
        }

        [HttpDelete("delete/{id}")]
        public ActionResult Delete(int id)
        {
            var fetchedArtist = _applicationDb.Artists.Find(id);

            if (fetchedArtist == null)
            {
                return NotFound("Artist Id not found!");
            }

            _applicationDb.Artists.Remove(fetchedArtist);
            _applicationDb.SaveChanges();

            return NoContent();
        }

        private bool ArtistNameExists(string name)
        {
            return _applicationDb.Artists.Any(artist => artist.Name.ToLower() == name.ToLower());
        }

        private Genre GetGenreByName(string name)
        {
            return _applicationDb.Genres.SingleOrDefault(genre => genre.Name.ToLower() == name.ToLower());
        }
    }
}
