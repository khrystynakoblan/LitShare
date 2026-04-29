namespace LitShare.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using LitShare.BLL.DTOs;
    using LitShare.BLL.Services;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Moq.Protected;
    using Xunit;

    public class GoogleBooksApiServiceTests
    {
        private readonly Mock<ILogger<GoogleBooksApiService>> loggerMock;
        private readonly Mock<HttpMessageHandler> httpMessageHandlerMock;

        public GoogleBooksApiServiceTests()
        {
            this.loggerMock = new Mock<ILogger<GoogleBooksApiService>>();
            this.httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        }

        [Fact]
        public async Task GetBookDetailsAsync_WhenBookFound_ReturnsVolumeInfo()
        {
            var expectedBook = new GoogleBooksResponseDto
            {
                Items = new List<GoogleBookItem>
                {
                    new GoogleBookItem
                    {
                        VolumeInfo = new VolumeInfo
                        {
                            Title = "1984",
                            Authors = new List<string> { "Джордж Орвелл" },
                            Description = "Антиутопічний роман"
                        }
                    }
                }
            };

            var jsonResponse = JsonSerializer.Serialize(expectedBook);

            this.httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse)
                });

            var httpClient = new HttpClient(this.httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("https://www.googleapis.com/books/v1/")
            };

            var service = new GoogleBooksApiService(httpClient, this.loggerMock.Object);

            var result = await service.GetBookDetailsAsync("1984");

            Assert.NotNull(result);
            Assert.Equal("1984", result.Title);
            Assert.Contains("Джордж Орвелл", result.Authors!);
            Assert.Equal("Антиутопічний роман", result.Description);
        }

        [Fact]
        public async Task GetBookDetailsAsync_WhenBookNotFound_ReturnsNull()
        {
            var emptyResponse = new GoogleBooksResponseDto
            {
                Items = null 
            };

            var jsonResponse = JsonSerializer.Serialize(emptyResponse);

            this.httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse)
                });

            var httpClient = new HttpClient(this.httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("https://www.googleapis.com/books/v1/")
            };

            var service = new GoogleBooksApiService(httpClient, this.loggerMock.Object);

            var result = await service.GetBookDetailsAsync("UnknownBook12345");

            Assert.Null(result);
        }

        [Fact]
        public async Task GetBookDetailsAsync_WhenApiExceptionOccurs_ThrowsHttpRequestException()
        {
            this.httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Збій мережі"));

            var httpClient = new HttpClient(this.httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("https://www.googleapis.com/books/v1/")
            };

            var service = new GoogleBooksApiService(httpClient, this.loggerMock.Object);

            await Assert.ThrowsAsync<HttpRequestException>(() => service.GetBookDetailsAsync("1984"));
        }
    }
}