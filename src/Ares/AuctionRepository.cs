using System.Collections.Generic;
using System.Linq;

public class Repository<T> : IRepository<Auction>
{
    private readonly List<Auction> auctions;

    public Repository(List<Auction> auctions)
    {
        this.auctions = auctions;
    }
    public Auction GetById(int id)
    {
        return auctions.Where(p => p.Id == id).FirstOrDefault();
    }

    public void Add(Auction auction)
    {
        auctions.Add(auction);
    }
}