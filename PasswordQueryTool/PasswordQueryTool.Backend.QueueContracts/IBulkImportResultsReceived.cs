using System;
using System.Collections.Generic;
using System.Text;

namespace PasswordQueryTool.Backend.QueueContracts
{
    /// <summary>
    /// Contract for receiving multiple imported chunks at once.
    /// </summary>
    public interface IBulkImportResultsReceived
    {
        public Guid ImportId { get; }

        public List<IChunkImportResultReceived> Chunks { get; }
    }
}
