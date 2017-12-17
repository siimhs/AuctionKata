using Microsoft.AspNetCore.Mvc;

public class BidsController : Controller
{
    private IRepository<Auction> repository;

    public BidsController(IRepository<Auction> repository)
    {
        this.repository = repository;
    }

    [HttpPost]
    [Route("[controller]")]
    public IActionResult CreateBid([FromBody] Bid bid)
    {
        if (ModelState.IsValid)
        {
            var auction = repository.GetById(bid.AuctionId.Value);

            if (auction != null)
            {
                return Json(bid);
            }
            else
            {
                return StatusCode(404);
            }
        }
        else
        {
            return StatusCode(422);
        }
    }
}