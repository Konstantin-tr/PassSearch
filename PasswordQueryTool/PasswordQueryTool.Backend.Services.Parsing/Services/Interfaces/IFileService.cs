using PasswordQueryTool.Backend.Services.Parsing.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PasswordQueryTool.Backend.Services.Parsing.Services.Interfaces
{
    public interface IFileService
    {
        public Task<bool> FileExists(string filename);

        public Task<IEnumerable<FileModel>> GetAllFiles();

        public Task GetFileStream(string filename, Func<Stream, Task> executor);

        public Task GetFileStream(string filename, long offset, long length, Func<Stream, Task> executor);
    }
}