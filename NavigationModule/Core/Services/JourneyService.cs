using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using NavigationModule.Domain.Entities;
using NavigationModule.ViewModels;

public class JourneyService : IJourneyService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMemoryCache _memoryCache;

    public JourneyService(ApplicationDbContext dbContext, IMemoryCache memoryCache)
    {
        _dbContext = dbContext;
        _memoryCache = memoryCache;
    }

    public async Task<Journey> CreateJourneyAsync(JourneyCreateViewModel journeyCreateViewModel, string userId)
    {

        if (journeyCreateViewModel.StartTime > journeyCreateViewModel.ArrivalTime)
        {
            throw new ArgumentException("StartTime cannot be greater than ArrivalTime.");
        }
        


        var journey = new Journey
        {
            UserId = userId,
            StartingLocation = journeyCreateViewModel.StartingLocation,
            ArrivalLocation = journeyCreateViewModel.ArrivalLocation,
            StartTime = journeyCreateViewModel.StartTime,
            ArrivalTime = journeyCreateViewModel.ArrivalTime,
            TransportationType = journeyCreateViewModel.TransportationType,
            RouteDistance = journeyCreateViewModel.RouteDistance
        };

        _dbContext.Journeys.Add(journey);
        _dbContext.SaveChanges();

        //update DailyStatus
        await CheckDailyGoalAchievementAsync(userId);

        return journey;
    }

    public async Task<bool> DeleteJourneyAsync(int journeyId, string userId)
    {
        var journey = await _dbContext.Journeys
            .Where(j => j.Id == journeyId && j.UserId == userId)
            .FirstOrDefaultAsync();

        if (journey == null)
            return false;

        _dbContext.Journeys.Remove(journey);
        _dbContext.SaveChanges();

        // Invalidate cache for the deleted journey
        string cacheKey = $"Journey_{journeyId}";
        _memoryCache.Remove(cacheKey);

        //update DailyStatus
        await CheckDailyGoalAchievementAsync(userId);

        return true;

    }

    public async Task<Journey> GetJourneyByIdAsync(int journeyId)
    {
        string cacheKey = $"Journey_{journeyId}";
        if (!_memoryCache.TryGetValue(cacheKey, out Journey journey))
        {
            journey = await _dbContext.Journeys
                .Where(j => j.Id == journeyId)
                .FirstOrDefaultAsync();

            if (journey != null)
            {
                // Cache the journey for 10 minutes
                _memoryCache.Set(cacheKey, journey, TimeSpan.FromMinutes(10));
            }
        }

        return journey;
    }


    public async Task<List<Journey>> GetJourneysByUserIdAsync(string userId)
    {
        string cacheKey = $"UserJourneys_{userId}";
        if (!_memoryCache.TryGetValue(cacheKey, out List<Journey> journeys))
        {
            journeys = await _dbContext.Journeys
                .Where(j => j.UserId == userId)
                .ToListAsync();

            if (journeys.Any())
            {
                // Cache the user's journeys for 10 minutes
                _memoryCache.Set(cacheKey, journeys, TimeSpan.FromMinutes(10));
            }
        }

        return journeys;
    }



    public async Task<Journey> UpdateJourneyAsync(JourneyUpdateViewModel journeyUpdateViewModel, string userId)
    {
        var journey = await _dbContext.Journeys
            .FirstOrDefaultAsync(j => j.Id == journeyUpdateViewModel.Id && j.UserId == userId);


        if (journey == null)
            return null;

        if (journeyUpdateViewModel.StartTime > journeyUpdateViewModel.ArrivalTime)
        {
            throw new ArgumentException("StartTime cannot be greater than ArrivalTime.");
        }
        

        journey.StartingLocation = journeyUpdateViewModel.StartingLocation;
        journey.ArrivalLocation = journeyUpdateViewModel.ArrivalLocation;
        journey.StartTime = journeyUpdateViewModel.StartTime;
        journey.ArrivalTime = journeyUpdateViewModel.ArrivalTime;
        journey.TransportationType = journeyUpdateViewModel.TransportationType;
        journey.RouteDistance = journeyUpdateViewModel.RouteDistance;

        _dbContext.SaveChanges();

        //update DailyStatus
        await CheckDailyGoalAchievementAsync(userId);

        return journey;
    }


    public async Task CheckDailyGoalAchievementAsync(string userId)
    {
        var journeys = await _dbContext.Journeys
            .Where(x => x.UserId == userId)
            .ToListAsync();

        if (journeys.Any())
        {

            var user = await _dbContext.Users
                    .FirstOrDefaultAsync(u => u.Id == userId);

            if (user != null)
            {
                double totalRouteDistance = journeys.Sum(j => j.RouteDistance);

                bool isDailyGoalAchieved = totalRouteDistance > 20.0;
                if (isDailyGoalAchieved && !user.DailyGoalAchievementStatus)
                {
                    user.DailyGoalAchievementStatus = isDailyGoalAchieved;
                    await _dbContext.SaveChangesAsync();


                }

                //no need to save on db if it previously hasnt been true
                if (!isDailyGoalAchieved && user.DailyGoalAchievementStatus)
                {
                    user.DailyGoalAchievementStatus = false;
                    await _dbContext.SaveChangesAsync();

                }



            }


        }
    }


    public async Task<List<Journey>> GetFilteredJourneysAsync(JourneyFilterViewModel filterViewModel)
    {
        IQueryable<Journey> query = _dbContext.Journeys;

        if (!string.IsNullOrEmpty(filterViewModel.UserId))
        {
            // Filter journeys by UserId if provided
            query = query.Where(j => j.UserId == filterViewModel.UserId);
        }

        if (!string.IsNullOrEmpty(filterViewModel.UserName))
        {
            // Filter journeys by UserName if provided
            query = query.Where(j => j.User.UserName == filterViewModel.UserName);
        }

        if (!string.IsNullOrEmpty(filterViewModel.UserName))
        {
            // Filter journeys by UserName if provided
            query = query.Where(j => j.User.UserName == filterViewModel.UserName);
        }

        if (filterViewModel.TransportationType != 0)
        {
            // Filter journeys by TransportationType if provided
            query = query.Where(j => j.TransportationType == filterViewModel.TransportationType);
        }

        if (filterViewModel.StartDate.HasValue)
        {
            // Filter journeys by StartDate if provided
            query = query.Where(j => j.StartTime.Date >= filterViewModel.StartDate.Value.Date);
        }

        if (filterViewModel.EndDate.HasValue)
        {
            // Filter journeys by EndDate if provided
            query = query.Where(j => j.ArrivalTime.Date <= filterViewModel.EndDate.Value.Date);
        }

        if (filterViewModel.StartDate.HasValue && filterViewModel.EndDate.HasValue)
        {
            if (filterViewModel.StartDate.Value > filterViewModel.EndDate.Value)
            {
                throw new ArgumentException("Start date cannot be greater than end date.");
            }
            
        }

        return await query.ToListAsync();
    }



    public async Task<List<MonthlyRouteDistanceViewModel>> GetMonthlyRouteDistanceAsync()
    {
        string cacheKey = "MonthlyRouteDistances";
        if (!_memoryCache.TryGetValue(cacheKey, out List<MonthlyRouteDistanceViewModel> monthlyRouteDistances))
        {
            monthlyRouteDistances = await _dbContext.Journeys
                .Include(j => j.User)
                .Where(j => j.StartTime >= DateTime.Today.AddMonths(-12)) // Filter journeys within the last 12 months
                .GroupBy(j => new { Month = j.StartTime.Month, Year = j.StartTime.Year, j.UserId, j.User.UserName })
                .Select(group => new MonthlyRouteDistanceViewModel
                {
                    Year = group.Key.Year,
                    Month = group.Key.Month,
                    TotalRouteDistance = group.Sum(j => j.RouteDistance),
                    UserName = group.Key.UserName
                })
                .ToListAsync();

            if (monthlyRouteDistances.Any())
            {
                // Cache the monthly route distances 10 minutes
                _memoryCache.Set(cacheKey, monthlyRouteDistances, TimeSpan.FromMinutes(10));
            }
        }

        return monthlyRouteDistances;
    }



}