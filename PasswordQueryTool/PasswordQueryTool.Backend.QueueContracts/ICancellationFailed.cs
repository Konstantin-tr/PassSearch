using System;
using System.Collections.Generic;
using System.Text;

namespace PasswordQueryTool.Backend.QueueContracts
{
    /// <summary>
    /// Contract for an unseccussful cancellation of a import job.
    /// </summary>
    public interface ICancellationFailed
    {
        Guid ImportId { get; }
        string ErrorCode { get; }
     }
}
