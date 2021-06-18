using System;

namespace PasswordQueryTool.Backend.QueueContracts
{
    /// <summary>
    /// Contract for transmitting the current state of an import.
    /// </summary>
    public interface IImportState
    {
        public int DiscardedLines { get; set; }
        public int ImportedLines { get; set; }
        Guid ImportId { get; }
        public int ProcessedChunks { get; set; }
        public int ProcessedLines { get; set; }
        string State { get; }
        public int TotalChunks { get; set; }

        public string FileName { get; set; }
    }
}