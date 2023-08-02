using System;
using System.ComponentModel.DataAnnotations;

public class JourneyFilterViewModel
{
    public string UserId { get; set; }

    public string UserName { get; set; }

    public int TransportationType { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime? StartDate { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime? EndDate { get; set; }
}