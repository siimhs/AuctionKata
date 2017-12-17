using Ares;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using NodaTime;
using NodaTime.Serialization.JsonNet;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AresTests
{
    public class PostBidTests
    {
        private const string API_URL = "http://localhost:5000";
        private JsonSerializerSettings jsonSettings = new JsonSerializerSettings().ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);

        [Fact]
        public async Task PostSuccessfulBidReturnsCreatedBid()
        {
            var bid = new Bid()
            {
                AuctionId = 1,
                Amount = 100,
                UserId = "User123"
            };

            var response = await Post(bid);

            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();

            var createdBid = JsonConvert.DeserializeObject<Bid>(responseJson, jsonSettings);

            Assert.Equal(bid.UserId, createdBid.UserId);
            Assert.Equal(bid.Amount, createdBid.Amount);
            Assert.Equal(bid.AuctionId, createdBid.AuctionId);
        }

        [Fact]
        public async Task PostBidWithoutAuctionIdReturns422()
        {
            var bid = new Bid()
            {
                UserId = "User123",
                Amount = 100
            };

            var response = await Post(bid);

            Assert.Equal((HttpStatusCode)422, response.StatusCode);
        }

        [Fact]
        public async Task PostBidWithoutUserIdReturns422()
        {
            var bid = new Bid()
            {
                AuctionId = 1,
                Amount = 100
            };

            var response = await Post(bid);

            Assert.Equal((HttpStatusCode)422, response.StatusCode);
        }

        [Fact]
        public async Task PostBidWithoutAmountReturns422()
        {
            var bid = new Bid()
            {
                AuctionId = 1,
                UserId = "User123"
            };

            var response = await Post(bid);

            Assert.Equal((HttpStatusCode)422, response.StatusCode);
        }

        private async Task<HttpResponseMessage> Post(Bid bid)
        {
            var hostBuilder = Program.CreateWebHostBuilder(new string[] { })
                                        .UseUrls(API_URL);

            using (var server = new TestServer(hostBuilder))
            {
                var client = server.CreateClient();

                var bidJson = JsonConvert.SerializeObject(bid, jsonSettings);

                var content = new StringContent(bidJson, Encoding.UTF8, "application/json");

                return await client.PostAsync($"{API_URL}/Bids", content);
            }
        }
    }
}