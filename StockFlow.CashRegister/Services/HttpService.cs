using StockFlow.Application.Common;
using StockFlow.CashRegister.Interfaces;
using System.Net.Http.Json;

namespace StockFlow.CashRegister.Services
{
    public class HttpService : IHttpService
    {
        private readonly HttpClient _httpClient;
        private const string Error = "An error occurred while processing the request.";

        public HttpService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse<TResponse>> GetAsync<TResponse>(string url)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);
                return await ReadAnswer<TResponse>(response);
            }
            catch (Exception)
            {
                return ApiResponse<TResponse>.Fail(Error);
            }
        }

        public async Task<ApiResponse<TResponse>> PostAsync<TResponse, TRequest>(string url, TRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(url, request);
                return await ReadAnswer<TResponse>(response);
            }
            catch (Exception)
            {
                return ApiResponse<TResponse>.Fail(Error);
            }
        }

        public async Task<ApiResponse<TResponse>> PutAsync<TResponse, TRequest>(string url, TRequest request)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync(url, request);
                return await ReadAnswer<TResponse>(response);
            }
            catch (Exception)
            {
                return ApiResponse<TResponse>.Fail(Error);
            }
        }

        public async Task<ApiResponse<TResponse>> DeleteAsync<TResponse>(string url)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(url);
                return await ReadAnswer<TResponse>(response);
            }
            catch (Exception)
            {
                return ApiResponse<TResponse>.Fail(Error);
            }
        }

        private static async Task<ApiResponse<TResponse>> ReadAnswer<TResponse>(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                string message = await response.Content.ReadAsStringAsync();
                return ApiResponse<TResponse>.Fail(string.IsNullOrWhiteSpace(message) ? Error : message);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return ApiResponse<TResponse>.Success(default!);

            // Explicitly verify content length only when specified
            if (response.Content.Headers.ContentLength.HasValue && response.Content.Headers.ContentLength.Value == 0)
                return ApiResponse<TResponse>.Success(default!);

            var data = await response.Content.ReadFromJsonAsync<TResponse>();
            if (data == null)
                return ApiResponse<TResponse>.Fail(Error);

            return ApiResponse<TResponse>.Success(data);
        }
    }
}