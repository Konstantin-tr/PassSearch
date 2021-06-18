using System;

namespace PasswordQueryTool.Backend.QueueContracts
{
    /// <summary>
    /// Contract for confirmation that an import was submitted.
    /// </summary>
    public interface IImportSubmitted
    {
        Guid ImportId { get; }

        DateTime Timestamp { get; }
    }
}