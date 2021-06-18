using System;

namespace PasswordQueryTool.Backend.QueueContracts
{
    /// <summary>
    /// Contract for indication that the import could not be started.
    /// </summary>
    public interface IImportRejected
    {
        string ErrorCode { get; }
        Guid ImportId { get; }

        DateTime Timestamp { get; }
    }
}