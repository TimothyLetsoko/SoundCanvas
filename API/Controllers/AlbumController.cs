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

        [HttpGet("get-all")]
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

        [HttpGet("get-one")]
        public async Task<ActionResult<AlbumDto>> GetOne(int id)
        {
            var album = await _applicationDb.Albums
                .Where(_ => _.Id == id)
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
                }).FirstOrDefaultAsync();

            if (album == null)
            {
                return NotFound();
            }

            return album;
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

        [HttpPut("update")]
        public async Task<IActionResult> Update(AlbumAddEditDto model)
        {
            var fetchedAlbum = await _applicationDb.Albums
                .Include(x => x.Artists)
                .FirstOrDefaultAsync(x => x.Id == model.Id);

            if (fetchedAlbum == null)
            {
                return NotFound();
            }

            if (fetchedAlbum.Name != model.Name.ToLower() && await AlbumNameExitsAsync(model.Name))
            {
                return BadRequest("Album name should be unique");
            }

            //clear all existing Artists
            foreach (var artist in fetchedAlbum.Artists)
            {
                var fetchedArtistAlbumBridge = await _applicationDb.ArtistAlbumBridge
                    .SingleOrDefaultAsync(x => x.ArtistId == artist.ArtistId && x.AlbumId == fetchedAlbum.Id);

                _applicationDb.ArtistAlbumBridge.Remove(fetchedArtistAlbumBridge);
            }

            await _applicationDb.SaveChangesAsync();

            fetchedAlbum.Name = model.Name.ToLower();
            fetchedAlbum.PhotoUrl = model.PhotoUrl;

            await AssignAritistsToAlbumAsync(fetchedAlbum.Id, model.ArtistIds);

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
