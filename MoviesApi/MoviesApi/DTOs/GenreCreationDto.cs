using MoviesApi.Validations;
using System.ComponentModel.DataAnnotations;

namespace MoviesApi.DTOs
{
    public class GenreCreationDto
    {
        [Required]
        [StringLength(40)]
        [FirtsLetterUppercase]
        public string Name { get; set; }
    }
}
