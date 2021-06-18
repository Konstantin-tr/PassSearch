using PasswordQueryTool.ImportModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordQueryTool.ParseWebApp.Client.Services
{
    public class MockImportProvider : IImportProvider
    {
        private string[] _files = new string[] { "one.txt", "passfile.txt", "adqvcxvqegsrts.md" };
        private List<ImportDTO> _imports = new() {
            new() { Name = "File.txt", ChunksAmount = 100, ChunksFinishedAmount = 25, Id = Guid.NewGuid(), InvalidLines = 24, LinesFinished = 300, State = ImportState.Importing },
            new() { Name = "Pass.txt", ChunksAmount = 231, ChunksFinishedAmount = 231, Id = Guid.NewGuid(), InvalidLines = 231, LinesFinished = 4000, State = ImportState.Finished },
            new() { Name = "Dump.txt", ChunksAmount = 1000, ChunksFinishedAmount = 0, Id = Guid.NewGuid(), InvalidLines = 0, LinesFinished = 0, State = ImportState.Analyzing },
        };

        public Task CancelImport(Guid jobId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> GetFiles() => Task.FromResult<IEnumerable<string>>(_files);

        public Task<IEnumerable<ImportDTO>> GetRunningImports() => Task.FromResult<IEnumerable<ImportDTO>>(_imports);

        public Task<bool> StartImport(string filename)
        {
            _imports.Add(new ImportDTO()
            {
                Id = Guid.NewGuid(),
                Name = filename,
                ChunksAmount = 2341,
                ChunksFinishedAmount = 1600,
                LinesFinished = 123013,
                InvalidLines = 1211,
                State = ImportState.Importing
            });

            return Task.FromResult(true);
        }
    }
}
