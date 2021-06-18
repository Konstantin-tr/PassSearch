using PasswordQueryTool.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PasswordQueryTool.Backend.Database
{
    /// <summary>
    /// An interface to provide easy access to the backing database and to enable all required queries.
    /// </summary>
    public interface IDatabaseHelper
    {
        /// <summary>
        /// Inserts an enumerable of data.
        /// </summary>
        /// <param name="datas">The data used for the insert.</param>
        /// <returns>The amount of rows that were inserted.</returns>
        Task<int> BulkInsertDataAsync(IEnumerable<LoginData> datas);

        /// <summary>
        /// Gets the amount of entries in the db.
        /// </summary>
        /// <returns>The amount of entries in the db.</returns>
        Task<long> GetAmountOfEntries();

        /// <summary>
        /// Empties the database.
        /// </summary>
        void EmptyDatabase();

        /// <summary>
        /// Gets the data by the domain.
        /// </summary>
        /// <param name="request">The request to query for.</param>
        /// <returns>A list of login data that matches the domain.</returns>
        Task<QueryResponse> GetDataByDomainAsync(QueryRequest request);

        /// <summary>
        /// Gets the data by matching the entire email.
        /// </summary>
        /// <param name="username">The username of the email.</param>
        /// <param name="domain">The domain of the email.</param>
        /// <returns>A list of login data that matches the full email.</returns>
        Task<List<LoginData>> GetDataByFullEmailAsync(string username, string domain);

        /// <summary>
        /// Gets the data by matching the username.
        /// </summary>
        /// <param name="username">The username of the email.</param>
        /// <returns>A list of login data that matches the username.</returns>
        Task<List<LoginData>> GetDataByUsernameAsync(string username);

        /// <summary>
        /// Gets the data by matching the top level domain.
        /// </summary>
        /// <param name="topLevelDomain">The top level domain.</param>
        /// <returns>A list of login data that matches the top level domain.</returns>
        Task<List<LoginData>> GetDataByTopLevelDomainAsync(string topLevelDomain);

        /// <summary>
        /// Gets the most common passwords by domain.
        /// </summary>
        /// <param name="domain">The domain to query for.</param>
        /// <returns>The data of the most common passwords for the domain specified.</returns>
        Task<MostCommonData> GetMostCommonByDomainAsync(string domain);

        /// <summary>
        /// Gets the most common passwords by top level domain.
        /// </summary>
        /// <param name="tld">The top level domain to query for.</param>
        /// <returns>The data of the most common passwords for the top level domain specified.</returns>
        Task<MostCommonData> GetMostCommonByTopLevelDomainAsync(string tld);

        /// <summary>
        /// Inserts a single row.
        /// </summary>
        /// <param name="data">The data to insert.</param>
        /// <returns>True if the insert was successful, false if otherwise.</returns>
        Task<bool> InsertDataAsync(LoginData data);

        /// <summary>
        /// Sets up the database. Should be called at least once at the beginning of the database access service.
        /// </summary>
        void Setup();
    }
}