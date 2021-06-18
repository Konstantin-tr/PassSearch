using System;

namespace PasswordQueryTool.Backend.Services.Parsing.StateMachines
{
    public enum ChunkState
    {
        Unprocessed = 0,
        Processing = 1,
        Processed = 2,
    }

    public class JobChunk
    {
        public ChunkState ChunkState { get; set; }
        public Guid Id { get; set; }

        public long Length { get; set; }
        public long Offset { get; set; }
        public ImportState Owner { get; set; }
    }
}