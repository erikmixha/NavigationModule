using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NavigationModule.Domain.Entities;
using NavigationModule.ViewModels;

[ApiController]
[Route("api/journey")]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "journey")]
[Authorize]
public class JourneyController : ControllerBase
{
    private readonly IJourneyService _journeyService;

    public JourneyController(IJourneyService journeyService)
    {
        _journeyService = journeyService;
    }

    [HttpPost("create-journey")]
    public async Task<ActionResult<Journey>> CreateJourney(JourneyCreateViewModel journeyCreateViewModel)
    {
        try
        {
            // Get the UserId from the current authenticated user
            string userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return NotFound(new { message = "No user logged in" });
            }

            var journey = await _journeyService.CreateJourneyAsync(journeyCreateViewModel, userId);
            return Ok(journey);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Failed to create journey.", error = ex.Message });
        }
    }

    [HttpPut("update-journey")]
    public async Task<ActionResult<Journey>> UpdateJourney(JourneyUpdateViewModel journeyUpdateViewModel)
    {
        try
        {
            // Get the UserId from the current authenticated user
            string userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var journey = await _journeyService.UpdateJourneyAsync(journeyUpdateViewModel, userId);
            if (journey == null)
            {
                return NotFound(new { message = "Journey not found" });
            }

            return Ok(journey);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Failed to update journey.", error = ex.Message });
        }
    }

    [HttpDelete("delete-journey/{journeyId}")]
    public async Task<ActionResult<bool>> DeleteJourney(int journeyId)
    {
        try
        {
            string userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            bool isDeleted = await _journeyService.DeleteJourneyAsync(journeyId, userId);
            return Ok(isDeleted);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Failed to delete journey.", error = ex.Message });
        }
    }

    [HttpGet("get-journey-by-id/{journeyId}")]
    public async Task<ActionResult<Journey>> GetJourneyById(int journeyId)
    {
        try
        {

            var journey = await _journeyService.GetJourneyByIdAsync(journeyId);
            if (journey == null)
            {
                return NotFound(new { message = "Journey not found" });
            }

            return Ok(journey);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Failed to get journey.", error = ex.Message });
        }
    }

    [HttpGet("get-journeys-by-user-id")]
    public async Task<ActionResult<List<Journey>>> GetJourneysByUserId()
    {
        try
        {
            string userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var journeys = await _journeyService.GetJourneysByUserIdAsync(userId);
            return Ok(journeys);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Failed to get journeys.", error = ex.Message });
        }
    }

    [HttpGet("get-filtered-journeys")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<List<Journey>>> GetFilteredJourneys([FromQuery] JourneyFilterViewModel filterViewModel)
    {
        try
        {
            var filteredJourneys = await _journeyService.GetFilteredJourneysAsync(filterViewModel);
            return Ok(filteredJourneys);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Failed to get filtered journeys.", error = ex.Message });
        }
    }

    [HttpGet("get-monthly-route-distance")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<MonthlyRouteDistanceViewModel>> GetMonthlyRouteDistance()
    {
        try
        {
            var monthlyRouteDistance = await _journeyService.GetMonthlyRouteDistanceAsync();
            return Ok(monthlyRouteDistance);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Failed to get monthly route distance.", error = ex.Message });
        }
    }
}