namespace NavigationModule.Domain.Entities
{
    public class Journey
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public string StartingLocation { get; set; }
        public string ArrivalLocation { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public int TransportationType { get; set; }
        public double RouteDistance { get; set; }

    }
}
