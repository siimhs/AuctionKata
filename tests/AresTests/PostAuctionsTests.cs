using Ares;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Net;
using NodaTime;
using NodaTime.Serialization.JsonNet;

namespace AresTests
{
    public class PostAuctionsTests
    {
        private const string API_URL = "http://localhost:5000";
        private JsonSerializerSettings jsonSettings = new JsonSerializerSettings().ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);

        [Fact]
        public async Task PostAuctionReturnsCreatedAuction()
        {
            var auction = new Auction()
            {
                ProductOnAuction = new Product()
                {
                    Name = "My product",
                    Description = "My product is worth its price!"
                },
                Duration = Duration.FromHours(2),
                UserId = "User123"
            };

            var response = await Post(auction);

            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();

            var responseAuction = JsonConvert.DeserializeObject<Auction>(responseJson, jsonSettings);

            Assert.Equal(auction.ProductOnAuction.Name, responseAuction.ProductOnAuction.Name);
            Assert.Equal(auction.ProductOnAuction.Description, responseAuction.ProductOnAuction.Description);
            Assert.Equal(auction.Duration, responseAuction.Duration);
        }

        [Fact]
        public async Task PostAuctionWithoutDurationReturns422()
        {
            var auction = new Auction()
            {
                ProductOnAuction = new Product()
                {
                    Name = "My product",
                    Description = "My product is worth its price!"
                },
                UserId = "User123"
            };

            var response = await Post(auction);

            Assert.Equal((HttpStatusCode)422, response.StatusCode);
        }

        [Fact]
        public async Task PostAuctionWithoutProductReturns422()
        {
            var auction = new Auction()
            {
                Duration = null,
                UserId = "User123"
            };

            var response = await Post(auction);

            Assert.Equal((HttpStatusCode)422, response.StatusCode);
        }

        [Fact]
        public async Task PostAuctionWithoutUserIdReturns422()
        {
            var auction = new Auction()
            {
                Duration = Duration.Zero,
                ProductOnAuction = new Product()
                {
                    Name = "My product",
                    Description = "My product is worth its price!"
                },
                UserId = null
            };

            var response = await Post(auction);

            Assert.Equal((HttpStatusCode)422, response.StatusCode);
        }

        private async Task<HttpResponseMessage> Post(Auction auction)
        {
            var hostBuilder = Program.CreateWebHostBuilder(new string[] { })
                                        .UseUrls(API_URL);

            using (var server = new TestServer(hostBuilder))
            {
                var client = server.CreateClient();

                var auctionJson = JsonConvert.SerializeObject(auction, jsonSettings);

                var content = new StringContent(auctionJson, Encoding.UTF8, "application/json");

                return await client.PostAsync($"{API_URL}/Auctions", content);
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
            var auction = new Auction()
            {
                Id = id,
                Duration = default(Duration),
                ProductOnAuction = new Product()
                {
                    Name = "My cool product",
                    Description = "This is the best product in the world!"
                },
                UserId = "User123"
            };

            var auctionJson = JsonConvert.SerializeObject(auction, jsonSettings);

            var content = new StringContent(auctionJson, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{API_URL}/Auctions", content);

            response.EnsureSuccessStatusCode();
        }

        private async Task AssertThatAuctionExists(HttpClient client, int id)
        {
            var response = await client.GetAsync($"{API_URL}/Auctions/{id}");

            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();

            var responseAuction = JsonConvert.DeserializeObject<Auction>(responseJson, jsonSettings);

            Assert.Equal(id, responseAuction.Id);
        }
    }
}
