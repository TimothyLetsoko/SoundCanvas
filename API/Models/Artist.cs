using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class Artist
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        public string PhotoUrl { get; set; }

        [ForeignKey("Genre")]
        public int GenreId { get; set; }
        public Genre Genre { get; set; }
        public ICollection<ArtistAlbumBridge> Albums { get; set; }
    }
}
