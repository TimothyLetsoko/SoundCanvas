using API.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            return Ok(_applicationDb.Genres.ToList());
        }

        [HttpGet("get-one/{id}")]
        public IActionResult GetOne(int id)
        {
            var genre = _applicationDb.Genres.FirstOrDefault(x => x.Id == id);

            if (genre == null) 
            {
                return NotFound();
            }

            return Ok(genre);
        }
    }
}
