using System;
using System.Collections.Generic;

namespace PasswordQueryTool.Model
{
    /// <summary>
    /// The complete data of most common passwords for a query.
    /// </summary>
    public class MostCommonData
    {
        /// <summary>
        /// Gets or sets a list containing all data of the query.
        /// </summary>
        public List<MostCommonDataInstance> Data
        {
            get;
            set;
        }
    }

    /// <summary>
    /// An instance of a password and how often it appeared in the query.
    /// </summary>
    public record MostCommonDataInstance(string Password, long Count);
}