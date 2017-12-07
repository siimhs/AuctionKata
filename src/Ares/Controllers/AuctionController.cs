using Microsoft.AspNetCore.Mvc;

public class AuctionsController : Controller
{
    private IRepository<Auction> repository;
    public AuctionsController(IRepository<Auction> repository)
    {
        this.repository = repository;
    }

    [HttpGet("{id}", Name = "GetAuction")]
    [Route("[controller]/{id}")]
    public IActionResult GetById(int id)
    {
        var auction = repository.GetById(id);
        return Json(auction);
    }
}