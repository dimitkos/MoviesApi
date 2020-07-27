using AutoMapper;
using Microsoft.AspNetCore.Identity;
using MoviesApi.DTOs;
using MoviesApi.Entities;
using System.Collections.Generic;

namespace MoviesApi.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Genre, GenreDto>().ReverseMap();
            CreateMap<GenreCreationDto, Genre>();

            CreateMap<Person, PersonDto>().ReverseMap();
            CreateMap<PersonCreationDto, Person>()
                .ForMember(x => x.Picture, options => options.Ignore());
            CreateMap<Person, PersonPatchDto>().ReverseMap();

            CreateMap<Movie, MovieDto>().ReverseMap();
            CreateMap<MovieCreationDto, Movie>()
                .ForMember(x => x.Poster, options => options.Ignore())
                .ForMember(x => x.MoviesGenres, options => options.MapFrom(MapMoviesGenres))
                .ForMember(x => x.MoviesActors, options => options.MapFrom(MapMoviesActors));
            CreateMap<Movie, MoviePatchDto>().ReverseMap();

            CreateMap<Movie, MovieDetailsDto>()
                .ForMember(x => x.Genres, options => options.MapFrom(MapMoviesGenres))
                .ForMember(x => x.Actors, options => options.MapFrom(MapMoviesActors));

            CreateMap<IdentityUser, UserDto>()
                .ForMember(x => x.EmailAdress, options => options.MapFrom(x => x.Email))
                .ForMember(x => x.UserId, options => options.MapFrom(x => x.Id));
        }

        private List<GenreDto> MapMoviesGenres(Movie movie, MovieDetailsDto movieDetailsDTO)
        {
            var result = new List<GenreDto>();
            foreach (var moviegenre in movie.MoviesGenres)
            {
                result.Add(new GenreDto() { Id = moviegenre.GenreId, Name = moviegenre.Genre.Name });
            }
            return result;
        }

        private List<ActorDto> MapMoviesActors(Movie movie, MovieDetailsDto movieDetailsDTO)
        {
            var result = new List<ActorDto>();
            foreach (var actor in movie.MoviesActors)
            {
                result.Add(new ActorDto() { PersonId = actor.PersonId, Character = actor.Character, PersonName = actor.Person.Name });
            }
            return result;
        }

        private List<MoviesGenres> MapMoviesGenres(MovieCreationDto movieCreationDto, Movie movie)
        {
            var moviesGenres = new List<MoviesGenres>();

            foreach (var id in movieCreationDto.GenresIds)
            {
                moviesGenres.Add(new MoviesGenres { GenreId = id });
            }

            return moviesGenres;
        }

        private List<MoviesActors> MapMoviesActors(MovieCreationDto movieCreationDto, Movie movie)
        {
            var moviesActors = new List<MoviesActors>();

            foreach (var actor in movieCreationDto.Actors)
            {
                moviesActors.Add(new MoviesActors { PersonId = actor.PersonId, Character = actor.Character });
            }

            return moviesActors;
        }
    }
}
