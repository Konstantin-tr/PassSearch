using PasswordQueryTool.Model;
using System;
using System.Threading.Tasks;

namespace PasswordQueryTool.WebApp.Client.Services
{
    public interface IQueryService
    {
        public Task<long> GetTotalEntries();
        public Task<QueryResponse> GetLoginDataByEmail(QueryRequest request);
        public Task<QueryResponse> GetLoginDataByUsername(QueryRequest request);

        public Task<MostCommonData> GetMostCommonPasswordByDomain(QueryRequest request);

        public Task<MostCommonData> GetMostCommonPasswordByTopLevelDomain(QueryRequest request);
    }
}