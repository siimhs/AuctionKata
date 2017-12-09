using Xunit;
using Ares;
using Moq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net;

namespace AresTests
{
    public class AresResourcesTests
    {
        private const string API_URL = "http://localhost:5000";

        [Fact]
        public async Task GetAuctionByIdReturnsExistingAuctionWhenFound()
        {
            var expectedId = 1;
            var fakeAuction = new Auction() { Id = expectedId };

            var auctionRepository = new Mock<IRepository<Auction>>();

            auctionRepository
                .Setup(p => p.GetById(It.IsAny<int>()))
                .Returns(fakeAuction);

            var hostBuilder = Program.CreateWebHostBuilder(new string[] { })
                                        .UseUrls(API_URL)
                                        .ConfigureServices((services) =>
                                        {
                                            services.AddTransient((s) =>
                                            {
                                                return auctionRepository.Object;
                                            });
                                        });

            using (var server = new TestServer(hostBuilder))
            {
                var client = server.CreateClient();

                var response = await client.GetAsync($"{API_URL}/Auctions/{expectedId}");

                var json = response.Content.ReadAsStringAsync().Result;

                var auction = JsonConvert.DeserializeObject<Auction>(json);

                response.EnsureSuccessStatusCode();
                Assert.True(auction.Id == expectedId);                
            }
        }

        [Fact]
        public async Task GetAuctionByIdReturns404WhenNotFound()
        {
            var auctionRepository = new Mock<IRepository<Auction>>();

            auctionRepository
                .Setup(p => p.GetById(It.IsAny<int>()))
                .Returns(default(Auction));

            var hostBuilder = Program.CreateWebHostBuilder(new string[] { })
                                        .UseUrls(API_URL)
                                        .ConfigureServices((services) =>
                                        {
                                            services.AddTransient((s) =>
                                            {
                                                return auctionRepository.Object;
                                            });
                                        });

            using (var server = new TestServer(hostBuilder))
            {
                var client = server.CreateClient();

                var notExistingResourceId = 999;
                var response = await client.GetAsync($"{API_URL}/Auctions/{notExistingResourceId}");

                Assert.True(response.StatusCode == HttpStatusCode.NotFound);
            }
        }
    }
}
