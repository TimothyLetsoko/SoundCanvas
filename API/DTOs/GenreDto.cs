using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class GenreDto
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
