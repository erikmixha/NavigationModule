using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NavigationModule.Domain.Entities;
using NavigationModule.ViewModels;
using System;
using System.Threading.Tasks;
using Xunit;

[TestClass]

public class JourneyServiceTests
{
    [TestMethod]
    public async Task CreateJourneyAsync_Should_Create_Journey()
    {
        // Arrange
        var userId = "ae962c62-b8fe-4126-8675-5eed354a8030";
        var journeyCreateViewModel = new JourneyCreateViewModel
        {
            StartingLocation = "Location A",
            ArrivalLocation = "Location B",
            StartTime = DateTime.Now,
            ArrivalTime = DateTime.Now.AddHours(5),
            RouteDistance = 10,
            TransportationType = 1,
        };

        // Mock the ApplicationDbContext using an in-memory database
        var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using (var dbContext = new ApplicationDbContext(dbContextOptions))
        {
            // Create a mock for JourneyService 
            var journeyServiceMock = new Mock<JourneyService>(dbContext, new MemoryCache(new MemoryCacheOptions()));

            // Use the mocked JourneyService
            var journeyService = journeyServiceMock.Object;

            // Act
            var journey = await journeyService.CreateJourneyAsync(journeyCreateViewModel, userId);

            // Assert
            Assert.IsNotNull(journey);
            Assert.AreEqual(userId, journey.UserId);

        }
    }

    [TestMethod]
    public async Task DeleteJourneyAsync_Should_Delete_Journey()
    {
        // Arrange
        var userId = "ae962c62-b8fe-4126-8675-5eed354a8030";
        var journeyId = 2;
        var journey = new Journey { Id = journeyId, UserId = userId };

        // Mock the ApplicationDbContext using an in-memory database
        var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using (var dbContext = new ApplicationDbContext(dbContextOptions))
        {
            // Add the journey to the in-memory database
            dbContext.Journeys.Add(journey);
            dbContext.SaveChanges();


            // Create a mock for JourneyService 
            var journeyServiceMock = new Mock<JourneyService>(dbContext, new MemoryCache(new MemoryCacheOptions()));

            var journeyService = journeyServiceMock.Object;

            // Act
            var isDeleted = await journeyService.DeleteJourneyAsync(journeyId, userId);

            // Assert
            Assert.IsTrue(isDeleted);

            // Verify that the journey is deleted from the database
            Assert.IsNull(dbContext.Journeys.FirstOrDefault(j => j.Id == journeyId));

        }
    }

