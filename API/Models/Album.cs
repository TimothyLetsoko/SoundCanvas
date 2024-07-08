using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Album
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string PhotoUrl { get; set; }
        public ICollection<ArtistAlbumBridge> Artists { get; set; }
    }
}
