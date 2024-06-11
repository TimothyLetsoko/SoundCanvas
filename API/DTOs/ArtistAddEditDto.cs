using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class ArtistAddEditDto
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Artist name should not exceed {0} of characters")]
        public string Name { get; set; }
        public string PhotoUrl { get; set; }
        [Required]
        public string Genre { get; set; }
    }
}
