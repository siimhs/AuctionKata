using Ares;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AresTests
{
    public class PostAuctionsTests
    {
        private const string API_URL = "http://localhost:5000";

        [Fact]
        public async Task PostAuctionReturnsCreatedAuction()
        {
            var auction = new Auction()
            {
                Id = 123
            };

            var hostBuilder = Program.CreateWebHostBuilder(new string[] { })
                                        .UseUrls(API_URL);

            using (var server = new TestServer(hostBuilder))
            {
                var client = server.CreateClient();

                var auctionJson = JsonConvert.SerializeObject(auction);

                var content = new StringContent(auctionJson, Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"{API_URL}/Auctions", content);

                response.EnsureSuccessStatusCode();

                var responseJson = await response.Content.ReadAsStringAsync();

                var responseAuction = JsonConvert.DeserializeObject<Auction>(responseJson);

                Assert.Equal(auction.Id, responseAuction.Id);
            }
        }
    }
}
