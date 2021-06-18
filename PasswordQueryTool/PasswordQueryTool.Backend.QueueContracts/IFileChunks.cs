using System;
using System.Collections.Generic;

namespace PasswordQueryTool.Backend.QueueContracts
{
    /// <summary>
    /// Cotract for feedback about a possible fragmentation of a file for easier processing.
    /// </summary>
    public interface IFileChunks
    {
        IReadOnlyList<Chunk> Chunks { get; }

        Guid ImportId { get; }

        DateTime Timestamp { get; }
    }
}