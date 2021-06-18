using System;

namespace PasswordQueryTool.Backend.QueueContracts
{
    /// <summary>
    /// Contract for the feedback about an import of a file chunk into the db.
    /// </summary>
    public interface IChunkImportResultReceived
    {
        Guid ChunkJobId { get; }
        int DiscardedLines { get; }
        int ImportedLines { get; }
        Guid ImportId { get; }
        int TotalLines { get; }
    }
}