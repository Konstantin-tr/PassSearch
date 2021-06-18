using System;

namespace PasswordQueryTool.Model
{
    /// <summary>
    /// A response to a query.
    /// </summary>
    public class QueryResponse
    {
        /// <summary>
        /// Gets or sets the resulting data of the query.
        /// </summary>
        public LoginData[] Data
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the total count of results to the query. May be 0 desipte the query returning values.
        /// </summary>
        public long TotalItemCount
        {
            get;
            set;
        }

        /// <summary>
        /// Creates an empty QueryResponse.
        /// </summary>
        /// <returns>An empty QueryResponse.</returns>
        public static QueryResponse Empty()
        {
            return new QueryResponse() { Data = Array.Empty<LoginData>(), TotalItemCount = 0 };
        }
    }
}