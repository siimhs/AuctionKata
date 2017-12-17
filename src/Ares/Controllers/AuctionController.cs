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
        if (ModelState.IsValid)
        {
            repository.Add(auction);

            return Json(auction);
        }
        else
        {
            return StatusCode(422);
        }
    }
}