using System.Collections.Generic;

namespace API.DTOs
{
    public class AlbumDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhotoUrl { get; set; }

        public List<ArtistDto> Artists { get; set; }
    }
}
