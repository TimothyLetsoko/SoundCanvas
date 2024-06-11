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
    }
}
