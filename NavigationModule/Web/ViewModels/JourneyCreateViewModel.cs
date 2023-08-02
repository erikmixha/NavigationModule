using System;
using System.ComponentModel.DataAnnotations;

public class JourneyCreateViewModel
{
    [Required(ErrorMessage = "Starting location is required.")]
    public string StartingLocation { get; set; }

    [Required(ErrorMessage = "Arrival location is required.")]
    public string ArrivalLocation { get; set; }

    [Required(ErrorMessage = "Start time is required.")]

    [DataType(DataType.DateTime)]
    public DateTime StartTime { get; set; }

    [DataType(DataType.DateTime)]
    [Required(ErrorMessage = "Arrival time is required.")]
    public DateTime ArrivalTime { get; set; }

    [Required(ErrorMessage = "Transportation type is required.")]
    public int TransportationType { get; set; }

    [Required(ErrorMessage = "Route distance is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Route distance must be greater than 0.")]
    public double RouteDistance { get; set; }
}
