using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace NavigationModule.Domain.Entities
{
    public class User : IdentityUser
    {
        public bool DailyGoalAchievementStatus { get; set; }
        [NotMapped]
        public Journey Journeys { get; set; }

    }
}
