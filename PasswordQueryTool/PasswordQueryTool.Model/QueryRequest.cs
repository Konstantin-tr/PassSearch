using System;

namespace PasswordQueryTool.Model
{
    /// <summary>
    /// A query with a given filter.
    /// </summary>
    public class QueryRequest
    {
        /// <summary>
        /// Gets or sets the filter of the query.
        /// </summary>
        public string Filter
        {
            get;
            set;
        }
    }
}