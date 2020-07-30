using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Helpers;

namespace MoviesApi.Tests
{
    public class BaseTests
    {
        protected ApplicationDbContext BuildContext(string databaseName)
        {
            //we use tha same dbcontext as the production, as the result the in memory database will have the same schema
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName).Options;

            var dbContext = new ApplicationDbContext(options);

            return dbContext;
        }

        protected IMapper BuildMap()
        {
            var config = new MapperConfiguration(options => 
            {
                options.AddProfile(new AutoMapperProfiles());
            });

            return config.CreateMapper();
        }
    }
}
