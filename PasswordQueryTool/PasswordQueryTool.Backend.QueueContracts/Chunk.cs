using System;

namespace PasswordQueryTool.Backend.QueueContracts
{
    /// <summary>
    /// Contract container class representing a region of a file.
    /// </summary>
    public class Chunk
    {
        public long Length { get; set; }
        public long Offset { get; set; }
    }
}