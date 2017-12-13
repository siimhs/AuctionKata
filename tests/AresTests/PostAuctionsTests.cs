using Ares;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Net;

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

        [Fact]
        public async Task PostAuctionCreatesAuction()
        {
            var auctionIdUnderTest = 222;

            var hostBuilder = Program.CreateWebHostBuilder(new string[] { })
                                        .UseUrls(API_URL);

            using (var server = new TestServer(hostBuilder))
            {
                var client = server.CreateClient();
                await AssertThatAuctionDoesNotExistsYet(client, auctionIdUnderTest);
                await CreateAuction(client, auctionIdUnderTest);
                await AssertThatAuctionExists(client, auctionIdUnderTest);
            }
        }

        private async Task AssertThatAuctionDoesNotExistsYet(HttpClient client, int id)
        {
            var response = await client.GetAsync($"{API_URL}/Auctions/{id}");

            Assert.True(response.StatusCode == HttpStatusCode.NotFound);
        }

        private async Task CreateAuction(HttpClient client, int id)
        {
            var auctionJson = JsonConvert.SerializeObject(new Auction() { Id = id });

            var content = new StringContent(auctionJson, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{API_URL}/Auctions", content);

            response.EnsureSuccessStatusCode();
        }

        private async Task AssertThatAuctionExists(HttpClient client, int id)
        {
            var response = await client.GetAsync($"{API_URL}/Auctions/{id}");
            
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();

            var responseAuction = JsonConvert.DeserializeObject<Auction>(responseJson);

            Assert.Equal(id, responseAuction.Id);
        }
    }
}