    [TestMethod]
    public async Task GetJourneyByIdAsync_Should_Return_Correct_Journey()
    {
        // Arrange
        var userId = "ae962c62-b8fe-4126-8675-5eed354a8030";
        var journeyId = 2;
        var journey = new Journey { Id = journeyId, UserId = userId };

        // Mock the ApplicationDbContext using an in-memory database
        var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using (var dbContext = new ApplicationDbContext(dbContextOptions))
        {
            // Add the journey to the in-memory database
            dbContext.Journeys.Add(journey);
            dbContext.SaveChanges();

            var journeyService = new JourneyService(dbContext, new MemoryCache(new MemoryCacheOptions()));

            // Act
            var result = await journeyService.GetJourneyByIdAsync(journeyId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(journeyId, result.Id);
        }
    }

    [TestMethod]
    public async Task GetJourneysByUserIdAsync_Should_Return_Correct_Journeys()
    {
        // Arrange
        var userId = "ae962c62-b8fe-4126-8675-5eed354a8030";
        var journeys = new List<Journey>
    {
        new Journey { Id = 2, UserId = userId },
        new Journey { Id = 3, UserId = userId }
    };

        // Mock the ApplicationDbContext using an in-memory database
        var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using (var dbContext = new ApplicationDbContext(dbContextOptions))
        {
            // Add the journeys to the in-memory database
            dbContext.Journeys.AddRange(journeys);
            dbContext.SaveChanges();


            var journeyService = new JourneyService(dbContext, new MemoryCache(new MemoryCacheOptions()));

            // Act
            var result = await journeyService.GetJourneysByUserIdAsync(userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(journeys.Count, result.Count);
            Assert.IsTrue(result.All(j => j.UserId == userId));
        }
    }

    [TestMethod]
    public async Task UpdateJourneyAsync_Should_Update_Journey()
    {
        // Arrange
        var userId = "ae962c62-b8fe-4126-8675-5eed354a8030";
        var journeyId = 1;
        var originalStartingLocation = "Location A";
        var updatedStartingLocation = " Location C";
        var journey = new Journey { Id = journeyId, UserId = userId, StartingLocation = originalStartingLocation };

        // Mock the ApplicationDbContext using an in-memory database
        var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using (var dbContext = new ApplicationDbContext(dbContextOptions))
        {
            // Add the journey to the in-memory database
            dbContext.Journeys.Add(journey);
            dbContext.SaveChanges();


            // Create a mock for JourneyService
            var journeyServiceMock = new Mock<JourneyService>(dbContext, new MemoryCache(new MemoryCacheOptions()));
      
            var journeyService = journeyServiceMock.Object;

            var journeyUpdateViewModel = new JourneyUpdateViewModel
            {
                Id = journeyId,
                StartingLocation = updatedStartingLocation
            };

            // Act
            var updatedJourney = await journeyService.UpdateJourneyAsync(journeyUpdateViewModel, userId);

            // Assert
            Assert.IsNotNull(updatedJourney);
            Assert.AreEqual(updatedStartingLocation, updatedJourney.StartingLocation);

            // Verify that the journey is updated in the database
            var updatedJourneyInDb = dbContext.Journeys.FirstOrDefault(j => j.Id == journeyId);
            Assert.IsNotNull(updatedJourneyInDb);
            Assert.AreEqual(updatedStartingLocation, updatedJourneyInDb.StartingLocation);

        }
    }

    [TestMethod]
    public async Task GetFilteredJourneysAsync_Should_Return_Correct_Journeys_Based_On_Filter()
    {
        // Arrange
        var userId = "ae962c62-b8fe-4126-8675-5eed354a8030";
        var journeys = new List<Journey>
    {
        new Journey { Id = 1, UserId = userId, TransportationType = 1, StartTime = DateTime.Now },
        new Journey { Id = 2, UserId = userId, TransportationType = 2, StartTime = DateTime.Now.AddDays(-2) },
        new Journey { Id = 3, UserId = userId, TransportationType = 1, StartTime = DateTime.Now.AddMonths(-1) },
    };

        // Mock the ApplicationDbContext using an in-memory database
        var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using (var dbContext = new ApplicationDbContext(dbContextOptions))
        {
            // Add the journeys to the in-memory database
            dbContext.Journeys.AddRange(journeys);
            dbContext.SaveChanges();


            var journeyService = new JourneyService(dbContext, new MemoryCache(new MemoryCacheOptions()));

            var filterViewModel = new JourneyFilterViewModel
            {
                UserId = userId,
                TransportationType = 1,
                StartDate = DateTime.Now.AddDays(-7),
                EndDate = DateTime.Now
            };

            // Act
            var result = await journeyService.GetFilteredJourneysAsync(filterViewModel);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result[0].Id);
            Assert.AreEqual(userId, result[0].UserId);
            Assert.AreEqual(1, result[0].TransportationType);
        }
    }

    [TestMethod]
    public async Task GetMonthlyRouteDistanceAsync_Should_Return_Correct_MonthlyRouteDistance()
    {
        // Arrange
        var userId = "ae962c62-b8fe-4126-8675-5eed354a8030";
        var journeys = new List<Journey>
    {
        new Journey { Id = 1, UserId = userId, RouteDistance = 10, StartTime = DateTime.Now.AddMonths(-1) },
        new Journey { Id = 2, UserId = userId, RouteDistance = 15, StartTime = DateTime.Now.AddMonths(-2) },
        new Journey { Id = 3, UserId = userId, RouteDistance = 5, StartTime = DateTime.Now.AddMonths(-2) },
    };

        // Mock the ApplicationDbContext using an in-memory database
        var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using (var dbContext = new ApplicationDbContext(dbContextOptions))
        {
            // Add the journeys to the in-memory database
            dbContext.Journeys.AddRange(journeys);
            dbContext.SaveChanges();


            var journeyService = new JourneyService(dbContext, new MemoryCache(new MemoryCacheOptions()));

            // Act
            var result = await journeyService.GetMonthlyRouteDistanceAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);

            // Verify that the results are sorted by year and month
            Assert.IsTrue(result[0].Year >= result[1].Year);
            if (result[0].Year == result[1].Year)
            {
                Assert.IsTrue(result[0].Month >= result[1].Month);
            }
        }
    }
}