using System;

namespace PasswordQueryTool.Backend.QueueContracts
{
    /// <summary>
    /// Contract for requesting the state of an import.
    /// </summary>
    public interface IImportStateRequested
    {
        Guid ImportId { get; }
    }
}