using NodaTime;
using System.ComponentModel.DataAnnotations;

public class Auction
{
    public int Id { get; set; }
    public Product ProductOnAuction { get; set; }
    [Required]
    public Duration? Duration { get; set; }
    public Instant Created { get; set; }
    public Instant Started { get; set; }
}