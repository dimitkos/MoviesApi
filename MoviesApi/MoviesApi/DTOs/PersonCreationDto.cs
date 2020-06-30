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
    }
}
