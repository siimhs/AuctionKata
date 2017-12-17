using Ares;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
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

            var auctionRepository = new Mock<IRepository<Auction>>();

            auctionRepository
                .Setup(p => p.GetById(It.IsAny<int>()))
                .Returns(new Auction() { Id = bid.AuctionId.Value });

            var services = new ServiceCollection();

            services.AddTransient((s) =>
            {
                return auctionRepository.Object;
            });

            var host = CreateHost(services);

            var response = await Post(bid, host);

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

            var host = CreateHost();

            var response = await Post(bid, host);

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

            var host = CreateHost();

            var response = await Post(bid, host);

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

            var host = CreateHost();

            var response = await Post(bid, host);

            Assert.Equal((HttpStatusCode)422, response.StatusCode);
        }

        [Fact]
        public async Task PostBidWithoutExistingAuctionReturns404NotFound()
        {
            var bid = new Bid()
            {
                AuctionId = 1,
                UserId = "User123",
                Amount = 100
            };

            var auctionRepository = new Mock<IRepository<Auction>>();

            auctionRepository
                .Setup(p => p.GetById(It.IsAny<int>()))
                .Returns(default(Auction));

            var services = new ServiceCollection();

            services.AddTransient((s) =>
            {
                return auctionRepository.Object;
            });

            var host = CreateHost(services);

            var response = await Post(bid, host);

            Assert.Equal((HttpStatusCode)404, response.StatusCode);
        }

        private IWebHostBuilder CreateHost()
        {
            return CreateHost(new ServiceCollection());
        }

        private IWebHostBuilder CreateHost(IServiceCollection services)
        {
            var hostBuilder = Program.CreateWebHostBuilder(new string[] { })
                                        .UseUrls(API_URL)
                                        .ConfigureServices((s) =>
                                        {
                                            foreach (var service in services)
                                            {
                                                s.Add(service);
                                            }
                                        });

            return hostBuilder;
        }

        private async Task<HttpResponseMessage> Post(Bid bid, IWebHostBuilder hostBuilder)
        {
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