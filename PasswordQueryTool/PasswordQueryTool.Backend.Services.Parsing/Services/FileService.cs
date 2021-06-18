using Minio;
using Minio.DataModel;
using Minio.Exceptions;
using PasswordQueryTool.Backend.Services.Parsing.Models;
using PasswordQueryTool.Backend.Services.Parsing.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace PasswordQueryTool.Backend.Services.Parsing.Services
{
    public class FileService : IFileService
    {
        private const string BucketName = "dumps";
        private readonly MinioClient _client;

        public FileService(MinioClient client)
        {
            _client = client;
        }

        public async Task<bool> FileExists(string filename)
        {
            try
            {
                var stats = await _client.StatObjectAsync(BucketName, filename);
                return true;
            }
            catch (MinioException)
            {
                return false;
            }
        }

        public async Task<IEnumerable<FileModel>> GetAllFiles()
        {
            await EnsureBucket();

            var observable = _client.ListObjectsAsync(BucketName, recursive: true);

            var files = new List<Item>();

            var awaiter = new ObservableAwaiter<Item>();

            observable.Subscribe(awaiter);

            await awaiter.WaitForCompletionAsync();

            return awaiter.Items.Where(i => !i.IsDir).Select(i => new FileModel { FileName = i.Key, FileSize = (long)i.Size }).ToList();
        }

        public async Task GetFileStream(string filename, Func<Stream, Task> executor)
        {
            await EnsureBucket();
            await _client.GetObjectAsync(BucketName, filename, s => executor.Invoke(s).Wait());
        }

        public async Task GetFileStream(string filename, long offset, long length, Func<Stream, Task> executor)
        {
            await EnsureBucket();
            await _client.GetObjectAsync(BucketName, filename, offset, length, s => executor.Invoke(s).Wait());
        }

        private async Task EnsureBucket()
        {
            if (await _client.BucketExistsAsync(BucketName))
            {
                return;
            }

            await _client.MakeBucketAsync(BucketName);
        }
    }

    public class ObservableAwaiter<T> : IObserver<T>
    {
        private TaskCompletionSource _completionSource = new TaskCompletionSource();
        private List<T> _items = new List<T>();
        public IReadOnlyList<T> Items => _items;

        public void OnCompleted()
        {
            _completionSource.SetResult();
        }

        public void OnError(Exception error)
        {
            _completionSource.SetException(error);
        }

        public void OnNext(T value)
        {
            _items.Add(value);
        }

        public Task WaitForCompletionAsync()
        {
            return _completionSource.Task;
        }
    }
}