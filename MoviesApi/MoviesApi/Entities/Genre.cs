using MoviesApi.Validations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MoviesApi.Entities
{
    public class Genre
    {
        public int Id { get; set; }
        [Required]
        [StringLength(40)]
        [FirtsLetterUppercase]
        public string Name { get; set; }
        public List<MoviesGenres> MoviesGenres { get; set; }
    }
}
