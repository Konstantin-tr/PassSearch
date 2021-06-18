using System;

namespace PasswordQueryTool.Backend.QueueContracts
{
    /// <summary>
    /// Contract for submitting a file for chunking in order to get a fragmented file that is easier to process.
    /// </summary>
    public interface ISubmitFileForChunking
    {
        string FileName { get; }
        Guid ImportId { get; }
        DateTime Timestamp { get; }
    }
}