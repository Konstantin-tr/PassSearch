using System;

namespace PasswordQueryTool.Backend.QueueContracts
{
    /// <summary>
    /// Contract for indicating that no import job could be found with this import id.
    /// </summary>
    public interface IImportJobNotFound
    {
        Guid ImportId { get; }
        DateTime Timestamp { get; }
    }
}