using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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

        if (auction == null)
        {
            return NotFound();
        }
        else
        {
            return Json(auction);
        }
    }

    [HttpPost]
    [Route("[controller]")]
    public IActionResult AddAuction([FromBody] Auction auction)
    {
        return Json(auction);
    }
}