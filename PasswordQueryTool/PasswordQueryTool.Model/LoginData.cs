using System;

namespace PasswordQueryTool.Model
{
    /// <summary>
    /// Represents a single set of login data.
    /// </summary>
    public class LoginData
    {
        /// <summary>
        /// Gets or sets the email of the login data set.
        /// </summary>
        public EmailData Email
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the password of the email data set.
        /// </summary>
        public string Password
        {
            get;
            set;
        }
    }
}