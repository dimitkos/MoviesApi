using Microsoft.AspNetCore.Http;
using MoviesApi.Validations;
using System;
using System.ComponentModel.DataAnnotations;

namespace MoviesApi.DTOs
{
    public class PersonCreationDto
    {
        [Required]
        [StringLength(40)]
        public string Name { get; set; }

        public string Biography { get; set; }

        public DateTime DateOfBirth { get; set; }

        [FileSizeValidator(4)]
        [ContentTypeValidator(ContentTypeGroup.Image)]
        public IFormFile Picture { get; set; }
    }
}
