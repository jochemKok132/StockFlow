using System.Text.Json;
using StockFlow.Shared.Common;
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
            try
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<TResponse>>();
                if (result is not null)
                    return result;
            }
            catch (Exception exception) when (exception is JsonException or NotSupportedException)
            {
            }

            return response.IsSuccessStatusCode
                ? ApiResponse<TResponse>.Ok()
                : ApiResponse<TResponse>.Fail(Error);
        }
    }
}