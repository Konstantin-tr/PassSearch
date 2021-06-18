using System;

namespace PasswordQueryTool.Backend.Database
{
    public class DatabaseOptions
    {
        public string ConnectionString
        {
            get;
            set;
        }

        public string SchemaName
        {
            get;
            set;
        }

        public string TableName
        {
            get;
            set;
        }
    }
}