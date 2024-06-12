using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlbumController : ControllerBase
    {
        private readonly ApplicationDb _applicationDb;

        public AlbumController(ApplicationDb applicationDb)
        {
            _applicationDb = applicationDb;
        }

        [HttpGet("getAll")]
        public async Task<ActionResult<List<AlbumDto>>> GetAll()
        {
            var albums = await _applicationDb.Albums
                .Select(albumDto => new AlbumDto
                {
                    Id = albumDto.Id,
                    Name = albumDto.Name,
                    PhotoUrl = albumDto.PhotoUrl,
                    Artists = albumDto.Artists.Select(artistDto => new ArtistDto
                    {
                        Id = artistDto.Artist.Id,
                        Name = artistDto.Artist.Name,
                        PhotoUrl = artistDto.Artist.PhotoUrl,
                        Genre = artistDto.Artist.Genre.Name
                    }).ToList(),
                }).ToListAsync();

            return albums;
        }


        [HttpPost("create")]
        public async Task<IActionResult> create(AlbumAddEditDto model)
        {
            if (AlbumNameExitsAsync(model.Name).GetAwaiter().GetResult())
            {
                return BadRequest();
            }

            if (model.ArtistIds.Count == 0)
            {
                return BadRequest("At least on eartist Id should be selected.");
            }

            var albumToAdd = new Album
            {
                Name = model.Name,
                PhotoUrl = model.PhotoUrl,
            };

            _applicationDb.Albums.Add(albumToAdd);
            await _applicationDb.SaveChangesAsync();

            await AssignAritistsToAlbumAsync(albumToAdd.Id, model.ArtistIds);
            await _applicationDb.SaveChangesAsync();

            return NoContent();

        }

        private async Task<bool> AlbumNameExitsAsync(string albumName)
        {
            return await _applicationDb.Albums.AnyAsync(album => album.Name == albumName.ToLower());
        }

        private async Task AssignAritistsToAlbumAsync(int albumId, List<int> artistIds)
        {
            //remove any duplicate artistIds
            artistIds = artistIds.Distinct().ToList();

            foreach (var artistId in artistIds)
            {
                var artist = await _applicationDb.Artists.FindAsync(artistIds);

                if (artist != null)
                {
                    var artistAlbumBridgeToAdd = new ArtistAlbumBridge
                    {
                        AlbumId = albumId,
                        ArtistId = artistId,
                    };

                    _applicationDb.ArtistAlbumBridge.Add(artistAlbumBridgeToAdd);
                }
            }
        }
    }
}
