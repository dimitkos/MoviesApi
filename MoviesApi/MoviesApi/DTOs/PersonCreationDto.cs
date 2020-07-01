using Microsoft.AspNetCore.Http;
using MoviesApi.Validations;

namespace MoviesApi.DTOs
{
    public class PersonCreationDto : PersonPatchDto
    {
        [FileSizeValidator(4)]
        [ContentTypeValidator(ContentTypeGroup.Image)]
        public IFormFile Picture { get; set; }
    }
}
