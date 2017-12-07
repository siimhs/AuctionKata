using System;
using Xunit;
using Ares;
using Moq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace AresTests
{
    public class AresResourcesTests
    {
        private const string API_URL = "http://localhost:5000";

        [Fact]
        public async Task GetAuctionByIdReturnsJson()
        {
            var expectedId = 1;
            var fakeAuction = new Auction() { Id = expectedId };

            var auctionRepository = new Mock<IRepository<Auction>>();
            
            auctionRepository
                .Setup(p => p.GetById(It.Is<int>(id => id == 1)))
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

                var response = await client.GetStringAsync($"{API_URL}/Auctions/{expectedId}");

                var auction = JsonConvert.DeserializeObject<Auction>(response);

                Assert.True(auction.Id == expectedId);
            }
        }
    }
}
