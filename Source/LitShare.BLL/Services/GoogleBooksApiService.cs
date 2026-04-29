namespace LitShare.BLL.Services
{
    using System.Net.Http.Json;
    using LitShare.BLL.DTOs;
    using LitShare.BLL.Services.Interfaces;
    using Microsoft.Extensions.Logging;

    public class GoogleBooksApiService : IExternalBookApiService
    {
        private readonly HttpClient httpClient;
        private readonly ILogger<GoogleBooksApiService> logger;

        public GoogleBooksApiService(HttpClient httpClient, ILogger<GoogleBooksApiService> logger)
        {
            this.httpClient = httpClient;
            this.logger = logger;
        }

        public async Task<VolumeInfo?> GetBookDetailsAsync(string bookTitle)
        {
            this.logger.LogInformation("Fetching external data for book: {BookTitle}", bookTitle);

            var response = await this.httpClient.GetFromJsonAsync<GoogleBooksResponseDto>(
                $"volumes?q=intitle:{bookTitle}&maxResults=1&langRestrict=uk");

            var bookInfo = response?.Items?.FirstOrDefault()?.VolumeInfo;

            if (bookInfo != null)
            {
                this.logger.LogInformation("Successfully found external data for: {BookTitle}", bookTitle);
            }
            else
            {
                this.logger.LogWarning("No external data found for: {BookTitle}", bookTitle);
            }

            return bookInfo;
        }
    }
}