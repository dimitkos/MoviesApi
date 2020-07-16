using Microsoft.AspNetCore.Http;
using MoviesApi.Validations;

namespace MoviesApi.DTOs
{
    public class MovieCreationDto : MoviePatchDto
    {
        [FileSizeValidator(4)]
        [ContentTypeValidator(ContentTypeGroup.Image)]
        public IFormFile Poster { get; set; }
    }
}
