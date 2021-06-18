using PasswordQueryTool.ImportModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordQueryTool.ParseWebApp.Client.Services
{
    public interface IImportProvider
    {
        public Task<IEnumerable<ImportDTO>> GetRunningImports();        
        public Task<IEnumerable<string>> GetFiles();
        public Task<bool> StartImport(string filename);
        public Task CancelImport(Guid jobId);
    }
}
