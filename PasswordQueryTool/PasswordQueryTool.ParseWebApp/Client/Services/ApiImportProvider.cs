using PasswordQueryTool.ImportModels;
using PasswordQueryTool.Parsing.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace PasswordQueryTool.ParseWebApp.Client.Services
{
    public class ApiImportProvider : IImportProvider
    {
        private readonly HttpClient _client;

        public ApiImportProvider(HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task CancelImport(Guid jobId)
        {
            await _client.DeleteAsync($"api/parsing/parsing/jobs/{jobId}");
        }

        public async Task<IEnumerable<string>> GetFiles()
        {
            var res = await _client.GetFromJsonAsync<ImportFile[]>("api/parsing/parsing/files");

            return res.Select(f => f.Name);
        }

        public async Task<IEnumerable<ImportDTO>> GetRunningImports()
        {
            var response = await _client.GetFromJsonAsync<ImportDTO[]>("api/parsing/parsing/jobs");

            return response;
        }

        public async Task<bool> StartImport(string filename)
        {
            return (await _client.PostAsJsonAsync<string>("api/parsing/parsing/jobs", filename)).IsSuccessStatusCode;
        }
    }
}
