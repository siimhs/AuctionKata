using NodaTime;

public class Auction
{
    public int Id { get; set; }
    public Product ProductOnAuction { get; set; }
    public Duration Duration { get; set; }
    public Instant Created { get; set; }
    public Instant Started { get; set; }
}