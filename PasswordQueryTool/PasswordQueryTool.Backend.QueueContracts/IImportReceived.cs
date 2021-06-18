using System;

namespace PasswordQueryTool.Backend.QueueContracts
{
    /// <summary>
    /// Contract indicating that a new import job was successfully received.
    /// </summary>
    public interface IImportReceived
    {
        string FileName { get; }
        Guid ImportId { get; }
        DateTime Timestamp { get; }
    }
}