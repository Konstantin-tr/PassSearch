using PasswordQueryTool.Model;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace PasswordQueryTool.WebApp.Client.Services
{
    public class ApiQueryService : IQueryService
    {
        private readonly HttpClient _client;

        public ApiQueryService(HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<QueryResponse> GetLoginDataByEmail(QueryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Filter))
            {
                return QueryResponse.Empty();
            }

            return await _client.GetFromJsonAsync<QueryResponse>($"api/db/queries/email/{request.Filter}");
        }

        public async Task<QueryResponse> GetLoginDataByUsername(QueryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Filter))
            {
                return QueryResponse.Empty();
            }

            return await _client.GetFromJsonAsync<QueryResponse>($"api/db/queries/search/username/{request.Filter}");
        }

        public async Task<MostCommonData> GetMostCommonPasswordByDomain(QueryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Filter))
            {
                return new MostCommonData();
            }

            return await _client.GetFromJsonAsync<MostCommonData>($"api/db/queries/common/domain/{request.Filter}");
        }

        public async Task<MostCommonData> GetMostCommonPasswordByTopLevelDomain(QueryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Filter))
            {
                return new MostCommonData();
            }

            return await _client.GetFromJsonAsync<MostCommonData>($"api/db/queries/common/tld/{request.Filter}");
        }

        public async Task<long> GetTotalEntries()
        {
            return await _client.GetFromJsonAsync<long>($"api/db/queries/debug/amount");
        }
    }
}