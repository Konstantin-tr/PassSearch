using System;

namespace PasswordQueryTool.Backend.QueueContracts
{
    /// <summary>
    /// Contract for submitting a file to the import process.
    /// </summary>
    public interface ISubmitImport
    {
        string FileName { get; }
        Guid ImportId { get; }
        DateTime Timestamp { get; }
    }
}