using System;

namespace PasswordQueryTool.Model
{
    /// <summary>
    /// Represents an email address.
    /// </summary>
    public class EmailData
    {
        /// <summary>
        /// Constructs an EmailData instance from a given email.
        /// </summary>
        /// <param name="fullEmail">The email to split. Needs to be a valid email.</param>
        public EmailData(string fullEmail)
        {
            var atIndex = fullEmail.IndexOf('@');

            if (atIndex < 0)
                throw new ArgumentException(fullEmail);

            UserName = fullEmail.Substring(0, atIndex);
            FullDomain = fullEmail.Substring(atIndex + 1);
        }

        /// <summary>
        /// Constructs an empty EmailData instance.
        /// </summary>
        public EmailData()
        {
        }

        /// <summary>
        /// Gets or sets the full domain of the email.
        /// </summary>
        public string FullDomain
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the username of the email.
        /// </summary>
        public string UserName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the full email as a string.
        /// </summary>
        /// <returns>The full email.</returns>
        public string GetFullEmail() => $"{UserName}@{FullDomain}";
    }
}