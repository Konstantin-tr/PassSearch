using Automatonymous;
using System;
using System.Collections.Generic;

namespace PasswordQueryTool.Backend.Services.Parsing.StateMachines
{
    public class ImportState : SagaStateMachineInstance
    {
        public ICollection<JobChunk> Chunks { get; set; }
        public Guid CorrelationId { get; set; }

        public DateTime? CreateTimestamp { get; set; }
        public string CurrentState { get; set; }
        public int DiscardedLines { get; set; }
        public string FileName { get; set; }
        public int ImportedLines { get; set; }
        public int ProcessedChunks { get; set; }
        public int ProcessedLines { get; set; }
        public int? TotalChunks { get; set; }
    }
}