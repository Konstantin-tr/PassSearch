using Nest;
using PasswordQueryTool.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordQueryTool.Backend.Database.ElasticSearch
{
    public class ElasticDatabaseHelperService : IDatabaseHelper
    {
        private readonly IElasticClient _elastic;

        public ElasticDatabaseHelperService(IElasticClient elastic)
        {
            _elastic = elastic ?? throw new ArgumentNullException(nameof(elastic));
        }

        public async Task<int> BulkInsertDataAsync(IEnumerable<LoginData> datas)
        {
            var array = datas.Select(d => DatabaseModel.Convert(d));

            var result = await _elastic.BulkAsync(i => i
                .Index("passwords")
                .IndexMany(array));

            return result.ItemsWithErrors.Count();
        }

        public void EmptyDatabase()
        {
            _elastic.DeleteByQuery<DatabaseModel>(s => s.MatchAll());
        }

        public async Task<long> GetAmountOfEntries()
        {
            var count = await _elastic.CountAsync<DatabaseModel>(s =>
    s.Query(q => q.MatchAll())
);
            return count.Count;
        }

        public async Task<QueryResponse> GetDataByDomainAsync(QueryRequest request)
        {
            var amount = 1000;
            var offset = 0;

            var count = await _elastic.CountAsync<DatabaseModel>(s =>
                s.Query(q =>
                    q.Term(f => f.Domain, request.Filter)
                )
            );

            var result = await _elastic.SearchAsync<DatabaseModel>(s =>
                s.From(offset).Size(amount).Query(q =>
                    q.Term(f => f.Domain, request.Filter)
                )
            );

            var data = result.Documents.Select(d =>
                new LoginData()
                {
                    Password = d.Password,
                    Email = new EmailData()
                    {
                        UserName = d.UserName,
                        FullDomain = d.Domain
                    }
                }
            ).ToArray();

            return new QueryResponse() { Data = data, TotalItemCount = count.Count };
        }

        public async Task<List<LoginData>> GetDataByFullEmailAsync(string username, string domain)
        {
            var result = await _elastic.SearchAsync<DatabaseModel>(s =>
                s.From(0).Take(100).Query(q =>
                    q.Bool(b => b
                        .Must(
                        mu => mu.Term(t => t.Field(f => f.Domain).Value(domain)),
                        mu => mu.Term(t => t.Field(f => f.UserName).Value(username))
                    )
                )
            )
          );

            return result.Documents.Select(d =>
                new LoginData()
                {
                    Password = d.Password,
                    Email = new EmailData()
                    {
                        UserName = d.UserName,
                        FullDomain = d.Domain
                    }
                }
            ).ToList();
        }

        public async Task<List<LoginData>> GetDataByTopLevelDomainAsync(string tld)
        {
            var result = await _elastic.SearchAsync<DatabaseModel>(s =>
                s.From(0).Take(1000).Query(q =>
                    q.Wildcard(w => w.Field(f => f.Domain).Value($"*{tld}"))
                    )
            );

            return result.Documents.Select(d =>
                new LoginData()
                {
                    Password = d.Password,
                    Email = new EmailData()
                    {
                        UserName = d.UserName,
                        FullDomain = d.Domain
                    }
                }
            ).ToList();
        }

        public async Task<List<LoginData>> GetDataByUsernameAsync(string username)
        {
            var result = await _elastic.SearchAsync<DatabaseModel>(s =>
    s.From(0).Take(100).Query(q =>
        q.Bool(b => b
            .Must(
            mu => mu.Term(t => t.Field(f => f.UserName).Value(username))
        )
    )
)
);

            return result.Documents.Select(d =>
                new LoginData()
                {
                    Password = d.Password,
                    Email = new EmailData()
                    {
                        UserName = d.UserName,
                        FullDomain = d.Domain
                    }
                }
            ).ToList();
        }

        public async Task<MostCommonData> GetMostCommonByDomainAsync(string domain)
        {
            var results = await _elastic.SearchAsync<DatabaseModel>(s =>
                s.Query(q => q
                    .Term(t => t.Domain, domain)
                )
                .Aggregations(a => a.Terms("password_how_often", st => st
                    .Field(p => p.Password)
                    .MinimumDocumentCount(2)
                    .Size(100)
                    .Order(o => o.CountDescending())
                    )
                )
            );

            var buckets = results.Aggregations.Terms("password_how_often").Buckets;

            var data = new MostCommonData() { Data = buckets.Select(s => new MostCommonDataInstance(s.Key, s.DocCount ?? 0)).ToList() };

            return data;
        }

        public async Task<MostCommonData> GetMostCommonByTopLevelDomainAsync(string tld)
        {
            var results = await _elastic.SearchAsync<DatabaseModel>(s =>
                s.Query(q =>
                    q.Wildcard(w => w.Field(f => f.Domain).Value($"*{tld}"))
                    )
                .Aggregations(a => a.Terms("password_how_often", st => st
                    .Field(p => p.Password)
                    .MinimumDocumentCount(2)
                    .Size(100)
                    .Order(o => o.CountDescending())
                    )
                )
            );

            var buckets = results.Aggregations.Terms("password_how_often").Buckets;

            var data = new MostCommonData() { Data = buckets.Select(s => new MostCommonDataInstance(s.Key, s.DocCount ?? 0)).ToList() };

            return data;
        }

        public async Task<bool> InsertDataAsync(LoginData data)
        {
            var newDoc = DatabaseModel.Convert(data);

            var result = await _elastic.IndexDocumentAsync<DatabaseModel>(newDoc);

            return result.IsValid;
        }

        public void Setup()
        {
            var retryCount = 0;
            var retryLimit = 20;
            var retryDelay = 500;

            var done = false;

            while (!done)
            {
                try
                {
                    _elastic.Indices.Create("passwords", c => c.Map<DatabaseModel>(mm => mm
                        .Properties(p => p
                            .Keyword(t => t
                                .Name(n => n.Domain)
                                .Fields(ff => ff
                                     .Text(tt => tt
                                         .Name("query")))
                                )
                            .Keyword(t => t
                                .Name(n => n.UserName)
                                )
                            .Keyword(t => t
                                .Name(n => n.Password)
                                )
                            )
                         )
                    );

                    done = true;
                }
                catch (Exception)
                {
                    if (retryCount >= retryLimit)
                    {
                        done = true;
                        throw new InvalidOperationException("Cannot reach Elastic Search Database!");
                    }

                    retryCount++;
                    Task.Delay(retryDelay);
                }
            }
        }
    }
}