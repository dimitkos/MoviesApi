using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviesApi.Controllers.V2;
using MoviesApi.DTOs;
using MoviesApi.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoviesApi.Tests.UnitTests
{
    [TestClass]
    public class GenresControllerTests : BaseTests
    {
        [TestMethod]
        public async Task GetAllGenres()
        {
            //Preparation
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();

            context.Genres.Add(new Genre() { Name = "Genre1" });
            context.Genres.Add(new Genre() { Name = "Genre2" });
            context.SaveChanges();

            //will use another context in the same db(and data) because this new context it is dont have on the memorh the 2 records
            var context2 = BuildContext(databaseName);

            //Testing
            var controller = new GenresV2Controller(context2, mapper);

            var response = await controller.Get();
            var result = response.Result as OkObjectResult;

            var genres = result.Value as List<GenreDto>;

            //Verification
            Assert.AreEqual(2, genres.Count);

            //if use as return type Task<ActionResult<List<GenreDTO>>>

            //// Testing
            //var controller = new GenresController(context2, mapper);
            //var response = await controller.Get();

            //// Verification
            //var genres = response.Value;
            //Assert.AreEqual(2, genres.Count);
        }

        [TestMethod]
        public async Task GetGenreByIdDoesNotExist()
        {
            //Preparation
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();

            //Testing
            var controller = new GenresV2Controller(context, mapper);
            ActionResult<GenreDto> response = await controller.Get(1);

            var result = response.Result as NotFoundResult;
            var statusCode = result.StatusCode;

            Assert.AreEqual(404, statusCode);

            //if use as return type Task<ActionResult<GenreDTO>>
            //var response = await controller.Get(1);
            //var result = response.Result as StatusCodeResult;
            //Assert.AreEqual(404, result.StatusCode);
        }

        [TestMethod]
        public async Task GetGenreByExistingId()
        {
            //Preparation
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();

            context.Genres.Add(new Genre() { Name = "Genre1" });
            context.Genres.Add(new Genre() { Name = "Genre2" });
            context.SaveChanges();

            //will use another context in the same db(and data) because this new context it is dont have on the memorh the 2 records
            var context2 = BuildContext(databaseName);

            //Testing
            var controller = new GenresV2Controller(context2, mapper);

            var Id = 1;
            var response = await controller.Get(Id);
            var result = response.Result as OkObjectResult;
            var genre = result.Value as GenreDto;

            Assert.AreEqual(Id, genre.Id);
        }

        [TestMethod]
        public async Task CreateGenre()
        {
            //Preparation
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();

            var newGenre = new GenreCreationDto { Name = "New Genre" };

            //Testing
            var controller = new GenresV2Controller(context, mapper);
            var response = await controller.Post(newGenre);
            var result = response as CreatedAtRouteResult;
            Assert.AreEqual(201, result.StatusCode);

            var context2 = BuildContext(databaseName);
            var count = await context2.Genres.CountAsync();
            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public async Task UpdateGenre()
        {
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();

            context.Genres.Add(new Genre { Name = "New Genre1" });
            context.SaveChanges();

            var context2 = BuildContext(databaseName);
            var controller = new GenresV2Controller(context2, mapper);
            var genreCreationDTO = new GenreCreationDto() { Name = "New name" };

            var id = 1;
            var response = await controller.Put(id, genreCreationDTO);
            var result = response as StatusCodeResult;
            Assert.AreEqual(204, result.StatusCode);

            var context3 = BuildContext(databaseName);
            var exists = await context3.Genres.AnyAsync(x => x.Name == "New name");
            Assert.IsTrue(exists);
        }

        [TestMethod]
        public async Task DeleteGenreNotFound()
        {
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();

            var controller = new GenresV2Controller(context, mapper);

            var response = await controller.Delete(1);
            var result = response as StatusCodeResult;
            Assert.AreEqual(404, result.StatusCode);
        }

        [TestMethod]
        public async Task DeleteGenre()
        {
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();

            context.Genres.Add(new Genre() { Name = "Genre 1" });
            context.SaveChanges();

            var context2 = BuildContext(databaseName);

            var controller = new GenresV2Controller(context2, mapper);

            var response = await controller.Delete(1);
            var result = response as StatusCodeResult;
            Assert.AreEqual(204, result.StatusCode);

            var context3 = BuildContext(databaseName);
            var exists = await context3.Genres.AnyAsync();
            Assert.IsFalse(exists);

        }
    }
}
