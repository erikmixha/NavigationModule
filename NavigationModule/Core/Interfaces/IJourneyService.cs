using NavigationModule.Domain.Entities;
using NavigationModule.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IJourneyService
{
    Task<Journey> CreateJourneyAsync(JourneyCreateViewModel journeyCreateViewModel, string userId);
    Task<bool> DeleteJourneyAsync(int journeyId, string userId);
    Task<Journey> GetJourneyByIdAsync(int journeyId);
    Task<List<Journey>> GetJourneysByUserIdAsync(string userId);
    Task<Journey> UpdateJourneyAsync(JourneyUpdateViewModel journeyUpdateViewModel, string userId);
    Task<List<Journey>> GetFilteredJourneysAsync(JourneyFilterViewModel filterViewModel);
    Task<List<MonthlyRouteDistanceViewModel>> GetMonthlyRouteDistanceAsync();
}