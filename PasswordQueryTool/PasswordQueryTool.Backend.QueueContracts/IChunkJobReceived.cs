using System;

namespace PasswordQueryTool.Backend.QueueContracts
{
    /// <summary>
    /// Contract for transmitting a file chunk for computation to a processor for eextraction of passwords.
    /// </summary>
    public interface IChunkJobReceived
    {
        Chunk Chunk { get; }
        Guid ChunkJobId { get; }
        string Filename { get; }
        Guid ImportId { get; }
        DateTime Timestamp { get; }
    }
}