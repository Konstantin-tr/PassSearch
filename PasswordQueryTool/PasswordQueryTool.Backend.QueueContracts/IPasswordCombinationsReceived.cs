using System;
using System.Collections.Generic;

namespace PasswordQueryTool.Backend.QueueContracts
{
    /// <summary>
    /// Contract for transmitting a block of password combinations to indexation in the database
    /// </summary>
    public interface IPasswordCombinationsReceived
    {
        Guid ChunkJobId { get; }
        int DiscardedLinesCount { get; }
        Guid ImportId { get; }
        IList<PasswordCombination> PasswordCombinations { get; }
        DateTime Timestamp { get; }
    }
}