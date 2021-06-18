using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PasswordQueryTool.Backend.Services.Parsing.Consumers
{
    public class FileChunker : IAsyncDisposable
    {
        private readonly Stream _stream;

        public FileChunker(Stream stream)
        {
            _stream = stream;
        }

        public async IAsyncEnumerable<FileChunk> CreateFileChunks(int minimalChunkSize)
        {
            if (minimalChunkSize < 1)
            {
                throw new ArgumentOutOfRangeException();
            }

            var bufferSize = 10 * 1024;

            using var buffer = MemoryPool<byte>.Shared.Rent(bufferSize);
            using var decodingTarget = MemoryPool<char>.Shared.Rent(bufferSize);

            Encoding encoding;

            var bytesRead = await _stream.ReadAsync(buffer.Memory);

            using (var memoryStream = new MemoryStream(buffer.Memory.ToArray()))
            using (var streamReader = new StreamReader(memoryStream))
            {
                streamReader.Peek();
                streamReader.Read();

                encoding = streamReader.CurrentEncoding;
            }

            var decoder = encoding.GetDecoder();

            long lastChunkEndIndex = 0;

            long currentBufferIndex = 0;

            while (true)
            {
                long chunkStart = 0;

                if (currentBufferIndex + bytesRead < lastChunkEndIndex + minimalChunkSize)
                {
                    currentBufferIndex += bytesRead;
                    bytesRead = await _stream.ReadAsync(buffer.Memory);

                    if (bytesRead == 0)
                    {
                        break;
                    }

                    continue;
                }

                var bufferStart = Math.Max(0, (int)((lastChunkEndIndex + minimalChunkSize) - currentBufferIndex));
                try
                {
                    decoder.Convert(buffer.Memory.Slice(bufferStart, bytesRead - bufferStart).Span, decodingTarget.Memory.Span, false, out var bytesUsed, out var charsUsed, out var completed);

                    var index = decodingTarget.Memory.Slice(0, charsUsed).Span.IndexOf('\n');

                    if (index == -1)
                    {
                        currentBufferIndex += bytesRead;
                        bytesRead = await _stream.ReadAsync(buffer.Memory);

                        if (bytesRead == 0)
                        {
                            break;
                        }

                        continue;
                    }

                    var byteCount = encoding.GetByteCount(decodingTarget.Memory.Slice(0, index + 1).Span);

                    chunkStart = lastChunkEndIndex;
                    lastChunkEndIndex += minimalChunkSize + byteCount;
                }
                catch (Exception e)
                {
                    // TODO Fix?
                }
                yield return new FileChunk() { Offset = chunkStart, Length = lastChunkEndIndex - chunkStart };
            }

            yield return new FileChunk() { Offset = lastChunkEndIndex, Length = currentBufferIndex - lastChunkEndIndex };
        }

        public async ValueTask DisposeAsync()
        {
            await this._stream.DisposeAsync();
        }

        public struct FileChunk
        {
            public long Length { get; set; }
            public long Offset { get; set; }
        }
    }
}