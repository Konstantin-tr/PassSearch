using System;
using System.Collections.Generic;
using System.Text;

namespace PasswordQueryTool.Backend.QueueContracts
{
    /// <summary>
    /// Contract for requesting the cancellation of an import.
    /// </summary>
    public interface IRequestCancellation
    {
        Guid ImportId { get; }
    }
}
