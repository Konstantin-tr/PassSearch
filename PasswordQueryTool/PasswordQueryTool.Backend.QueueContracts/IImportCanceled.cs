using System;
using System.Collections.Generic;
using System.Text;

namespace PasswordQueryTool.Backend.QueueContracts
{
    /// <summary>
    /// Contract for indicating a successfully canceled import.
    /// </summary>
    public interface IImportCanceled
    {
        Guid ImportId { get; }
    }
}
