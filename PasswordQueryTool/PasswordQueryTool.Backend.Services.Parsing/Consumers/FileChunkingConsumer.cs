using MassTransit;
using PasswordQueryTool.Backend.QueueContracts;
using PasswordQueryTool.Backend.Services.Parsing.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static PasswordQueryTool.Backend.Services.Parsing.Consumers.FileChunker;

namespace PasswordQueryTool.Backend.Services.Parsing.Consumers
{
    public class FileChunkingConsumer : IConsumer<ISubmitFileForChunking>
    {
        private readonly IFileService _fileService;

        public FileChunkingConsumer(IFileService fileService)
        {
            _fileService = fileService;
        }

        public async Task Consume(ConsumeContext<ISubmitFileForChunking> context)
        {
            await _fileService.GetFileStream(context.Message.FileName, stream => ProcessFileStream(stream, context));
        }

        private async Task ProcessFileStream(Stream stream, ConsumeContext<ISubmitFileForChunking> context)
        {
            var chunker = new FileChunker(stream);

            var chunks = new List<FileChunk>();

            await foreach (var chunk in chunker.CreateFileChunks(1024 * 1024))
            {
                chunks.Add(chunk);
            }

            await context.Publish<IFileChunks>(new
            {
                Chunks = chunks.Select(c => new Chunk() { Length = c.Length, Offset = c.Offset }).ToList(),
                ImportId = context.Message.ImportId,
                Timestamp = InVar.Timestamp
            });

            await chunker.DisposeAsync();
        }
    }
}