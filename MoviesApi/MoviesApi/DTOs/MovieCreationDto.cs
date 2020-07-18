using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoviesApi.Helpers;
using MoviesApi.Validations;
using System.Collections.Generic;

namespace MoviesApi.DTOs
{
    public class MovieCreationDto : MoviePatchDto
    {
        [FileSizeValidator(4)]
        [ContentTypeValidator(ContentTypeGroup.Image)]
        public IFormFile Poster { get; set; }
        [ModelBinder(BinderType = typeof(TypeBinder<List<int>>))]
        public List<int> GenresIds { get; set; }
        [ModelBinder(BinderType = typeof(TypeBinder<List<ActorDto>>))]
        public List<ActorDto> Actors { get; set; }
    }
}
