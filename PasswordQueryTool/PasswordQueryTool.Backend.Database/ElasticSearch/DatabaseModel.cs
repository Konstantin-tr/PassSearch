using PasswordQueryTool.Model;
using System;

namespace PasswordQueryTool.Backend.Database.ElasticSearch
{
    internal class DatabaseModel
    {
        public string Domain
        {
            get;
            set;
        }

        public string Password
        {
            get;
            set;
        }

        public string UserName
        {
            get;
            set;
        }

        public static DatabaseModel Convert(LoginData data)
        {
            var domain = data.Email.FullDomain;
            var userName = data.Email.UserName;

            return new DatabaseModel()
            {
                Password = data.Password,
                Domain = domain,
                UserName = userName
            };
        }
    }
}