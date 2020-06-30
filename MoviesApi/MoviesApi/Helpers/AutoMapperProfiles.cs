using AutoMapper;
using MoviesApi.DTOs;
using MoviesApi.Entities;

namespace MoviesApi.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Genre, GenreDto>().ReverseMap();
            CreateMap<GenreCreationDto, Genre>();
            CreateMap<Person, PersonDto>().ReverseMap();
            CreateMap<PersonCreationDto, Person>();
        }
    }
}
