using Microsoft.AspNetCore.Mvc;

public class BidsController : Controller
{
    [HttpPost]
    [Route("[controller]")]
    public IActionResult CreateBid([FromBody] Bid bid)
    {
        if (ModelState.IsValid)
        {
            return Json(bid);
        }
        else
        {
            return StatusCode(422);
        }
    }
}