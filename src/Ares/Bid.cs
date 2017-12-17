using System.ComponentModel.DataAnnotations;

public class Bid
{
    [Required]
    public string UserId { get; set; }
    [Required]
    public int? Amount { get; set; }
    [Required]
    public int? AuctionId { get; set; }
}