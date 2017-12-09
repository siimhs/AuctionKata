using Xunit;
using Ares;
using Moq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;

namespace AresTests
{
    public class AresGetAuctionsTests
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

            var hostBuilder = CreateAndSetupHostBuilderWith(auctionRepository.Object);

            var response = await GetResponse(hostBuilder, $"{API_URL}/Auctions/{expectedId}");

            var json = response.Content.ReadAsStringAsync().Result;

            var auction = JsonConvert.DeserializeObject<Auction>(json);

            response.EnsureSuccessStatusCode();
            Assert.True(auction.Id == expectedId);
        }

        [Fact]
        public async Task GetAuctionByIdReturns404WhenNotFound()
        {
            var auctionRepository = new Mock<IRepository<Auction>>();

            auctionRepository
                .Setup(p => p.GetById(It.IsAny<int>()))
                .Returns(default(Auction));

            var hostBuilder = CreateAndSetupHostBuilderWith(auctionRepository.Object);

            var notExistingResourceId = 999;
            var response = await GetResponse(hostBuilder, $"{API_URL}/Auctions/{notExistingResourceId}");

            Assert.True(response.StatusCode == HttpStatusCode.NotFound);
        }

        private IWebHostBuilder CreateAndSetupHostBuilderWith(IRepository<Auction> repository)
        {
            var hostBuilder = Program.CreateWebHostBuilder(new string[] { })
                                        .UseUrls(API_URL)
                                        .ConfigureServices((services) =>
                                        {
                                            services.AddTransient((s) =>
                                            {
                                                return repository;
                                            });
                                        });
            return hostBuilder;
        }

        private async Task<HttpResponseMessage> GetResponse(IWebHostBuilder hostBuilder, string url)
        {
            using (var server = new TestServer(hostBuilder))
            {
                var client = server.CreateClient();

                return await client.GetAsync(url);
            }
        }
    }
}
